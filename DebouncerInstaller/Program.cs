using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace HipChatDebouncerInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            const string userKey = @"HKEY_CURRENT_USER\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.hipchat.linkshelper";
            Registry.SetValue(userKey, null, @"c:\\ChromeExtensions\\HipChatDebouncer\\com.hipchat.linkshelper.json");
            string installDir = @"c:\ChromeExtensions\HipChatDebouncer\";
            if (!Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
            }
            File.WriteAllBytes(installDir + "com.hipchat.linkshelper.json", Properties.Resources.com_hipchat_linkshelper);
            File.WriteAllBytes(installDir + "HipChatLinksHelper.exe", Properties.Resources.HipChatLinksHelper);
        }
    }
}
