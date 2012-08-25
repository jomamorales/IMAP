using System;
using System.Collections.Generic;
using System.Text;
using Koolwired.Imap;
using InterIMAP;
using System.Configuration;

namespace Magas.Core.Imap
{
    public class ImapMailMessageGateway
        : IMailMessageGateway
    {
        #region ImapMailMessageGateway Members

        public IList<MailMessage> GetMailMessagesOfAccount(MailboxAccount account)
        {
            string[] downloadAllMessagesFolders = GetFolderArrayFromString(account.FoldersToBeDownloadAllMessages);
            string[] downloadNewMessagesFolders = GetFolderArrayFromString(account.FoldersToBeDownloadOnlyNewMessages);

            IList<MailMessage> messages = new List<MailMessage>();
            IMAPConfig config = new IMAPConfig(account.Server, account.Username, account.Password, account.UseSSL, false, string.Empty);
            IMAPClient client = null;
            try
            {
                client = new IMAPClient(config, null, 0);
                client.Logon();
                IMAPFolderCollection folders =  client.Folders;
                ReadMessagesFromFolder(folders, messages, downloadAllMessagesFolders, downloadNewMessagesFolders);
            }
            finally
            {
                if (client != null)
                {
                    client.Logoff();
                    client = null;
                }
            }
            return messages;
        }


        public MailMessage GetMailMessageOfAccountById(MailboxAccount account, string messageId)
        {
            IList<MailMessage> messages = new List<MailMessage>();
            messages = this.GetMailMessagesOfAccount(account);
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
            string[] downloadAllMessagesFolders = GetFolderArrayFromString(account.FoldersToBeDownloadAllMessages);
            string[] downloadNewMessagesFolders = GetFolderArrayFromString(account.FoldersToBeDownloadOnlyNewMessages);

            IList<MailMessage> messages = new List<MailMessage>();
            IMAPConfig config = new IMAPConfig(account.Server, account.Username, account.Password, account.UseSSL, false, string.Empty);
            IMAPClient client = null;
            try
            {
                client = new IMAPClient(config, null, 0);
                client.Logon();

                SearchAndDeleteMessage(client.Folders, int.Parse(messageId));
            }
            finally
            {
                if (client != null)
                {
                    client.Logoff();
                    client = null;
                }
            }
        }
        #endregion

        private bool SearchAndDeleteMessage(IMAPFolderCollection folders, int uid)
        {
            foreach (var folder in folders)
            {
                IMAPMessage message = folder.GetMessageByID(uid);
                if (message == null)
                {
                    SearchAndDeleteMessage(folder.SubFolders, uid);
                    continue;
                }
                folder.DeleteMessage(message);
                return true;
            }
            return false;
        }


        private string[] GetFolderArrayFromString(string commaSeparatedList)
        {
            if (string.IsNullOrEmpty(commaSeparatedList))
            {
                return new string[] { };
            }
            string[] list = commaSeparatedList.Replace(", ", ",").Replace(" ,", ",").Split(',');
            return list;
        }

        private void ReadMessagesFromFolder(IMAPFolderCollection folders, IList<MailMessage> messages, string[] downloadAllMessagesFolders, string[] downloadNewMessagesFolders)
        {
            foreach (var folder in folders)
            {
                bool downloadAllMessages = StringArrayContainsString(downloadAllMessagesFolders, folder.FolderName);
                bool downloadNewMessages = !downloadAllMessages && StringArrayContainsString(downloadNewMessagesFolders, folder.FolderName);

                if (!downloadAllMessages && !downloadNewMessages)
                {
                    ReadMessagesFromFolder(folder.SubFolders, messages, downloadAllMessagesFolders, downloadNewMessagesFolders);
                    continue;
                }

                if (downloadAllMessages)
                {
                    IMAPMessageCollection folderMessages = folder.Messages;
                    foreach (var item in folderMessages)
                    {
                        MailMessage message = CreateMailMessageFromIMAPMessage(item);
                        messages.Add(message);
                    }
                }

                if (downloadNewMessages)
                {
                    IMAPSearchQuery query = new IMAPSearchQuery();
                    query.NotSeen = true;

                    IMAPSearchResult results = folder.Search(query);
                    foreach (var item in results.Messages)
                    {
                        MailMessage message = CreateMailMessageFromIMAPMessage(item);
                        messages.Add(message);
                    }
                }

                ReadMessagesFromFolder(folder.SubFolders, messages, downloadAllMessagesFolders, downloadNewMessagesFolders);
            }
        }

        private bool StringArrayContainsString(string[] list, string matchString)
        {
            matchString = string.IsNullOrEmpty(matchString) ? string.Empty : matchString;
            foreach (var item in list)
            {
                if (item.ToLower().Equals(matchString.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private MailMessage CreateMailMessageFromIMAPMessage(IMAPMessage imapMessage)
        {
            if (imapMessage == null)
            {
                return null;
            }
            string from = GetAddressesStringFromAddressList(imapMessage.From);
            string to = GetAddressesStringFromAddressList(imapMessage.To);

            IMAPMessageContent plainContent = GetTextDataFromIMAPMessage(imapMessage);
            IMAPMessageContent htmlContent = GetHtmlDataFromIMAPMessage(imapMessage);

            string body = string.Empty;
            MailMessageFormat format = MailMessageFormat.None;
            string contentType = string.Empty;

            if (htmlContent != null)
            {
                format = MailMessageFormat.Html;
                body = htmlContent.TextData;
                contentType = htmlContent.ContentType;
            }
            else if (plainContent != null)
            {
                format = MailMessageFormat.Text;
                body = plainContent.TextData;
                contentType = plainContent.ContentType;
            }

            return new MailMessage()
            {
                Id = imapMessage.Uid.ToString()
                ,
                Format = format
                ,
                ContentType = contentType
                ,
                To = to
                ,
                From = from
                ,
                Subject = imapMessage.Subject
                ,
                Body = body
                ,
                Date = imapMessage.Date
                ,
                FolderPath = imapMessage.Folder.FolderPath
            };
        }

        private IMAPMessageContent GetHtmlDataFromIMAPMessage(IMAPMessage message)
        {
            foreach (IMAPMessageContent content in message.BodyParts)
            {
                if (content.ContentType.ToLower().Contains("html"))
                    return content;
            }
            return null;
        }
        private IMAPMessageContent GetTextDataFromIMAPMessage(IMAPMessage message)
        {
            foreach (IMAPMessageContent content in message.BodyParts)
            {
                if (content.ContentType.ToLower().Contains("plain"))
                    return content;
            }
            return null;
        }
        private string GetAddressesStringFromAddressList(List<IMAPMailAddress> addresses)
        {
            if (addresses == null || addresses.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var address in addresses)
            {
                sb.Append(string.Format("{0} [{1}], ", address.DisplayName, address.Address));
            }
            return sb.ToString(0, sb.Length - 2).Trim();
        }
    }
}
