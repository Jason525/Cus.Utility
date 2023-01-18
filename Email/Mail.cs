using System;

namespace Utility.Email
{
    [Serializable]
    public class Mail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public string Attach { get; set; }
        public string ReplyTo { get; set; }

        public SendingResult SendBy(Smtp server)
        {
            return Utils.SendWithCredentials(this, server);
        }

        public SendingResult Send()
        {
            return Utils.SendWithCredentials(this);
        }
    }
}
