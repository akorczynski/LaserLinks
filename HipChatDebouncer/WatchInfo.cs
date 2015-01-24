using System.Collections.Generic;

namespace DeBouncer
{
    public class WatchInfo
    {
        public string DirToWatch { get; set; }
        public int RoomID { get; set; }
        public List<string> InlucdeFilterList { get; set; }
        public List<string> ExcludeFilterList { get; set; }
        public List<string> IncludeUserList { get; set; }
        public List<string> ExcludeUserList { get; set; }

        public WatchInfo()
        {
            InlucdeFilterList = new List<string>();
            ExcludeFilterList = new List<string>();
            IncludeUserList = new List<string>();
            ExcludeUserList = new List<string>();
        }
    }
}
