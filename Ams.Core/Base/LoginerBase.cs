using System;
using System.Collections.Generic;
namespace Ams.Core
{
    public class LoginerBase
    {
        //拓展框架用，区别不同登陆医院
        public int hos_id { get; set; }
        public string hos_short { get; set; }
        public int roleLevel { get; set; }
        public string role { get; set; }
        public List<KeyValue> allowRole { get; set; }
        public List<int> ut_ids { get; set; }
        public int man_id { get; set; }
        public int ut_id { get; set; }
        public string ut_name { get; set; }
        //public List<int> man_ids { get; set; }
        //public List<int> zs_ids { get; set; }
        public string UserName { get; set; }
        public string UserCode { get; set; }
        public string HosName { get; set; }
    }

    public class KeyValue
    {
        public string key { get; set; }
        public string value { get; set; }
        public bool mark { get; set; }
    }
}
