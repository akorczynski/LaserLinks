﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DeBouncer
{
    public class Watcher
    {
        private IChatClient _ChatClient;
        private DateTime _LastRead = DateTime.MinValue;
        private static Dictionary<string, DebounceInfo> _DebounceInfo = new Dictionary<string, DebounceInfo>();
        private static object _DebounceLocker = new object();
        private TimeSpan _DebounceTime;
        private int _ErrorRoomId;
        private WatchInfo _WatchInfo;

        public void StartWatch(string authCode, int debounceSeconds, int errorRoomId, WatchInfo watchInfo)
        {
            _DebounceTime = new TimeSpan(0, 0, 0, debounceSeconds);
            _ErrorRoomId = errorRoomId;
            _ChatClient = ChatFactory.GetChatClient(1);
            _ChatClient.Connect(authCode, watchInfo.RoomID, "default");
            _WatchInfo = watchInfo;

            // Setup watch files
            var fsw = new FileSystemWatcher(watchInfo.DirToWatch)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };
            fsw.Changed += SendFileChangeCreateMessage;
            fsw.Created += SendFileChangeCreateMessage;

            fsw.Renamed += (sender, eventArgs) => SendMessage(eventArgs, false);
            fsw.EnableRaisingEvents = true;

            // Setup watch directories
            var dsw = new FileSystemWatcher(watchInfo.DirToWatch)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.DirectoryName 
            };
            dsw.Changed += (sender, eventArgs) => SendMessage(eventArgs, true);
            dsw.Created += (sender, eventArgs) => SendMessage(eventArgs, true);
            dsw.Renamed += (sender, eventArgs) => SendMessage(eventArgs, true);
            dsw.EnableRaisingEvents = true;
        }

        // This handles file change or create events others (rename and dir events) call directly to SendMessage
        private void SendFileChangeCreateMessage(object sender, FileSystemEventArgs fsEvent)
        {
            SendFileChangeCreateMessage(fsEvent);
        }

        private void SendFileChangeCreateMessage(FileSystemEventArgs fsEvent)
        {
            try
            {
                var lastWriteTime = File.GetLastWriteTime(fsEvent.FullPath);
                if (lastWriteTime != _LastRead && !Directory.Exists(fsEvent.FullPath))
                {
                    SendMessage(fsEvent, false);
                    _LastRead = lastWriteTime;
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Do nothing other than logging it
                Logger.LogMessage("Unauthorized access exception: " + fsEvent.FullPath);
            }
            catch (PathTooLongException)
            {
                SendMessage(fsEvent, false);
            }
            catch (Exception e)
            {
                string msg, msg2, msg3, msg4;
                msg = msg2 = msg3 = msg4 = "SendFileChangeCreateMessage CRITICAL";
                try
                {
                    msg = "SendFileChangeCreateMessage Error File: " + fsEvent.FullPath;
                    msg2 = "SendFileChangeCreateMessage Error Message: " + e.Message;
                    msg3 = "SendFileChangeCreateMessage Error Source: " + e.Source;
                    var innerExp = e.InnerException == null ? "NONE" : e.InnerException.Message;
                    msg4 = "SendFileChangeCreateMessage Error Inner Exception: " + innerExp;
                    _ChatClient.SendMessage(msg, _ErrorRoomId);
                    _ChatClient.SendMessage(msg2, _ErrorRoomId);
                    _ChatClient.SendMessage(msg3, _ErrorRoomId);
                    _ChatClient.SendMessage(msg4, _ErrorRoomId);
                }
                finally
                {
                    Logger.LogMessage(msg + "\n" + msg2 + "\n" + msg3 + "\n" + msg4);
                }
            }
        }

        private void SendMessage(FileSystemEventArgs fsEvent, bool isDir)
        {
            try
            {
                lock (_DebounceLocker)
                {
                    CleanDebounce();
                    if (_DebounceInfo.ContainsKey(_WatchInfo.RoomID + fsEvent.FullPath))
                    {
                        Logger.LogMessage("File debounced:  " + fsEvent.FullPath);
                        return;
                    }

                    _DebounceInfo.Add(_WatchInfo.RoomID + fsEvent.FullPath, new DebounceInfo
                    {
                        FileName = fsEvent.FullPath,
                        LastSeen = DateTime.Now
                    });
                }
                // Apply filter to entire path
                string filterCaught = null;
                if (!FilterCheck(fsEvent.FullPath, out filterCaught))
                {
                    Logger.LogMessage("File rejected by filter: " + filterCaught + " filename: " + fsEvent.FullPath);
                    return;
                }
                // Turn change type into nice text
                string changeType;
                switch (fsEvent.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        changeType = "Modified ";
                        break;
                    case WatcherChangeTypes.Created:
                        changeType = "New ";
                        break;
                    case WatcherChangeTypes.Renamed:
                        changeType = "Renamed ";
                        break;
                    default:
                        changeType = "Something went wrong";
                        break;
                }
                // Find owner
                var userName = "";
                try
                {
                    userName =
                        File.GetAccessControl(fsEvent.FullPath)
                            .GetOwner(typeof (System.Security.Principal.NTAccount))
                            .ToString();
                }
                catch (UnauthorizedAccessException)
                {
                    Logger.LogMessage("Rejected because unauthorized exception trying to find owner: " +
                                        fsEvent.FullPath);
                    return;
                }
                catch (Exception e)
                {
                    Logger.LogMessage("Unknown error trying to find owner (file/dir not being rejected) Error: " + e.Message + " Path: " + fsEvent.FullPath);
                }

                // Filter on username
                if (!FilterUserCheck(userName, out filterCaught))
                {
                    Logger.LogMessage("Rejected because user filter.  Filter: " + filterCaught + " Path: " + fsEvent.FullPath);
                    return;
                }
                var msg = @"<a href='file:///" + fsEvent.FullPath.Replace(@"\", "/") + @"'>" + fsEvent.FullPath +
                          "</a> " + (string.IsNullOrEmpty(userName) ? "" : " (" + userName + ")");
                _ChatClient.SendMessage(msg, _WatchInfo.RoomID, changeType + (isDir ? "Directory " : "File "));
                Logger.LogMessage(changeType + (isDir ? "Directory " : "File ") + fsEvent.FullPath +
                                  (string.IsNullOrEmpty(userName) ? "" : " (" + userName + ")"));
            }
            catch (Exception e)
            {
                string msg, msg2, msg3, msg4;
                msg = msg2 = msg3 = msg4 = "SendMessage CRITICAL";
                try
                {
                    msg = "Send Message Error File: " + fsEvent.FullPath;
                    msg2 = "Send Message Error Message: " + e.Message;
                    msg3 = "Send Message Error Source: " + e.Source;
                    var innerExp = e.InnerException == null ? "NONE" : e.InnerException.Message;
                    msg4 = "Send Message Error Inner Exception: " + innerExp;
                    _ChatClient.SendMessage(msg, _ErrorRoomId);
                    _ChatClient.SendMessage(msg2, _ErrorRoomId);
                    _ChatClient.SendMessage(msg3, _ErrorRoomId);
                    _ChatClient.SendMessage(msg4, _ErrorRoomId);
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    Logger.LogMessage(msg + "\n" + msg2 + "\n" + msg3 + "\n" + msg4);
                }
            }
        }

        private void CleanDebounce()
        {
            var toRemove = (from debounce in _DebounceInfo where DateTime.Now > debounce.Value.LastSeen + _DebounceTime 
                            select debounce.Key).ToList();
            foreach (var del in toRemove)
            {
                _DebounceInfo.Remove(del);
            }
        }

        private bool FilterUserCheck(string userName, out string filterCaught)
        {
            filterCaught = null;
            if (String.IsNullOrEmpty(userName))
            {
                return true;
            }
            foreach (var filter in _WatchInfo.ExcludeUserList)
            {
                if (Regex.IsMatch(userName, filter))
                {
                    filterCaught = filter;
                    return false;
                }
            }
            if (!_WatchInfo.IncludeUserList.Any())
            {
                return true;
            }
            if (_WatchInfo.IncludeUserList.Any(filter => Regex.IsMatch(userName, filter)))
            {
                return true;
            }
            filterCaught = "No user filters caught";
            return false;
        }

        private bool FilterCheck(string relativePath, out string filterUsed)
        {
            filterUsed = null;
            foreach (var filter in _WatchInfo.ExcludeFilterList)
            {
                if (Regex.IsMatch(relativePath, filter))
                {
                    filterUsed = filter;
                    return false;
                }
            }
            if (!_WatchInfo.InlucdeFilterList.Any())
            {
                return true;
            }
            if (_WatchInfo.InlucdeFilterList.Any(filter => Regex.IsMatch(relativePath, filter)))
            {
                return true;
            }
            filterUsed = "Caught by neither include or exclude";
            return false;
        }
    }
}
