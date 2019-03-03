using Ams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace hippos_api.Controllers
{
    public class U8STController : ControllerBase
    {
        [HttpPost]
        public IHttpActionResult GetAllU8StList()
        {
            try
            {
                string sqlwhere = string.Format(@"select cstcode,cstname from v_saletype");
                var list = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (list != null)
                {
                    return Success("success", new
                    {
                        items = list,
                        total = list.Count
                    });
                }
                else
                {
                    return Error("未能查询到销售类型信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }
    }
}
