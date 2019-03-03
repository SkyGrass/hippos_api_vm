﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Ams.Data;

namespace Ams.Core
{
    public class EditPageEventArgs
    {
        //execute
        public IDbContext db { get; set; }
        public int executeValue { get; set; }

        //arg
        public JObject data { get; set; }
        public RequestWrapper formWrapper { get; set; }
        public List<RequestWrapper> tabsWrapper { get; set; }
 
        //form
        public JToken dataNew { get; set; }
        public dynamic dataOld { get; set; }
        public RequestWrapper dataWrapper { get; set; }
        public OptType dataAction { get; set; }

        //rows
        public int tabIndex { get; set; }
        public TabType tabType { get; set; }
        public JToken tabData { get; set; }

        public EditPageEventArgs()
        {
            dataAction = OptType.None;
        }
    }
}
