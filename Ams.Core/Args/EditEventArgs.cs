using System;
using Newtonsoft.Json.Linq;
using Ams.Data;

namespace Ams.Core
{
    public class EditEventArgs
    {
        public IDbContext db { get; set; }
        public JToken form { get; set; }
        public dynamic formOld { get; set; }
        public JToken row { get; set; }
        public dynamic rowOld { get; set; }
        public JToken list { get; set; }
        public RequestWrapper wrapper { get; set; }
        public OptType type { get; set; }
        public dynamic executeValue { get; set; }

        public EditEventArgs()
        {
            type = OptType.None;
        }
    }
}
