using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

using Kwwika;
using Kwwika.Logging;

namespace KwwikaChat
{
    #region Logger
    internal class Logger : ILogger
    {
        private TextBox _textbox;

        public Logger(TextBox textbox)
        {
            _textbox = textbox;
        }

        #region ILogger Members

        public void Log(LogLevels level, string[] categories, string message)
        {
            string msg = level.ToString() + " " + string.Join(",", categories) + " " + message;
            Debug.WriteLine(msg);

            _textbox.Dispatcher.BeginInvoke(new Action(()=>
            {
                string text = _textbox.Text;
                // restrict to 2000 characters to ensure the UI doesn't get filled with
                // logging information
                if (text.Length > 2000)
                {
                    text = text.Substring(text.Length - 1000);
                }
                _textbox.Text = text + message + Environment.NewLine;
                _textbox.SelectionStart = _textbox.Text.Length;
            }),null);
        }

        public void Log(LogLevels level, string category, string message)
        {
            Log(level, new string[] { category }, message);
        }

        public void Log(LogLevels level, string category, string message, params object[] messageParameters)
        {
            Log(level, new string[] { category }, string.Format(message, messageParameters));
        }

        public void Log(LogLevels level, string[] categories, string message, params object[] messageParameters)
        {
            Log(level, categories, string.Format(message, messageParameters));
        }

        #endregion
    }
    #endregion
}
