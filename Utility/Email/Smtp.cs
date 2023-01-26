using System;

namespace Utility.Email
{
    [Serializable]
    public class Smtp
    {
        public string ServerName { get; set; }
        public string RelayServer { get; set; }
        public int RelayPort { get; set; }
        public bool UseSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }

        public SendingResult Send(Mail mail)
        {
            return Utils.SendWithCredentials(mail, this);
        }
    }
}
