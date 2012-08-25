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
    internal class XmlMailboxAccountMapper
        : IDataMapper<MailboxAccount>
    {
        string filespath = string.Empty;

        public XmlMailboxAccountMapper()
        {
            this.filespath = ConfigurationManager.AppSettings["DataDirectory"];
            if (string.IsNullOrEmpty(this.filespath))
            {
                throw new InvalidOperationException("No data directory was specified");
            }
            this.filespath += @"\data.xml";   
        }


        public IList<MailboxAccount> GetAll()
        {
            XmlUserMapper userMapper = new XmlUserMapper();
            IList<MailboxAccount> accounts = new List<MailboxAccount>();
            IList<User> users = userMapper.GetAll();
            foreach (User item in users)
            {
                foreach (MailboxAccount account in item.MailboxAccounts)
                {
                    accounts.Add(account);
                }
            }
            return accounts;
        }

        public MailboxAccount GetById(object id)
        {

            IList<MailboxAccount> items = this.GetAll();
            foreach (MailboxAccount item in items)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public IList<MailboxAccount> GetByField(string fieldname, object value)
        {
            PropertyInfo pi = typeof(User).GetProperty(fieldname, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            if (pi == null)
            {
                throw new InvalidOperationException("Property " + fieldname + " not belong to MailboxAccount type");
            }

            IList<MailboxAccount> items = this.GetAll();
            foreach (MailboxAccount item in items)
            {
                if (value.Equals(pi.GetValue(item, new object[]{})))
                {
                    List<MailboxAccount> us = new List<MailboxAccount>();
                    us.Add(item);
                    return us;
                }
            }
            return null;
        }


        
    }
}
