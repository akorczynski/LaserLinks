using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.ServiceProcess;

namespace DeBouncer
{
    public class WatchDirector : ServiceBase
    {
        private List<Watcher> _WatcherList = new List<Watcher>();

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public void Start()
        {
            int debounceSeconds;
            int errorRoomId;
            string authToken;
            string logFilePath;
            var dirsToWatch = new List<WatchInfo>();
            using (var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Config.txt"))
            {
                authToken = sr.ReadLine();
                logFilePath = sr.ReadLine();
                debounceSeconds = int.Parse(sr.ReadLine());
                errorRoomId = int.Parse(sr.ReadLine());
                var includeUser = new List<string>();
                var excludeUser = new List<string>();
                while (!sr.EndOfStream)
                {
                    var dataIn = sr.ReadLine();
                    if (dataIn.ToUpper().StartsWith("USERNAME:"))
                    {
                        var arrIn = dataIn.Substring(9).Split(',');
                        foreach (var uFilter in arrIn)
                        {
                            if (string.IsNullOrEmpty(uFilter)) continue;
                            if (uFilter.StartsWith("!"))
                            {
                                excludeUser.Add(uFilter.Substring(1));
                                RegexCheck(uFilter.Substring(1));
                            }
                            else
                            {
                                includeUser.Add(uFilter);
                                RegexCheck(uFilter);
                            }
                        }
                    }
                    else
                    {
                        var arrIn = dataIn.Split(',');
                        var watchInfo = new WatchInfo()
                        {
                            DirToWatch = arrIn[0],
                            RoomID = int.Parse(arrIn[1]),
                        };
                        for (var index = 2; index < arrIn.Length; index++)
                        {
                            if (string.IsNullOrEmpty(arrIn[index])) continue;
                            if (arrIn[index].StartsWith("!"))
                            {
                                watchInfo.ExcludeFilterList.Add(arrIn[index].Substring(1));
                                RegexCheck(arrIn[index].Substring(1));
                            }
                            else
                            {
                                watchInfo.InlucdeFilterList.Add(arrIn[index]);
                                RegexCheck(arrIn[index]);
                            }
                        }
                        // Add inclusion filter if none present
                        if (!watchInfo.InlucdeFilterList.Any())
                        {
                            watchInfo.InlucdeFilterList.Add(".*");
                        }
                        watchInfo.IncludeUserList = includeUser;
                        watchInfo.ExcludeUserList = excludeUser;
                        dirsToWatch.Add(watchInfo);
                    }
                }
            }
            // Make a list so in future you can access them
            Logger.LogFileName = logFilePath;
            Logger.LogMessage("Starting");
            foreach (var dirToWatch in dirsToWatch)
            {
                var watcher = new Watcher();
                watcher.StartWatch(authToken, debounceSeconds, errorRoomId, dirToWatch);
                _WatcherList.Add(watcher);
            }
            Logger.LogMessage("Started");
        }

        private void RegexCheck(string filter)
        {
            try
            {
                Regex.IsMatch("testPattern", filter);
            }
            catch (Exception e)
            {
                Logger.LogMessage("Invalid Regex: "  + filter + "  Error: " + e.Message);
            }
        }
    }
}
