#region Copyright (c) Koolwired Solutions, LLC.
/*--------------------------------------------------------------------------
 * Copyright (c) 2006-2007, Koolwired Solutions, LLC.
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
 * Date       Programmer      Description
 * 12/27/2007 Keith Kikta     Inital release. Decoding created by who ever created
 *                            GITSmail. Contact me if this is yours so I can give
 *                            you credit.
 *--------------------------------------------------------------------------*/
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Koolwired.Imap
{
    internal class ImapDecode
    {
        /// <summary>
        /// Decodes from ASCII representations of characters in the given encoding
        /// </summary>
        /// <param name="input">The input, can be many lines long, but only in one encoding.</param>
        /// <param name="enc">The encoding the input is coming from</param>
        /// <returns>The decoded string, suitable for .Net use</returns>
        internal static string Decode(string input, Encoding enc)
        {
            if (input == "" || input == null)
                return "";
            string decoded;
            byte[] bytes;
            MatchCollection matches = Regex.Matches(input, @"\=(?<num>[0-9A-Fa-f]{2})");// Substring(input.IndexOf('=') + 1, 2);
            foreach (Match match in matches) //while (input.Contains("="))
            {
                //string ttr = Regex.Match("input", @"=(?<num>[0-9A-Fa-f]{2})").Groups[num].Substring(input.IndexOf('=') + 1, 2);
                //int i = int.Parse(ttr, System.Globalization.NumberStyles.HexNumber);
                int i = int.Parse(match.Groups["num"].Value, System.Globalization.NumberStyles.HexNumber);
                char str = (char)i;
                input = input.Replace(match.Groups[0].Value, str.ToString());
            }
            bytes = System.Text.Encoding.Default.GetBytes(input);
            decoded = enc.GetString(bytes);
            return decoded;
        }

        /// <summary>
        /// Decodes a string to it's native encoding 
        /// Pulls from the string correcting multiple =?ENC?METHOD?TEXT?=
        /// </summary>
        /// <param name="input">The string with embedded encoding(s)</param>
        /// <returns>The decoded string, suitable for .Net use</returns>
        internal static string Decode(string input)
        {
            if (input == "" || input == null)
                return "";
            Regex regex = new Regex(@"=\?(?<Encoding>[^\?]+)\?(?<Method>[^\?]+)\?(?<Text>[^\?]+)\?=");
            MatchCollection matches = regex.Matches(input);
            string ret = input;
            foreach (Match match in matches)
            {
                string encoding = match.Groups["Encoding"].Value;
                string method = match.Groups["Method"].Value;
                string text = match.Groups["Text"].Value;
                string decoded;
                if (method == "B")
                {
                    byte[] bytes = Convert.FromBase64String(text);
                    Encoding enc = Encoding.GetEncoding(encoding);
                    decoded = enc.GetString(bytes);
                }
                else
                    decoded = Decode(text, Encoding.GetEncoding(encoding));
                ret = ret.Replace(match.Groups[0].Value, decoded);
            }
            return ret;
        }
    }
}