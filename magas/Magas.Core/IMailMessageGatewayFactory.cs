using System;
using System.Collections.Generic;
using System.Text;
using Magas.Core;

namespace Magas.Core
{
    public interface IMailMessageGatewayFactory
    {
        IMailMessageGateway GetMailMessageGateway(MailboxAccount mailboxAccount);
    }
}
