#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2006, Koolwired Solutions, LLC.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 *
 * Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer. 
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution. 
 * Neither the name of Koolwired Solutions, LLC. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission. 
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS
 * AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 * THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
 * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS
 * OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
 * WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *--------------------------------------------------------------------------*/
#endregion

#region History
/*--------------------------------------------------------------------------
 * Modification History: 
 * Date       Programmer        Description
 * 09/16/06   Keith Kikta       Inital release.
 * 09/27/07   Keith Kikta       Modified decoding of body parts to look for '=='
 *                              instead looking for "[tag] OK"
 * 10/18/08   Frans-J King      Add missing Expunge method.
 * 10/18/08   Bradley Llewellyn Added support for Microsoft Outlooks "Welcome to Outlook" message. 
 *--------------------------------------------------------------------------*/
#endregion

#region Refrences
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Koolwired.Imap
{
    #region Header
    /// <summary>
    /// Represents the ImapCommand object.
    /// </summary>
    #endregion
    public class ImapCommand
    {
        #region private variables
        const string patFetchComplete = @"^kw\d+\WOK\W([Ff][Ee][Tt][Cc][Hh]\W|)[Cc][Oo][Mm][Pp][Ll][Ee][Tt][Ee]";
        const string patFetchNotOk = @"^kw\d+\WNO";
        ImapConnect _connection;
        #endregion

        #region protected properties
        /// <summary>
        /// Sets the ImapConnect to use in this instance.
        /// </summary>
        public ImapConnect Connection
        {
            set { _connection = value; }
            private get { return _connection; }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Initalizes an instance of the ImapCommand object.
        /// </summary>
        public ImapCommand() { }
        /// <summary>
        /// Initalizes an instance of the ImapCommand object.
        /// </summary>
        /// <param name="connection">A ImapConnect object representing the connection to use in this instance.</param>
        public ImapCommand(ImapConnect connection)
        {
            this.Connection = connection;
        }
        #endregion

        #region public enumerators
        /// <summary>
        /// Properties that messages can be sorted on.
        /// </summary>
        public enum SortMethod
        {
            /// <summary>
            /// No property.
            /// </summary>
            NONE, 
            /// <summary>
            /// Sort on arrival (received).
            /// </summary>
            ARRIVAL, 
            /// <summary>
            /// Sort on CC addresses.
            /// </summary>
            CC, 
            /// <summary>
            /// Sort on sent date.
            /// </summary>
            DATE, 
            /// <summary>
            /// Sort on the From address.
            /// </summary>
            FROM, 
            /// <summary>
            /// Sort on message size.
            /// </summary>
            SIZE, 
            /// <summary>
            /// Sort on message subject.
            /// </summary>
            SUBJECT
        }
        /// <summary>
        /// Options for indicating the direction of the sort.
        /// </summary>
        public enum SortOrder {
            /// <summary>
            /// Sorts the messages ascending.
            /// </summary>
            ASC,
            /// <summary>
            /// Sorts the messages descending.
            /// </summary>
            DESC
        }
        #endregion

        #region public methods
        /// <summary>
        /// Examines a mailbox.
        /// </summary>
        /// <param name="mailbox">The name of the mailbox to examine.</param>
        /// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
        public ImapMailbox Examine(string mailbox) {
            ImapMailbox Mailbox = null;
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write("EXAMINE \"" + mailbox + "\"\r\n");
            Mailbox = ParseMailbox(mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Selects a mailbox to perform commands on.
        /// </summary>
        /// <param name="mailbox">The name of the mailbox to select.</param>
        /// <returns>Returns a mailbox object containing the properties of the mailbox.</returns>
        public ImapMailbox Select(string mailbox)
        {
            ImapMailbox Mailbox = null;
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write("SELECT \"" + mailbox + "\"\r\n");
            Mailbox = ParseMailbox(mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Obtains a sorted collection of messages from a mailbox.
        /// </summary>
        /// <param name="sort">A value of type SortMethod.</param>
        /// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
        /// <param name="records">An interger value containing the number of messages to return.</param>
        /// <param name="page">An integer value representing the page to display.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Sort(SortMethod sort, SortOrder order, int records, int page)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SORT ({0}{1}) US-ASCII ALL\r\n", OrderToString(order), SortToString(sort)));
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Connection.Read();
                MatchCollection matches = Regex.Matches(response, @"\d+");
                if (matches.Count > 0)
                {
                    int[] ids;
                    if ((page + 1) * records > matches.Count)
                    {
                        page = matches.Count / records;
                        ids = new int[matches.Count % records];
                    }
                    else
                        ids = new int[records];
                    for (int i = page * records; i < matches.Count && i < (page + 1) * records; i++)
                        ids[i - page * records] = Convert.ToInt16(matches[i].Value);
                    return Fetch(ids);
                }
            }
            return new ImapMailbox();

        }
        /// <summary>
        /// Obtains a sorted collection of messages from a mailbox.
        /// </summary>
        /// <param name="sort">A value of type SortMethod.</param>
        /// <param name="order">A value of type SortOrder that specifies ascending or descending.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Sort(SortMethod sort, SortOrder order)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("SORT ({0){1}) US-ASCII ALL\r\n", OrderToString(order), SortToString(sort)));
            string response = Connection.Read();
            if (response.StartsWith("*")) {
                Connection.Read();
                MatchCollection matches = Regex.Matches(response, @"\d+");
                if (matches.Count > 0) {
                    int[] ids = new int[matches.Count];
                    for (int i = 0; i < matches.Count; i++)
                        ids[i] = Convert.ToInt16(matches[i].Value);
                    return Fetch(ids);
                }
            }
            return new ImapMailbox();
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="begin">The first message to retreive.</param>
        /// <param name="end">The last message to retreive.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(int begin, int end)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, begin, end);
        }
        /// <summary>
        /// Obtains messages from a mailbox.
        /// </summary>
        /// <param name="messages">A interger array of message ids.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(int[] messages)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, messages);
        }
        /// <summary>
        /// Retreives message headers for a message.
        /// </summary>
        /// <param name="message">A integer representing the message id.</param>
        /// <returns>Returns a ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchHeaders(int message)
        {
            ImapMailbox Mailbox = new ImapMailbox();
            return Fetch(Mailbox, message, message).Messages[0];
        }
        /// <summary>
        /// Retreives the bodystructure of a message.
        /// </summary>
        /// <param name="message">A ImapMailboxMessage object.</param>
        /// <returns>Returns an ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchBodyStructure(ImapMailboxMessage message)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0} BODYSTRUCTURE\r\n", message.ID));
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                response = response.Substring(response.IndexOf(" (", response.IndexOf("BODYSTRUCTURE")));
                message.Errors = response;
                message.BodyParts = BodyPartSplit(response.Trim().Substring(0, response.Trim().Length - 1));
                response = Connection.Read();
                for (int i = 0; i < message.BodyParts.Count && i < 2; i++)
                {
                    if (message.BodyParts[i].ContentType.MediaType.ToLower() == "text/html")
                    {
                        message.HasHTML = true;
                        message.HTML = i;
                    }
                    else if (message.BodyParts[i].ContentType.MediaType.ToLower() == "text/plain")
                    {
                        message.HasHTML = true;
                        message.Text = i;
                    }
                }
                return message;
            }
            else
                throw new ImapCommandInvalidMessageNumber("No UID found for message number" + message.ID);
        }
        /// <summary>
        /// Retreives the content of a particular body part.
        /// </summary>
        /// <param name="message">A ImapMailboxMessage object.</param>
        /// <param name="part">A numeric value representing the body part in the message.</param>
        /// <returns>Returns an ImapMailboxMessage object.</returns>
        public ImapMailboxMessage FetchBodyPart(ImapMailboxMessage message, int part)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0} BODY[{1}]\r\n", message.ID, message.BodyParts[part].BodyPart));
            string response = Connection.Read();
            if (response.StartsWith("*"))
                message.BodyParts[part].Data = ParseBodyPart(message.BodyParts[part].ContentEncoding, message.BodyParts[part].Encoding);
            return message;
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <param name="begin">The first message to retreive.</param>
        /// <param name="end">The last message to retreive.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox, int begin, int end)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH {0}:{1} ALL\r\n", begin, end));
            ParseMessages(ref Mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Obtains messages from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <param name="messages">A interger array of message ids.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox, int[] messages)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            string messagelist = string.Empty;
            for (int i = 0; i < messages.Length; i++)
                messagelist += (i == 0) ? messages[i].ToString() : "," + messages[i];
            Connection.Write(string.Format("FETCH {0} ALL\r\n", messagelist));
            ParseMessages(ref Mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Obtains message from a mailbox.
        /// </summary>
        /// <param name="Mailbox">The ImapMailbox object to add the messages to.</param>
        /// <returns>Returns a ImapMailbox object containing the messages.</returns>
        public ImapMailbox Fetch(ImapMailbox Mailbox)
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write(string.Format("FETCH 1:* ALL\r\n"));
            ParseMessages(ref Mailbox);
            return Mailbox;
        }
        /// <summary>
        /// Converts a message number to the server message UID.
        /// </summary>
        /// <param name="messageNumber">The message number to convert.</param>
        /// <returns>Returns the server UID of the message.</returns>
        public int FetchUID(int messageNumber)
        {
            Connection.Write(string.Format("FETCH {0} UID\r\n", messageNumber));
            string response = Connection.Read();
            int uid = 0;
            if (response.StartsWith("*"))
            {
                Match match = Regex.Match(response, @"\(UID (\d+)\)");
                uid = Convert.ToInt32(match.Groups[1].ToString());
                Connection.Read();
                return uid;
            }
            else
                throw new ImapCommandInvalidMessageNumber("No UID found for message number" + messageNumber);
        }
        /// <summary>
        /// Expunges a mailbox
        /// </summary>
        public void Expunge()
        {
            if (!(Connection.ConnectionState == ConnectionState.Open))
                NoOpenConnection();
            Connection.Write("EXPUNGE\r\n");

            string response;
            do
            {
                response = Connection.Read();
            } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete) || Regex.IsMatch(response, patFetchNotOk))); 
        }
        /// <summary>
        /// Sets the seen flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetSeen(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Seen", value);
        }
        /// <summary>
        /// Sets the answered flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetAnswered(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Answered", value);
        }
        /// <summary>
        /// Sets the flagged flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetFlagged(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Flagged", value);
        }
        /// <summary>
        /// Sets the deleted flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetDeleted(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Deleted", value);
        }
        /// <summary>
        /// Sets the draft flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetDraft(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Draft", value);
        }
        /// <summary>
        /// Sets the recent flag of a message.
        /// </summary>
        /// <param name="messageNumber">The position of the message on the server.</param>
        /// <param name="value">A boolean value to set or unset the flag.</param>
        /// <returns>Returns true if the command succeded.</returns>
        public bool SetRecent(int messageNumber, bool value)
        {
            return SetFlag(FetchUID(messageNumber), @"\Recent", value);
        }
        #endregion

        #region private methods

        private bool SetFlag(int uid, string flag, bool append)
        {
            string method = null;
            if (append)
                method = "+flags";
            else
                method = "-flags";
            Connection.Write(string.Format("UID STORE {0} {1} ({2})\r\n", uid.ToString(), method, flag));
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Connection.Read();
                return true;
            }
            else
                return false;
        }

        private void ParseMessages(ref ImapMailbox Mailbox)
        {
            string response = string.Empty;
            if (Mailbox.Messages == null)
                Mailbox.Messages = new List<ImapMailboxMessage>();
            do
            {
                response += Connection.Read();
            } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete) || Regex.IsMatch(response, patFetchNotOk)));
            if (response.StartsWith("*"))
            {
                do
                {
                    ImapMailboxMessage Message = new ImapMailboxMessage();
                    Message.Flags = new ImapMessageFlags();
                    Message.Addresses = new ImapAddressCollection();
                    Match match;
                    if ((match = Regex.Match(response, @"\* (\d*)")).Success)
                        Message.ID = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"\(FLAGS \(([^\)]*)\)")).Success)
                        Message.Flags.ParseFlags(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"INTERNALDATE ""([^""]+)""")).Success)
                        Message.Received = DateTime.Parse(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"RFC822.SIZE (\d+)")).Success)
                        Message.Size = Convert.ToInt32(match.Groups[1].ToString());
                    if ((match = Regex.Match(response, @"ENVELOPE")).Success)
                        response = response.Remove(0, match.Index + match.Length);
                    if ((match = Regex.Match(response, @"\(""(?:\w{3}\, )?([^""]+)""")).Success)
                    {
                        Match subMatch;
                        subMatch = Regex.Match(match.Groups[1].ToString(), @"([\-\+]\d{4}.*|NIL.*)"); //(-\d{4}|-\d{4}[^""]+|NIL)
                        DateTime d;
                        DateTime.TryParse(match.Groups[1].ToString().Remove(subMatch.Index), out d);
                        Message.Sent = d;
                        Message.TimeZone = subMatch.Groups[1].ToString();
                        response = response.Remove(0, match.Index + match.Length);
                    }
                    string TOKEN;
                    int TOKEN_OFFSET = 0;
                    if (response.Contains("(("))
                        TOKEN = "((";
                    else
                    {
                        TOKEN = "\" NIL";
                        TOKEN_OFFSET = 2;
                    }
                    Message.Subject = response.Substring(0, response.IndexOf(TOKEN) + TOKEN_OFFSET).Trim();
                    if (Message.Subject == "NIL")
                        Message.Subject = null;
                    else if ((match = Regex.Match(Message.Subject, "^\"(.*)\"$")).Success)
                        Message.Subject = match.Groups[1].ToString();
                    Message.Subject = ImapDecode.Decode(Message.Subject);
                    response = response.Remove(0, response.Substring(0, response.IndexOf("((")).Length);

                    //if ((match = Regex.Match(response, @"(""[^""]*"" \(\(|NIL)")).Success)
                    //{
                    //    Message.Subject = match.Groups[1].ToString();
                    //    if (Message.Subject == "NIL")
                    //        Message.Subject = null;
                    //    else if (Message.Subject.StartsWith("\""))
                    //        Message.Subject = Message.Subject.Substring(1, Message.Subject.Length -5);
                    //    response = response.Remove(0, match.Index + match.Length - 3);
                    //}
                    if ((match = Regex.Match(response, @"""<([^>]+)>""\)\)")).Success)
                    {
                        Message.MessageID = match.Groups[1].ToString();
                        response = response.Remove(match.Index).Trim();
                    }
                    if (response.EndsWith("NIL"))
                        response = response.Remove(response.Length - 3);
                    else {
                        match = Regex.Match(response, @"""<([^>]+)>""");
                        Message.Reference = match.Groups[1].ToString();
                    }
                    try
                    {
                        Message.Addresses = Message.Addresses.ParseAddresses(response);
                    }
                    catch (Exception ex)
                    {
                        Message.Errors = response + ex.ToString();
                    }
                    Mailbox.Messages.Add(Message);
                    response = string.Empty;
                    do
                    {
                        response += Connection.Read();
                    } while (!(response.EndsWith("))") || Regex.IsMatch(response, patFetchComplete) || Regex.IsMatch(response, patFetchNotOk)));
                } while (response.StartsWith("*"));

                //match = Regex.Match(response, @"\(FLAGS \(([\w\\]+)\) INTERNALDATE ""([^""]+)"" RFC822\.SIZE (\d+) ENVELOPE \(""([^""]+)"" ""([^""]+)"" \(\(NIL NIL ""([^""]+""\)\)");
            }
        }

        private string ParseBodyPart(Imap.BodyPartEncoding encoding)
        {
            return ParseBodyPart(encoding, null);
        }

        private string ParseBodyPart(Imap.BodyPartEncoding encoding, Encoding en)
        {
            string response;
            StringBuilder sb = new StringBuilder("");
            do
            {
                response = Connection.Read();
                if (Regex.IsMatch(response, patFetchComplete))
                    break;
                if (encoding == Imap.BodyPartEncoding.BASE64)
                    sb.Append(response);
                else if (encoding == Imap.BodyPartEncoding.QUOTEDPRINTABLE)
                    if (response.EndsWith("=") || response.EndsWith(")"))
                        sb.Append(response.Substring(0, response.Length - 1));
                    else
                        sb.AppendLine(response);
                else
                    sb.AppendLine(response);
            } while (true);
            //} while (!(response.EndsWith("==") || response == ")"));
            if (sb.ToString().Trim().EndsWith(")"))
                sb = sb.Remove(sb.ToString().LastIndexOf(")"), 1);
            if (encoding != BodyPartEncoding.BASE64)
                return ImapDecode.Decode(sb.ToString(), en);
            return sb.ToString();
        }

        private ImapMailbox ParseMailbox(string mailbox)
        {
            ImapMailbox Mailbox = null;
            string response = Connection.Read();
            if (response.StartsWith("*"))
            {
                Mailbox = new ImapMailbox(mailbox);
                Mailbox.Flags = new ImapMessageFlags();
                do
                {
                    Match match;
                    if ((match = Regex.Match(response, @"(\d+) EXISTS")).Success)
                        Mailbox.Exist = Convert.ToInt32(match.Groups[1].ToString());
                    else if ((match = Regex.Match(response, @"(\d+) RECENT")).Success)
                        Mailbox.Recent = Convert.ToInt32(match.Groups[1].ToString());
                    else if ((match = Regex.Match(response, @" FLAGS \((.*?)\)")).Success)
                        Mailbox.Flags.ParseFlags(match.Groups[1].ToString());
                    response = Connection.Read();
                } while (response.StartsWith("*"));
                if ((response.StartsWith("OK") || response.Substring(7, 2) == "OK") && (response.ToUpper().Contains("READ/WRITE") || response.ToUpper().Contains("READ-WRITE")))
                    Mailbox.ReadWrite = true;
            }
            return Mailbox;
        }

        private ImapMessageBodyPartList BodyPartSplit(string response)
        {
            ImapMessageBodyPartList Parts = new ImapMessageBodyPartList();
            int i = 0;
            int index = 1;
            int count = 0;
            do
            {
                int next = index;
                do
                {
                    if (response[next] == '(')
                        i++;
                    else if (response[next] == ')')
                        i--;
                    next++;
                } while (i > 0 || response[next - 1] != ')');
                if (i >= 0 && response[index] == '(')
                {
                    count++;
                    // Parse nested body parts
                    if (response.Substring(index, next - index).StartsWith("(("))
                    {
                        ImapMessageBodyPartList temp = BodyPartSplit(response.Substring(index, next));
                        for (int j = 0; j < temp.Count; j++)
                        {
                            temp[j].BodyPart = count.ToString() + "." + temp[j].BodyPart;
                            Parts.Add(temp[j]);
                        }
                    }
                    else
                    {
                        ImapMessageBodyPart Part = new ImapMessageBodyPart(response.Substring(index, next - index));
                        Part.BodyPart = count.ToString();
                        Parts.Add(Part);
                    }
                }
                else if(Parts.Count == 0)
                {
                    ImapMessageBodyPart Part = new ImapMessageBodyPart(response);
                    Part.BodyPart = "1";
                    Parts.Add(Part);
                }
                index = next;
            } while (i >= 0);
            return Parts;
        }

        private string SortToString(SortMethod sort)
        {
            switch (sort)
            {
                case SortMethod.ARRIVAL: return "ARRIVAL";
                case SortMethod.CC: return "CC";
                case SortMethod.DATE: return "DATE";
                case SortMethod.FROM: return "FROM";
                case SortMethod.SIZE: return "SIZE";
                case SortMethod.SUBJECT: return "SUBJECT";
                default: return string.Empty;
            }
        }

        private string OrderToString(SortOrder order)
        {
            if (order == SortOrder.DESC)
                return "REVERSE ";
            return string.Empty;
        }

        private void NoOpenConnection()
        {
            throw new ImapConnectionException("Connection must be open before commands can be performed.");
        }
        #endregion
    }
}