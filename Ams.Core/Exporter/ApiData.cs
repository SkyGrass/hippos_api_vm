
using System;
using System.Dynamic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ams.Core
{
    public class ApiData : IDataGetter
    {
        public object GetData(HttpContext context)
        {
            try
            {
                dynamic data = null;
                var url = context.Request.Form["dataAction"];
                var param = JsonConvert.DeserializeObject<dynamic>(context.Request.Form["dataParams"]);

                var route = url.Replace("/api/", "").Split('/'); //appointment/report/getdocweekordernumber route[0]=appointment,route[1]=report,route[2]=getdocweekordernumber
                var type = Type.GetType(String.Format("Ams.Areas.{0}.Controllers.{1}ApiController,Ams.Web", route), false, true);
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);

                    var action = route.Length > 2 ? route[2] : "Get";
                    var methodInfo = type.GetMethod(action);
                    var parameters = new object[] { new RequestWrapper().SetRequestData(param) };
                    data = methodInfo.Invoke(instance, parameters);

                    if (data.GetType() == typeof(ExpandoObject))
                    {
                        if ((data as ExpandoObject).Where(x => x.Key == "rows").Count() > 0)
                            data = data.rows;
                    }
                }
                return data;
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
