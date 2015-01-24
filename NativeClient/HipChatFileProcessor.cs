using System;
using System.IO;
using System.Text;

namespace HipChatNativeClient
{
    public class HipChatFileProcessor
    {
        /*  Just same sample input json
            akorczynski 12/14/2014 6:06:46 PM {"file":"c:\\Temp2\\New Text Document.txt","dir":"c:\\Temp2","cmd":"openfile"}
            akorczynski 12/20/2014 3:26:21 PM {"file":"c:\\Temp2\\New Text Document.txt","dir":"c:\\Temp2","cmd":"opendir"}
        */

        private const string OPENFILE_COMMAND = "openfile";
        private const string OPENDIR_COMMAND = "opendir";

        private static Action<string> _ShowMessage;
        private static Action<string> _OpenFileOrDir;

        public static bool ProcessFile(string incomingStr, Action<string> showMessage, Action<string> openFileOrDir)
        {
            _ShowMessage = showMessage;
            _OpenFileOrDir = openFileOrDir;

            // log incoming command
            var sb = new StringBuilder(Environment.UserName);
            sb.Append(" ");
            sb.Append(incomingStr);
            LogMessage(sb.ToString());

            // break down message
            var incomingData = incomingStr.Split('"');
            if (incomingData.Length != 13)
            {
                var msg = "Invalid parameters passed from Chrome: " + incomingStr;
                LogMessage(msg);
                showMessage(msg);
                return false;
            }
            if (incomingData[11].Contains(OPENFILE_COMMAND))
            {
                return OpenFile(incomingData[3].Replace(@"\\", @"\"));
            }
            if (incomingData[11].Contains(OPENDIR_COMMAND))
            {
                return OpenDir(incomingData[7].Replace(@"\\", @"\"));
            }
            var err = "Invalid command passed from Chrome: " + incomingStr;
            LogMessage(err);
            showMessage(err); ;
            return false;
        }

        public static void LogMessage(string message)
        {
            var logLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\HipChatClient\";
            if (!Directory.Exists(logLocation))
            {
                Directory.CreateDirectory(logLocation);
            }
            using (var sw = new StreamWriter(logLocation + @"\HipChatClientLog.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("MM/dd/yy hh:mm:ss.fff") + " " + message);
            }
        }

        private static bool OpenFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var msg = "Cannot find file: " + fileName;
                LogMessage(msg);
                _ShowMessage(msg);
                return false;
            }
            try
            {
                _OpenFileOrDir(fileName);
                return true;
            }
            catch (Exception e)
            {
                var msg = "Error opening file " + fileName + ": " + e.Message;
                LogMessage(msg);
                _ShowMessage(msg);
                return false;
            }
        }

        private static bool OpenDir(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                var msg = "Cannot find file: " + dirName;
                LogMessage(msg);
                _ShowMessage(msg);
                return false;
            }
            try
            {
                _OpenFileOrDir(dirName);
                return true;
            }
            catch (Exception e)
            {
                var msg = "Error opening directgory " + dirName + ": " + e.Message;
                LogMessage(msg);
                _ShowMessage(msg);
                return false;
            }
        }
    }
}
