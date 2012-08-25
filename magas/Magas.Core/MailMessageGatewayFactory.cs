using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using Magas.Core.Imap;
using Magas.Core.Pop3;
using Magas.Core.Pop3;
using Magas.Core.Imap;

namespace Magas.Core
{
    /// <summary>
    /// Summary description for MailMessageGatewayFactory.
    /// </summary>
    public class MailMessageGatewayFactory : IMailMessageGatewayFactory
    {
        //private static Assembly activeProvider = null;
        //private static IMailMessageGatewayFactory activeMailMessageGatewayFactory = null;

        //static MailMessageGatewayFactory()
        //{
        //    string providerName = ConfigurationManager.AppSettings["MailMessageGateway"];
        //    string providerFactoryName = ConfigurationManager.AppSettings["MailMessageGatewayFactory"];
        //    activeProvider = Assembly.Load(providerName);
        //    activeMailMessageGatewayFactory = (IMailMessageGatewayFactory)activeProvider.CreateInstance(providerFactoryName);
        //}

        #region IMailMessageGatewayFactory Members

        public IMailMessageGateway GetMailMessageGateway(MailboxAccount mailboxAccount)
        {
            if (mailboxAccount.AccountType == MailboxAccountType.None)
            {
                throw new InvalidOperationException("Can't create message gateway for account type none");
            }

            if (mailboxAccount.AccountType == MailboxAccountType.Pop3)
            {
               return new Pop3MailMessageGateway();
            }

            if (mailboxAccount.AccountType == MailboxAccountType.Imap)
            {
                return new ImapMailMessageGateway();
            }
            throw new NotSupportedException("Mailbox type not supported");
        }

        #endregion
    }
}
