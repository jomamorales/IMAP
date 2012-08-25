using System;
using System.Collections.Generic;
using System.Text;

namespace Magas.Core
{
    public enum MailMessageFormat
    {
        None = 0, Text = 1, Html = 2
    }

    public class MailMessage
    {
        public string Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public string FolderPath { get; set; }
        public string ContentType { get; set; }
        public MailMessageFormat Format { get; set; }

        public string BodyPreview
        {
            get
            {
                if (string.IsNullOrEmpty(Body))
                {
                    return string.Empty;
                }
                return Body.Substring(0, 20);
            }
        }
    }
}
