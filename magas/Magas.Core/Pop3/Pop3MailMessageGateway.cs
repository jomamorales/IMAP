using System;
using System.Collections.Generic;
using System.Text;
using Pop3;

namespace Magas.Core.Pop3
{
    public class Pop3MailMessageGateway
        : IMailMessageGateway
    {
        #region IMailMessageGateway Members

        public IList<MailMessage> GetMailMessagesOfAccount(MailboxAccount account)
        {
            Pop3Client client = null;
            IList<MailMessage> messages = null;

            try
            {
                client = new Pop3Client(account.Username, account.Password, account.Server);
                client.OpenInbox();

                messages = new List<MailMessage>();

                while (client.NextEmail())
                {
                    
                    MailMessage message = new MailMessage() { Id = client.Date.Ticks.ToString(), To = client.To, From = client.From, Subject = client.Subject, Body = client.Body, Date=client.Date };
                    if (client.ContentType != null)
                    {
                        message.Format = client.ContentType.ToLower().Contains("html") ? MailMessageFormat.Html : MailMessageFormat.Text;
                        message.ContentType = client.ContentType;
                    }
                    else
                    {
                        message.Format = MailMessageFormat.Text;
                        message.ContentType = string.Empty;
                    }
                    messages.Add(message);
                }
            }
            finally
            {
                client.CloseConnection();
            }
            return messages;
        }

        public MailMessage GetMailMessageOfAccountById(MailboxAccount account, string messageId)
        {
            IList<MailMessage> messages = this.GetMailMessagesOfAccount(account);
            foreach (var item in messages)
            {
                if (item.Id == messageId)
                {
                    return item;
                }
            }
            return null;
        }

        public void DeleteMailMessageOfAccountById(MailboxAccount account, string messageId)
        {
            Pop3Client client = null;

            try
            {
                client = new Pop3Client(account.Username, account.Password, account.Server);
                client.OpenInbox();

                while (client.NextEmail())
                {
                    string id = client.Date.Ticks.ToString();
                    bool result = client.DeleteEmail();
                    if (!result)
                    {
                        throw new ApplicationException("Mail message not deleted");
                    }
                }
            }

            finally
            {
                client.CloseConnection();
            }
        }
     
        #endregion
    }
}
