using System;//
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeBouncer;

namespace Tests
{
    [TestClass]
    public class MainFileTest
    {
        private string _FolderTest = @"D:\TestShare";
        private string _FolderAccess = @"\OK\";
        private string _FolderNoAccess = @"\NotOK\";

        [TestMethod]
        public void TestFile()
        {
            // With access
            var fileName = _FolderTest + _FolderAccess + "Fc.txt";
            // Create and modify
            using (var newFile = File.CreateText(fileName))
            {
                newFile.WriteLine("test");
                newFile.Flush();
            }
            // Rename
            var newFileName = _FolderTest + _FolderAccess + "Fc2.txt";
            File.Move(fileName, newFileName);
            // Delete
            File.Delete(newFileName);

            // With no access
            fileName = _FolderTest + _FolderNoAccess + "Fc.txt";
            // Create and modify
            using (var newFile = File.CreateText(fileName))
            {
                newFile.WriteLine("test");
                newFile.Flush();
            }
            // Rename
            newFileName = _FolderTest + _FolderNoAccess + "Fc2.txt";
            File.Move(fileName, newFileName);
            // Delete
            File.Delete(newFileName);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestDirectory()
        {
            // With access
            var folderName = _FolderTest + _FolderAccess + @"\Fc";
            // Create 
            var aDir = Directory.CreateDirectory(folderName);         
            // Rename
            Directory.Move(aDir.FullName, folderName + "2");
            // Delete
            Directory.Delete(folderName + "2");

            // With no access
            folderName = _FolderTest + _FolderNoAccess + @"\Fc";
            // Create 
            // Create 
            aDir = Directory.CreateDirectory(folderName);         
            // Rename
            Directory.Move(aDir.FullName, folderName + "2");
            // Delete
            Directory.Delete(folderName + "2");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestFileChangeException()
        {
            Logger.DisableLogging = true;
            var fsEvent = new FileSystemEventArgs(WatcherChangeTypes.Changed, ":", ":");
            var watcher = new Watcher();
            // TO DO  - need to start the watcher
            int errorRoomId;
            string authToken;
            string dummy;
            int debounceTime;
            string logFilePath;
            using (var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\Config.txt"))
            {
                authToken = sr.ReadLine();
                logFilePath = sr.ReadLine();  // log file path
                debounceTime = int.Parse(sr.ReadLine()); // debounceTime
                errorRoomId = int.Parse(sr.ReadLine());
            }
            var watchInfo = new WatchInfo { DirToWatch = @"c:\temp" };
            watcher.StartWatch(authToken, debounceTime, errorRoomId, watchInfo); // can leave watchinfo blank since testing error only
            PrivateObject obj = new PrivateObject(watcher);
            var retVal = obj.Invoke("SendFileChangeCreateMessage", fsEvent);
            Assert.IsTrue(true);
        }
    }
}
