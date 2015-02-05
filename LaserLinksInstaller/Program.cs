using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;

namespace LaserLinksInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            const string userKey = @"HKEY_CURRENT_USER\SOFTWARE\Google\Chrome\NativeMessagingHosts\com.hipchat.laserlinks";
            Registry.SetValue(userKey, null, @"c:\\ChromeExtensions\\LaserLinks\\com.hipchat.laserlinks.json");
            string installDir = @"c:\ChromeExtensions\LaserLinks\";
            if (!Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
            }
            File.WriteAllBytes(installDir + "com.hipchat.laserlinks.json", Properties.Resources.com_hipchat_linkshelper);
            File.WriteAllBytes(installDir + "LaserLinks.exe", Properties.Resources.LaserLinks);
        }
    }
}
