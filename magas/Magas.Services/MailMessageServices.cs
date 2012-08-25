using System;
using System.Collections.Generic;
using System.Text;
using Magas.Core;
using Magas.Core;

namespace Magas.Services
{
    public class MailMessageServices
    {
        public IList<MailMessage> GetMailMessagesOfAccount(string userId, string accountId)
        {
            MailboxAccount account = new MailboxAccountServices().GetMailboxAccountById(userId, accountId);
            if (account == null)
            {
                throw new InvalidOperationException("No account found with id " + accountId);
            }

            IList<MailMessage> messages = new MailMessageGatewayFactory().GetMailMessageGateway(account).GetMailMessagesOfAccount(account);

            return messages;
        }

        public MailMessage GetMailMessageOfAccountById(string userId, string accountId, string messageId)
        {
            MailboxAccount account = new MailboxAccountServices().GetMailboxAccountById(userId, accountId);
            if (account == null)
            {
                throw new InvalidOperationException("No account found with id " + accountId);
            }

            MailMessage message = new MailMessageGatewayFactory().GetMailMessageGateway(account).GetMailMessageOfAccountById(account, messageId);

            return message;
        }

        public void DeleteMailMessageOfAccount(string userId, string accountId, string messageId)
        {
            MailboxAccount account = new MailboxAccountServices().GetMailboxAccountById(userId, accountId);
            if (account == null)
            {
                throw new InvalidOperationException("No account found with id " + accountId);
            }

            new MailMessageGatewayFactory().GetMailMessageGateway(account).DeleteMailMessageOfAccountById(account, messageId);
        }
    }
}
