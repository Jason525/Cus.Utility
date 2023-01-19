using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace Utility.Email
{
    #region Sending Result
    public class SendingResult
    {
        public bool IsSent
        {
            get
            {
                return First.Successful || (TryAgain != null && TryAgain.Successful);
            }
        }

        public string ErrorMessage
        {
            get
            {
                var msg = "";

                if (!IsSent)
                {
                    msg = TryAgain != null ? TryAgain.Error.Message : First.Error.Message;
                }

                return msg; 
            }
        }

        public SingleResult First { get; set; }
        public SingleResult TryAgain { get; set; }
    }
    #endregion

    #region Single Result
    public class SingleResult
    {
        public bool Successful { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public SmtpException Error { get; set; }

        public SingleResult BeginSend()
        {
            BeginTime = DateTime.Now;

            return this;
        }

        public void EndSend(SmtpException ex = null)
        {
            EndTime = DateTime.Now;
            Error = ex;
            Successful = ex == null;
        }
    }
    #endregion 
}
