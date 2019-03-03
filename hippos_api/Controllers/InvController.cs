using Ams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace hippos_api.Controllers
{
    /// <summary>
    ///产品控制器
    /// </summary>
    public class InvController : ControllerBase
    {
        /// <summary>
        /// 检索产品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetInvList(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    int page = model.page ?? 1;
                    int limit = model.limit ?? 20;

                    string searchword = model.searchword ?? string.Empty; //检索内容
                    string ccuscode = model.ccuscode ?? string.Empty; //检索内容
                    if (string.IsNullOrEmpty(ccuscode))
                    {
                        return Error("未指定客户编码，请核实!");
                    }
                    else
                    {
                        string sqlwhere = string.Format(@"select cinvccode,t1.cinvcode,cinvname,cinvstd,ccomunitcode,ccomunitname,
                        ivolume,isnull(t2.fminquantity,0)fminquantity,isnull(t2.iuprice1,0)iuprice ,
                        isnull(t3.iinvnowcost,t2.iuprice1)iinvnowcost
                        from v_inventory t1 left join (
                        select * from (
                        select cinvcode,fminquantity,iuprice1,dstartdate,row_number() over(partition by cinvcode order by dstartdate desc) as rindex from dbo.v_invprice) as t
                        where t.rindex <=1) t2 
                        on t1.cinvcode = t2.cinvcode
                        left join(select t.cinvcode,t.iinvnowcost from (
                        select *,row_number() over(partition by cinvcode order by dstartdate desc) as rindex from dbo.v_custinvprice) as t
                        where  t.ccuscode = '{0}' and t.rindex <=1) as t3 on t3.cinvcode = t1.cinvcode where 1=1", ccuscode);
                        if (!string.IsNullOrEmpty(searchword))
                        {
                            sqlwhere = string.Format(@"{0} and (t1.cinvcode like '%{1}%' or t1.cinvname like '%{1}%')", sqlwhere, searchword);
                        }

                        var invs = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                        if (invs != null)
                        {
                            return Success("success", new
                            {
                                items = invs.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                                total = invs.Count
                            });
                        }
                        else
                        {
                            return Error("未能查询到产品信息，请核实!");
                        }
                    }
                }
                else
                {
                    return Error("未能查询到产品信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

    }
}
