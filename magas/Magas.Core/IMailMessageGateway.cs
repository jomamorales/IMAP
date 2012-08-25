using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using Magas.Core;

namespace Magas.Core
{
    public interface IMailMessageGateway
    {
        IList<MailMessage> GetMailMessagesOfAccount(MailboxAccount account);
        MailMessage GetMailMessageOfAccountById(MailboxAccount account, string messageId);
        void DeleteMailMessageOfAccountById(MailboxAccount account, string messageId);
    }
}
