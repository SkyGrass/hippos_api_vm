
using System;
using log4net;
using Newtonsoft.Json;

namespace Ams.Core
{
    public partial class ServiceBase<T> where T : ModelBase, new()
    {
        protected static ILog Log = LogManager.GetLogger(String.Format("Service{0}", typeof(T).Name));

        protected static void Logger(string function, Action tryHandle, Action<Exception> catchHandle = null, Action finallyHandle = null)
        {
            LogHelper.Logger( Log, function, ErrorHandle.Throw, tryHandle, catchHandle, finallyHandle);
        }

        protected static void Logger(string function, ErrorHandle errorHandleType, Action tryHandle, Action<Exception> catchHandle = null, Action finallyHandle = null)
        {
            LogHelper.Logger( Log, function, errorHandleType, tryHandle, catchHandle, finallyHandle);
        }

        public void Logger(string position,string target,string type,object message) 
        {
            using (var context = Db.Context().UseSharedConnection(true))
            {
                var user = FormsAuth.GetUserData();
                context.Insert("sys_log")
                    .Column("UserCode", user.UserCode)
                    .Column("UserName", user.UserName)
                    .Column("Position", position)
                    .Column("Target", target)
                    .Column("Type", type)
                    .Column("Message", JsonConvert.SerializeObject(message))
                    .Column("Date", DateTime.Now)
                    .Execute();
            }
        }
    }
}
