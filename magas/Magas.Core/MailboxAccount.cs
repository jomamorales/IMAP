using System;
using System.Collections.Generic;
using System.Text;

namespace Magas.Core
{
    [Serializable]
    public class MailboxAccount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public short Port { get; set; }
        public bool UseSSL { get; set; }
        public MailboxAccountType AccountType { get; set; }
        public string FoldersToBeDownloadAllMessages { get; set; }
        public string FoldersToBeDownloadOnlyNewMessages { get; set; }

        public MailboxAccount()
        {
            this.Port = 110;
        }
    }

    public enum MailboxAccountType
    { 
    None = 0, Pop3, Imap
    }
}
