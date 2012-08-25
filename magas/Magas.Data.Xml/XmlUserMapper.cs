using System;
using System.Collections.Generic;
using System.Text;
using Magas.Core;
using Magas.Core;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Magas.Data.Xml
{
    internal class XmlUserMapper
        : IDataMapper<User>
    {
        string filespath = string.Empty;

        public XmlUserMapper()
        {
            this.filespath = ConfigurationManager.AppSettings["DataDirectory"];
            if (string.IsNullOrEmpty(this.filespath))
            {
                throw new InvalidOperationException("No data directory was specified");
            }

            this.filespath += @"\data.xml";   
        }


        public IList<User> GetAll()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            using (Stream stream = File.OpenRead(this.filespath))
            {
                IList<User> users = (IList<User>)serializer.Deserialize(stream);
                if (users == null)
                {
                    return new List<User>();
                }
                return users;
            }
        }

        public User GetById(object id)
        {

            IList<User> items = this.GetAll();
            foreach (User item in items)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public IList<User> GetByField(string fieldname, object value)
        {
            PropertyInfo pi = typeof(User).GetProperty(fieldname, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            if (pi == null)
            {
                throw new InvalidOperationException("Property " + fieldname + " not belong to User type");
            }
            
            IList<User> users = this.GetAll();
            foreach (User user in users)
            {
                if (value.Equals(pi.GetValue(user, new object[]{})))
                {
                    List<User> us = new List<User>();
                    us.Add(user);
                    return us;
                }
            }
            return null;
        }

        public void SaveUser(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
            throw new ArgumentNullException("Id");
            }

            IList<User> users = this.GetAll();
            for (int i = 0; i < users.Count; i++)
            {
                if (user.Id.Equals(users[i].Id))
                {
                    users[i] = user;
                    this.SaveUsers(users);
                    return;
                }
            }
        }

        private void SaveUsers(IList<User> users)
        {
            File.Move(this.filespath, this.filespath + ".bkp");
            using (Stream stream = File.OpenWrite(this.filespath))
            {
                XmlSerializer serializer = new XmlSerializer(users.GetType());
                serializer.Serialize(stream, users);
            }
            File.Delete(this.filespath + ".bkp");
        }

        private void SaveFakeUsers()
        {
            //IList<User>  users = new List<User>();
            //for (int i = 0; i < 3; i++)
            //{
            //    User u = new User() { Guid = Guid.NewGuid().ToString(), Name = i.ToString(), Password=i.ToString(), Surname = i.ToString(), Username=i.ToString()};
            //    users.Add(u);
            //    MailboxAccount account = new MailboxAccount();
            //    account.Guid = Guid.NewGuid().ToString();
            //    account.Name = i.ToString();
            //    account.Username= i.ToString();
            //    account.Password = i.ToString();
            //    account.Server = i.ToString();
            //    account.AccountType = MalboxAccountType.Pop3;
            //    u.MailboxAccounts.Add(account);
            //}

            //using (Stream stream = File.OpenWrite(this.filespath))
            //{
            //    XmlSerializer serializer = new XmlSerializer(users.GetType());
            //    serializer.Serialize(stream, users);
            //}
        }
    }
}
