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
    /// 
    /// </summary>
    public class U8CusController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllU8CusList(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select ccuscode,ccusname from dbo.v_customer where 1=1");
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容

                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and (ccuscode like '%{1}%' or ccusname like '%{1}%')", sqlwhere, searchword);
                    }
                }
                var customers = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (customers != null)
                {
                    return Success("success", new
                    {
                        items = customers,
                        total = customers.Count
                    });
                }
                else
                {
                    return Error("未能查询到客戶信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllU8CusListForCanBind(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select ccuscode,ccusname from dbo.v_customer where ccuscode not in (select isnull(cuscode,'') from dbo.usertab) 
                                and ccuscode not in (select customerusercode from tradercustomermap)");
                int page = model.page ?? 1;
                int limit = model.limit ?? 20;
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容

                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and (ccuscode like '%{1}%' or ccusname like '%{1}%')", sqlwhere, searchword);
                    }
                }
                var customers = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (customers != null)
                {
                    return Success("success", new
                    {
                        items = customers.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                        total = customers.Count
                    });
                }
                else
                {
                    return Error("未能查询到客戶信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllU8CusListWithCode(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select top 1 ccuscode,ccusname from dbo.v_customer where 1=1 ");
                if (model != null)
                {
                    string cuscode = model.cuscode ?? string.Empty; //检索内容

                    if (!string.IsNullOrEmpty(cuscode))
                    {
                        sqlwhere = string.Format(@"{0} and  ccuscode in (select isnull(ccuscode,'') from usertab where username ='{1}')", sqlwhere, cuscode);
                    }
                }
                var customers = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (customers != null)
                {
                    return Success("success", new
                    {
                        items = customers,
                        total = customers.Count
                    });
                }
                else
                {
                    return Error("未能查询到客戶信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllU8CusListForBind(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select ccuscode,ccusname from dbo.v_customer where 1=1 and ccuscode not in(select isnull(cuscode,'') from usertab)");
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容
                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0}  and  ccuscode not in (select customerusercode from tradercustomermap)
                            and (ccuscode like '%{1}%' or ccusname like '%{1}%')", sqlwhere, searchword);
                    }
                    else
                    {
                        sqlwhere = string.Format(@"{0}  and  ccuscode not in (select customerusercode from tradercustomermap)", sqlwhere);
                    }
                }
                var customers = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (customers != null)
                {
                    return Success("success", new
                    {
                        items = customers,
                        total = customers.Count
                    });
                }
                else
                {
                    return Error("未能查询到客戶信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllU8CusListHaveBind(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select ccuscode,ccusname from dbo.v_customer where 1=1 ");
                if (model != null)
                {
                    string trader = model.trader ?? string.Empty; //检索内容
                    sqlwhere = string.Format(@"{0} and  ccuscode in (select customerusercode from tradercustomermap where traderusercode='{1}')", sqlwhere, trader);

                }
                var customers = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                if (customers != null)
                {
                    return Success("success", new
                    {
                        items = customers,
                        total = customers.Count
                    });
                }
                else
                {
                    return Error("未能查询到客戶信息，请核实!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult saveTraderBindCus(M model)
        {
            try
            {
                if (model != null)
                {
                    List<Item> list = model.list;
                    var trader = model.trader ?? string.Empty;
                    List<string> list_sql = new List<string>();

                    list_sql.Add(string.Format(@"delete from  tradercustomermap where traderusercode='{0}'", trader));

                    if (list != null && list.Count > 0)
                    {
                        list.ForEach(f =>
                        {
                            list_sql.Add(string.Format(@"insert into tradercustomermap(traderusercode,customerusercode) values ('{0}','{1}')", f.trader, f.customer));
                        });

                        var effectRow = 0;
                        using (var db = Db.Context(APP.DB_DEFAULT_CONN_NAME).UseTransaction(true))
                        {
                            list_sql.ForEach(f =>
                            {
                                effectRow += db.Sql(f).Execute();
                            });

                            if (effectRow > 0)
                            {
                                db.Commit();
                                return Success("保存数据成功!");
                            }
                            else
                            {
                                return Error("保存数据失败!");
                            }
                        }
                    }
                    else
                    {
                        return Error("未能发现表单数据!");
                    }
                }
                else
                {
                    return Error("未能发现表单数据!");
                }

            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }


        public class M
        {
            public string trader { get; set; }
            public List<Item> list { get; set; }
        }

        public class Item
        {
            public string trader { get; set; }
            public string customer { get; set; }
        }
    }
}
