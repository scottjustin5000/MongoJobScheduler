using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Infrastructure
{
    internal static class SecurityHelper
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string userName, string domainName, string password, int logonType, int logonProvider, ref IntPtr token);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(IntPtr handle);

        [DllImport("credui.dll", EntryPoint = "CredUIParseUserNameA")]
        private static extern int CredUIParseUserName(byte[] userName, byte[] userPart, int maxUserPart, byte[] domainPart, int maxDomainPart);

        /// <summary>
        /// Creates and returns a token by calling the LogonUser API
        /// function.
        /// </summary>
        /// <param name="userName">
        /// The name of the user. This value can contain the domain and 
        /// username in UPN format (username@domain) or standard format 
        /// (domain\username). If no domain is specified, then it is 
        /// assumed the account is a local account for the current machine.
        /// </param>
        /// <param name="password">
        /// The password of the specified user.
        /// </param>
        public static IntPtr CreateToken(string userName, string password)
        {
            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int INTERACTIVE_AUTHENTICATION = 2;

            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            if (userName.Length == 0)
            {
                throw new ArgumentException("Parameter \"userName\" must be specified.");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (password.Length == 0)
            {
                throw new ArgumentException("Parameter \"password\" must be specified.");
            }

            IntPtr hToken = IntPtr.Zero;

            userName = userName.Replace("/", "\\");
            string domain = null;

            if (userName.IndexOf("\\") != -1 || userName.IndexOf("@") != -1)
            {
                string newUserName = null;
                ParseDomainUser(userName, out domain, out newUserName);
                userName = newUserName;
            }

            if (!LogonUser(userName, domain, password, INTERACTIVE_AUTHENTICATION, LOGON32_PROVIDER_DEFAULT, ref hToken))
            {
                hToken = IntPtr.Zero;
            }

            return hToken;
        }

        /// <summary>
        /// Destroys the specified token, logging off the specified user.
        /// </summary>
        public static void DestroyToken(IntPtr token)
        {
            if (token != IntPtr.Zero)
            {
                CloseHandle(token);
            }
        }

        private static void ParseDomainUser(string domainUserName, out string domainName, out string userName)
        {
            const int BUFFER_CAPACITY = 255;

            userName = null;
            domainName = null;

            // CredUIParseUserName does not parse accounts with the @ syntax (userName@domainName).
            int ampIndex = domainUserName.IndexOf("@");

            if (ampIndex != -1)
            {
                userName = domainUserName.Substring(0, ampIndex);
                domainName = domainUserName.Substring(ampIndex + 1, domainUserName.Length - ampIndex - 1);
            }
            else
            {
                byte[] userBytes = new byte[BUFFER_CAPACITY];
                byte[] domainBytes = new byte[BUFFER_CAPACITY];
                byte[] domainUserBytes = new byte[domainUserName.Length + 1];
                Encoding.ASCII.GetBytes(domainUserName, 0, domainUserName.Length, domainUserBytes, 0);

                if (CredUIParseUserName(domainUserBytes, userBytes, BUFFER_CAPACITY, domainBytes, BUFFER_CAPACITY) != 0)
                {
                    // an error was generated, so we will try to parse
                    // the domain name and user name manually.
                    int slashIndex = domainUserName.IndexOf("/");

                    if (slashIndex == -1)
                    {
                        slashIndex = domainUserName.IndexOf("\\");
                    }

                    if (slashIndex != -1)
                    {
                        domainName = domainUserName.Substring(0, slashIndex);
                        userName = domainUserName.Substring(slashIndex + 1, domainUserName.Length - slashIndex - 1);
                    }
                    else
                    {
                        userName = domainUserName;
                        domainName = "";
                    }
                }
                else
                {
                    domainName = Encoding.ASCII.GetString(domainBytes).Replace("\0", "");
                    userName = Encoding.ASCII.GetString(userBytes).Replace("\0", "");
                }
            }
        }
    }
}
