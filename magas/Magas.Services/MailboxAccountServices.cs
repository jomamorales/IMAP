using System;
using System.Collections.Generic;
using System.Text;
using Magas.Core;
using Magas.Data;
using Magas.Core;
using Magas.Data;

namespace Magas.Services
{
    public class MailboxAccountServices
    {
        private IDataContext dataContext;

        public MailboxAccountServices()
        {
            this.dataContext = new DataAccessProviderFactory().GetDataContext();
        }


        public MailboxAccount GetMailboxAccountById(string userId, string id)
        {
            User user = this.dataContext.GetById<User>(userId);
            foreach (MailboxAccount item in user.MailboxAccounts)
            {
                if (item.Id.Equals(id))
                {
                    return item;
                }
            }
            return null;
        }

        public void DeleteMailboxAccount(string userId, string accountId)
        {
            MailboxAccount account = this.GetMailboxAccountById(userId, accountId);
            this.dataContext.Delete(account);
        }

        public void CreateMailboxAccount(string userId, MailboxAccount account)
        {
            if (!string.IsNullOrEmpty(account.Id))
            {
                throw new ArgumentException("Id must be null or empty", "Id");
            }

            if (string.IsNullOrEmpty(account.Name))
            {
                throw new ArgumentNullException("Name");
            }

            if (string.IsNullOrEmpty(account.Username))
            {
                throw new ArgumentNullException("Username");
            }

            if (string.IsNullOrEmpty(account.Server))
            {
                throw new ArgumentNullException("Server");
            }

            User user = this.dataContext.GetById<User>(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User with id " + userId + " not found");
            }
            account.Id = Guid.NewGuid().ToString();
            user.MailboxAccounts.Add(account);
            this.dataContext.Save(user);
        }

        public void SaveMailboxAccount(string userId, MailboxAccount account)
        {
            MailboxAccount savedAccount = this.dataContext.GetById<MailboxAccount>(account.Id);
             
            if (savedAccount == null)
            {
                throw new ArgumentException("Account not found.", "Account");
            }

            if (string.IsNullOrEmpty(account.Name))
            {
                throw new ArgumentNullException("Name");
            }

            if (string.IsNullOrEmpty(account.Username))
            {
                throw new ArgumentNullException("Username");
            }

            if (string.IsNullOrEmpty(account.Server))
            {
                throw new ArgumentNullException("Server");
            }

            User user = this.dataContext.GetById<User>(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User with id " + userId + " not found");
            }

            for (int i = 0; i < user.MailboxAccounts.Count; i++)
            {
                if (user.MailboxAccounts[i].Id.Equals(account.Id))
                {
                    user.MailboxAccounts[i] = account;
                    break;
                }
            }

            this.dataContext.Save(user);
        }
    }
}
