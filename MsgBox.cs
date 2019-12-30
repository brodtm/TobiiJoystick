using System;
using System.Windows.Forms;
namespace Utils
{
    public class MsgBox
    {
        internal static void ShowException(Exception exception)
        {
            string msg = exception.Message;
            Exception exc = exception;
            string fullmessage = "";
            while (exc != null)
            {
                fullmessage = fullmessage + exc.Message;
                exc = exc.InnerException;
                if (exc != null)
                {
                    fullmessage = fullmessage + "\r\n";
                }
            }
            MessageBox.Show(fullmessage);
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="msg"></param>
        public static void ShowException(Exception exc, string msg)
        {
            MessageBox.Show(msg);
            return;
        }
        public static void Show(Exception exc)
        {
            ShowException(exc);
        }
    }
}
