using System;
using System.Diagnostics;
using System.IO;
using LaserLinks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaserLinksTest
{
    /*  Just same sample input json
        akorczynski 12/14/2014 6:06:46 PM {"file":"c:\\Temp2\\New Text Document.txt","dir":"c:\\Temp2","cmd":"openfile"}
        akorczynski 12/20/2014 3:26:21 PM {"file":"c:\\Temp2\\New Text Document.txt","dir":"c:\\Temp2","cmd":"opendir"}
    */
    [TestClass]
    public class MainTests
    {
        private const string LOCAL_FILENAME = @"c:\\temp2\\New Text Document.txt";
        private const string LOCAL_SPECIAL_FILENAME = @"c:\\temp2\Wow^&'@{}[],$=!-#()%.+~_.txt";
        private const string LOCAL_DIRNAME = @"c:\\temp2\newdir";
        private const string NETWORK_FILENAME = @"\\\\rcrfile\\Command_Center\\ProjectorController\\ProjectorControl.exe.config";

        [TestMethod]
        public void LogTest()
        {
            LaserLinksProcessor.LogMessage("Test");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GeneralFileTest()
        {
            CreateLocalFile(LOCAL_FILENAME);
            bool result = LaserLinksProcessor.ProcessFile(@"{""file"":""" + LOCAL_FILENAME + @""",""dir"":""c:\\Temp2"",""cmd"":""openfile""}", SetShowMessageAction(), SetShowFileOrDir());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GeneralDirTest()
        {
            CreateLocalDir(LOCAL_DIRNAME);
            bool result = LaserLinksProcessor.ProcessFile(@"{""file"":""" + LOCAL_DIRNAME + @""",""dir"":""c:\\temp2"",""cmd"":""opendir""}", SetShowMessageAction(), SetShowFileOrDir());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FileLengthTest()
        {
        }

        [TestMethod]
        public void FileSpecialCharacterTest()
        {
            CreateLocalFile(LOCAL_SPECIAL_FILENAME);
            bool result = LaserLinksProcessor.ProcessFile(@"{""file"":""" + LOCAL_SPECIAL_FILENAME + @""",""dir"":""c:\\Temp2"",""cmd"":""openfile""}", SetShowMessageAction(), SetShowFileOrDir());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NetworkFileTest()
        {
            // Just put in a url based file that can be anywhere..more for testing time out than anything
            bool result = LaserLinksProcessor.ProcessFile(@"{""file"":""" + NETWORK_FILENAME + @""",""dir"":""\\unknownDrive"",""cmd"":""openfile""}", SetShowMessageAction(), SetShowFileOrDir());
            Assert.IsTrue(result);
        }

        private void CreateLocalFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                using (var sw = new StreamWriter(fileName))
                {
                    sw.WriteLine("Test");
                }
            }
        }

        private void CreateLocalDir(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }

        private Action<string> SetShowMessageAction()
        {
            return new Action<string>(ShowMessage);
        }

        private void ShowMessage(string message)
        {
            // Just consume it for now, only looking at return values
        }

        private Action<string> SetShowFileOrDir()
        {
            return new Action<string>(ShowFileOrDir);
        }

        private void ShowFileOrDir(string fileOrDir)
        {
            var proc = Process.Start(fileOrDir);
            proc.Kill();
        }
    }
}
