using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Magas.Core
{
    [Serializable]
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public List<MailboxAccount> MailboxAccounts { get; set; }

        public User()
        {
            this.MailboxAccounts = new List<MailboxAccount>();
        }

        public bool HasAuthenticationCredential(string username, string password)
        {
            return this.Username == username && this.Password == password;
        }
    }
}
