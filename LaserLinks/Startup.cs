using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace LaserLinks
{
    public class Startup : Application
    {
        private const int BUFFER_SIZE = 2000;

        [STAThreadAttribute()]
        [DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            Stream inputStream = Console.OpenStandardInput();
            byte[] bytes = new byte[BUFFER_SIZE]; //  If it's longer than this we probably have a problem
            int outputLength = inputStream.Read(bytes, 0, BUFFER_SIZE);

            // Skip the first 4 bytes b/c hey just hold size which we already know
            char[] chars = Encoding.UTF7.GetChars(bytes, 4, outputLength - 4);
            var incomingStr = new string(chars);
            LaserLinksProcessor.ProcessFile(incomingStr, ShowMessage, ShowFileOrDir);
            //ShowWindow();
        }

        private static void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private static void ShowFileOrDir(string fileOrDir)
        {
            Process.Start(fileOrDir);
        }

        #region WPF Window
        private static void ShowWindow()
        {
            Startup app = new Startup();
            app.InitializeComponent();
            app.Run();            
        }

        [DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            // This is for potential later use when we want to hava a full blow window
            StartupUri = new Uri("MainWindow.xaml", System.UriKind.Relative);
        }
        #endregion
    }
}
