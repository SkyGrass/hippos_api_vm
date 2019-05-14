using Ams.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace hippos_api.Controllers
{
    public class PreSellController : ControllerBase
    {
        [HttpPost]
        public IHttpActionResult GetPreSellBillNo(dynamic model)
        {
            try
            {
                string type = model.type ?? "persell";
                var currentDate = DateTime.Now.ToString("yyyyMMdd");
                var currentSerialNo = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select isnull(serialno,1)serialno from t_serialno where year = '{0}' and month = '{1}' and day = '{2}' and type ='{3}'",
                    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, type)).QuerySingle<dynamic>();
                int nextSerialNo = 0;
                if (currentSerialNo != null)
                {
                    nextSerialNo = currentSerialNo.serialno;
                    nextSerialNo += 1;
                    Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"update t_serialno set serialno  = serialno +1 where year = '{0}' and month = '{1}' and day = '{2}' and type='{3}' ",
                  DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, type)).Execute();
                }
                else
                {
                    nextSerialNo += 1;

                    Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"insert into t_serialno (type,year,month,day,serialno) values('{0}','{1}','{2}','{3}','{4}')",
                   type, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, nextSerialNo)).Execute();
                }

                var billConfig = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select prefix from t_billConfig where billType = '{0}' ", type)).QuerySingle<dynamic>();

                string preFixStr = "";
                if (billConfig != null)
                {
                    preFixStr = billConfig.prefix;
                }
                return Success("success", new
                {
                    billNo = preFixStr + currentDate + nextSerialNo.ToString().PadLeft(4, '0'),
                    date = DateTime.Now.ToShortDateString()
                });
            }
            catch (Exception ex)
            {
                return Exception("生成预订单单号发生异常!" + ex.Message);
            }
        }

        public IHttpActionResult SavePreSell(PostForm postform)
        {
            try
            {
                var state = postform.state ?? "add";
                Form form = postform.form;
                List<Item> list = postform.list;

                var effectRow = 0;
                if (form != null && list != null && list.Count > 0)
                {
                    if (string.IsNullOrEmpty(form.FID)) //add
                    {
                        form.FID = GetTableMaxId("t_Order").ToString();

                        list.ForEach(f => f.FID = form.FID.ToString());
                    }

                    List<string> sqls = new List<string>();

                    if (state == "edit")
                    {
                        sqls.Add(string.Format(@"delete from t_order where fid='{0}'", form.FID));
                        sqls.Add(string.Format(@"delete from t_orderentry where fid='{0}'", form.FID));
                    }
                    sqls = sqls.Union(BuildSql(state, form, list)).ToList();

                    using (var db = Db.Context(APP.DB_DEFAULT_CONN_NAME).UseTransaction(true))
                    {
                        sqls.ForEach(f =>
                        {
                            effectRow += db.Sql(f).Execute();
                        });

                        if (effectRow > 0)
                        {
                            db.Commit();
                            return Success("保存成功!", form.FID);
                        }
                        else
                            return Error("保存失败!");
                    }
                }
                else
                {
                    return Error("表单不完整,保存失败!");
                }
            }
            catch (Exception ex)
            {
                return Exception("保存失败!" + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GetPreSellInfo(Query model)
        {
            try
            {
                var id = model.id ?? string.Empty;
                var form = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select t1.*,t2.name as FBillerName,t3.name as FDealerName ,t4.ccusname as FCusName from t_order t1 left join usertab t2 on t1.fbiller =t2.username left outer join usertab t3 on 
t1.fdealercode = t3.username left join v_customer t4 on t1.fcustcode  = t4.ccuscode where ftype <>3   and fid= '{0}'", id)).QuerySingle<Form>();
                var list = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select t1.*,t2.cInvName as FInvName,cInvStd as FInvStd,t2.cComUnitName as FInvUnitName from t_orderentry t1 left join v_Inventory t2 on t1.FInvCode = t2.cInvCode where FID= '{0}'", id)).QueryMany<Item>();
                if (form != null && list != null && list.Count > 0)
                {
                    return Success("", new { form = form, list = list });
                }
                else
                {
                    return Error("未能查询到预订单信息!");
                }
            }
            catch (Exception e)
            {
                return Exception("查询预订单信息失败!" + e.Message);
            }

        }

        [HttpPost]
        public IHttpActionResult GetPreviewData(Query model)
        {
            try
            {
                var id = model.id ?? string.Empty;
                var form = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from v_Order_Preview where fid = '{0}'", id)).QuerySingle<dynamic>();
                var list = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from v_OrderList_Preview where fid= '{0}'", id)).QueryMany<dynamic>();
                if (form != null && list != null && list.Count > 0)
                {
                    return Success("", new { form = form, list = list });
                }
                else
                {
                    return Error("未能查询到预订单信息!");
                }
            }
            catch (Exception e)
            {
                return Exception("查询预订单信息失败!" + e.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult GetPreSellForMonthInfo(Query model)
        {
            try
            {
                var id = model.id ?? string.Empty;
                var form = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select t1.*,t2.name as FBillerName,t3.name as FDealerName ,t4.ccusname as FCusName from t_order t1 left join usertab t2 on t1.fbiller =t2.username left outer join usertab t3 on 
t1.fdealercode = t3.username left join v_customer t4 on t1.fcustcode  = t4.ccuscode where ftype =3 and FID= '{0}'", id)).QuerySingle<Form>();
                var list = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select t1.*,t2.cInvName as FInvName,cInvStd as FInvStd,t2.cComUnitName as FInvUnitName from t_orderentry t1 left join v_Inventory t2 on t1.FInvCode = t2.cInvCode where FID= '{0}'", id)).QueryMany<Item>();
                if (form != null && list != null && list.Count > 0)
                {
                    return Success("", new { form = form, list = list });
                }
                else
                {
                    return Error("未能查询到预订单信息!");
                }
            }
            catch (Exception e)
            {
                return Exception("查询预订单信息失败!" + e.Message);
            }

        }

        [HttpPost]
        public IHttpActionResult GetPreSellList(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select * from v_preselllist where 1=1");
                int page = model.page ?? 1;
                int limit = model.limit ?? 20;
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容
                    string trader = model.trader ?? string.Empty; //检索内容
                    string cuscode = model.cuscode ?? string.Empty; //检索内容
                    string status = model.status ?? string.Empty; //检索内容
                    string role = model.role ?? string.Empty; //检索内容
                    string biller = model.biller ?? string.Empty; //检索内容
                    List<string> date = new List<string>();
                    if (((Newtonsoft.Json.Linq.JContainer)model.date) != null)
                    {
                        if (((Newtonsoft.Json.Linq.JContainer)model.date).HasValues)
                        {
                            date = new List<string>() {
                        Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.date).First.ToString()).ToString("yyyy-MM-dd"),
                         Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.date).Last.ToString()).ToString("yyyy-MM-dd")
                            };
                        }
                    }
                    else
                    {
                        date = new List<string>() {
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") };
                    }


                    List<string> requestdate = new List<string>();
                    if (((Newtonsoft.Json.Linq.JContainer)model.requestdate) != null)
                    {
                        if (((Newtonsoft.Json.Linq.JContainer)model.requestdate).HasValues)
                        {
                            requestdate = new List<string>() {
                        Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.requestdate).First.ToString()).ToString("yyyy-MM-dd"),
                         Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.requestdate).Last.ToString()).ToString("yyyy-MM-dd")
                            };
                        }
                    }
                    else
                    {
                        requestdate = new List<string>() {
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") };
                    }


                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and fbillno like '%{1}%' ", sqlwhere, searchword);
                    }
                    if (!string.IsNullOrEmpty(trader))
                    {
                        sqlwhere = string.Format(@" {0} and fdealercode = '{1}'", sqlwhere, trader);
                    }
                    if (!string.IsNullOrEmpty(cuscode))
                    {
                        sqlwhere = string.Format(@" {0} and fcustcode = '{1}'", sqlwhere, cuscode);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        sqlwhere = string.Format(@" {0} and fstatus = '{1}'", sqlwhere, status);
                    }
                    if (!string.IsNullOrEmpty(role))
                    {
                        switch (role)
                        {
                            case "seller":
                            case "admin":
                                break;
                            default:
                                sqlwhere = string.Format(@" {0} and fbiller = '{1}'", sqlwhere, biller);
                                break;
                        }
                    }

                    if (date.Count > 0)
                    {
                        sqlwhere = string.Format(@" {0} and fdate between '{1}' and '{2}'", sqlwhere, date[0], date[1]);
                    }

                    if (requestdate.Count > 0)
                    {
                        sqlwhere = string.Format(@" {0} and frequestdate between '{1}' and '{2}'", sqlwhere, requestdate[0], requestdate[1]);
                    }
                }

                sqlwhere = string.Format(@" {0} order by fdate,fbillno,fno", sqlwhere);

                var preselllist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", new
                {
                    items = preselllist.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                    total = preselllist.Count
                });
            }
            catch (Exception e)
            {
                return Exception("查询预订单信息失败!" + e.Message);
            }

        }


        [HttpPost]
        public IHttpActionResult GetPreSellForMonthList(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select * from v_presellformonthlist where 1=1");
                int page = model.page ?? 1;
                int limit = model.limit ?? 20;
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容
                    string trader = model.trader ?? string.Empty; //检索内容
                    string cuscode = model.cuscode ?? string.Empty; //检索内容
                    string status = model.status ?? string.Empty; //检索内容
                    string role = model.role ?? string.Empty; //检索内容
                    string biller = model.biller ?? string.Empty; //检索内容
                    List<string> date = new List<string>();
                    if (((Newtonsoft.Json.Linq.JContainer)model.date) != null)
                    {
                        if (((Newtonsoft.Json.Linq.JContainer)model.date).HasValues)
                        {
                            date = new List<string>() {
                        Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.date).First.ToString()).ToString("yyyy-MM-dd"),
                         Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.date).Last.ToString()).ToString("yyyy-MM-dd")
                            };
                        }
                    }
                    else
                    {
                        date = new List<string>() {
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") };
                    }


                    List<string> requestdate = new List<string>();
                    if (((Newtonsoft.Json.Linq.JContainer)model.requestdate) != null)
                    {
                        if (((Newtonsoft.Json.Linq.JContainer)model.requestdate).HasValues)
                        {
                            requestdate = new List<string>() {
                        Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.requestdate).First.ToString()).ToString("yyyy-MM-dd"),
                         Convert.ToDateTime(((Newtonsoft.Json.Linq.JContainer)model.requestdate).Last.ToString()).ToString("yyyy-MM-dd")
                            };
                        }
                    }
                    else
                    {
                        requestdate = new List<string>() {
                        DateTime.Now.ToString("yyyy-MM-dd"),
                        DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") };
                    }


                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and fbillno like '%{1}%' ", sqlwhere, searchword);
                    }
                    if (!string.IsNullOrEmpty(trader))
                    {
                        sqlwhere = string.Format(@" {0} and fdealercode = '{1}'", sqlwhere, trader);
                    }
                    if (!string.IsNullOrEmpty(cuscode))
                    {
                        sqlwhere = string.Format(@" {0} and fcustcode = '{1}'", sqlwhere, cuscode);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        sqlwhere = string.Format(@" {0} and fstatus = '{1}'", sqlwhere, status);
                    }
                    if (!string.IsNullOrEmpty(role))
                    {
                        switch (role)
                        {
                            case "seller":
                            case "admin":
                                break;
                            default:
                                sqlwhere = string.Format(@" {0} and fbiller = '{1}'", sqlwhere, biller);
                                break;
                        }
                    }

                    if (date.Count > 0)
                    {
                        sqlwhere = string.Format(@" {0} and fdate between '{1}' and '{2}'", sqlwhere, date[0], date[1]);
                    }

                    if (requestdate.Count > 0)
                    {
                        sqlwhere = string.Format(@" {0} and frequestdate between '{1}' and '{2}'", sqlwhere, requestdate[0], requestdate[1]);
                    }
                }

                sqlwhere = string.Format(@" {0} order by fdate,fbillno,fno", sqlwhere);

                var preselllist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", new
                {
                    items = preselllist.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                    total = preselllist.Count
                });
            }
            catch (Exception e)
            {
                return Exception("查询预订单信息失败!" + e.Message);
            }

        }


        [HttpPost]
        public IHttpActionResult AuditPreSell(dynamic model)
        {
            try
            {
                string id = model.id ?? string.Empty;
                string verifier = model.verifier ?? string.Empty;
                var current = DateTime.Now;
                var newstatus = 1;
                var row = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from t_order where fid = '{0}' and fstatus =0",
                    id)).QuerySingle<dynamic>();
                if (row != null)
                {
                    var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"update t_order set fverifier ='{3}' ,fverifydate = '{0}',
                        fstatus = {2} where fid = '{1}'", current, id, newstatus, verifier)).Execute();

                    if (effectRow > 0)
                    {
                        return Success("审批成功!", new
                        {
                            verifier = verifier,
                            date = current,
                            status = newstatus,
                            statusname = "已审批"
                        });
                    }
                    else
                    {
                        return Error("审批失败!");
                    }
                }
                else
                {
                    return Error("未能查询到订单或者订单已审批\n 审批失败!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult UnAuditPreSell(dynamic model)
        {
            try
            {
                string id = model.id ?? string.Empty;
                var newstatus = 0;
                var row = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from t_order where fid = '{0}' and fstatus =1",
                    id)).QuerySingle<dynamic>();
                if (row != null)
                {
                    var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"update t_order set fverifier =NULL ,fverifydate =NULL,
                        fstatus = {1} where fid = '{0}'", id, newstatus)).Execute();

                    if (effectRow > 0)
                    {
                        return Success("弃审成功!", new
                        {
                            verifier = string.Empty,
                            date = string.Empty,
                            status = newstatus,
                            statusname = "已弃审"
                        });
                    }
                    else
                    {
                        return Error("弃审失败!");
                    }
                }
                else
                {
                    return Error("未能查询到订单或者订单状态不符合\n 弃审失败!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 删除预订单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult DelPreSell(dynamic model)
        {
            try
            {
                string id = model.id ?? string.Empty;
                var row = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from t_order where fid = '{0}' and fstatus =0 ",
                    id)).QuerySingle<dynamic>();
                if (row != null)
                {
                    List<string> list = new List<string>();
                    list.Clear();

                    list.Add(string.Format(@"delete from t_order where fid ={0}", id));
                    list.Add(string.Format(@"delete from t_orderEntry where fid = {0}", id));

                    var effectRow = 0;

                    using (var db = Db.Context(APP.DB_DEFAULT_CONN_NAME).UseTransaction(true))
                    {

                        list.ForEach(f =>
                        {
                            effectRow += db.Sql(f).Execute();
                        });

                        if (effectRow > 0)
                        {
                            db.Commit();
                            return Success("删除成功!", new
                            {
                                id = id
                            });
                        }
                        else
                        {
                            return Success("删除失败!");
                        }
                    }
                }
                else
                {
                    return Error("未能查询到订单或者订单已审批\n 删除失败!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult BuildU8SO(dynamic model)
        {
            try
            {
                string id = model.id ?? string.Empty;
                var row = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from t_order where fid = '{0}' and fstatus =1",
                   id)).QuerySingle<dynamic>();
                if (row != null)
                {
                    var WsUrl = ConfigurationManager.AppSettings["WsUrl"];
                    var Method = ConfigurationManager.AppSettings["Method"];
                    var args = new object[] { id, null };
                    object result = WsHelper.InvokeWebService(WsUrl, Method, args);
                    if (args[1] != null && (string)args[1] != "")
                    {
                        return Error(args[1].ToString());
                    }

                    var r = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select t1.*,t2.name as FBillerName,t3.name as FDealerName ,t4.ccusname as FCusName from t_order t1 left join usertab t2 on t1.fbiller =t2.username left outer join usertab t3 on 
t1.fdealercode = t3.username left join v_customer t4 on t1.fcustcode  = t4.ccuscode  where fid = '{0}' and fstatus =2",
                   id)).QuerySingle<Form>();
                    if (r != null)
                    {
                        return Success("生单成功!", r);
                    }
                    else
                    {
                        return Error("生单失败!");
                    }
                }
                else
                {
                    return Error("未能查询到订单或者订单已生单\n 生单失败!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }
        public List<string> BuildSql(string state, Form form, List<Item> list)
        {
            List<string> ls_sql = new List<string>();
            ls_sql.Clear();

            string sql_columns = string.Empty;
            string sql_values = string.Empty;

            List<string> FormNoNeedSaveColumns = new List<string>() {
                "FDealerName",
                "FBillerName",
                "FCusName"
            };
            #region 遍历对象
            foreach (System.Reflection.PropertyInfo p in form.GetType().GetProperties())
            {
                var proName = p.Name;
                if (!FormNoNeedSaveColumns.Exists(item => item == proName))
                {
                    sql_columns += string.Format(@"{0},", proName);

                    var proValue = p.GetValue(form);
                    sql_values += string.Format(@"'{0}',", proValue);
                }
            }
            #endregion

            sql_columns = sql_columns.Substring(0, sql_columns.Length - 1); //remove last ,
            sql_values = sql_values.Substring(0, sql_values.Length - 1); //remove last ,

            ls_sql.Add(string.Format(@"insert into t_Order({0})values({1})", sql_columns, sql_values));

            List<string> EntryNoNeedSaveColumns = new List<string>() {
                "FEntryID",
                "FInvName",
                "FInvStd",
                "FInvUnitName"
            };

            list.ForEach(f =>
            {
                sql_columns = string.Empty;
                sql_values = string.Empty;

                #region 遍历对象
                foreach (System.Reflection.PropertyInfo p in f.GetType().GetProperties())
                {

                    var proName = p.Name;
                    if (!EntryNoNeedSaveColumns.Exists(item => item == proName))
                    {
                        sql_columns += string.Format(@"{0},", proName);

                        var proValue = p.GetValue(f);
                        sql_values += string.Format(@"'{0}',", proValue);
                    }
                }
                #endregion

                sql_columns = sql_columns.Substring(0, sql_columns.Length - 1); //remove last ,
                sql_values = sql_values.Substring(0, sql_values.Length - 1); //remove last ,

                ls_sql.Add(string.Format(@"insert into t_OrderEntry({0})values({1})", sql_columns, sql_values));
            });

            return ls_sql;
        }


        public int GetTableMaxId(string tableName)
        {
            var maxId = 0;
            var record = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select top 1 * from t_maxid where tablename = '{0}'",
                    tableName)).QuerySingle<TableMaxId>();
            if (record != null)
            {
                maxId = record.maxid + 1;
                Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"update t_maxid set maxid = maxid +1 where tablename='{0}'",
                                   tableName)).QuerySingle<TableMaxId>();
            }
            else
            {
                maxId += 1;
                Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"insert into t_maxid(tablename,maxid)values('{0}',{1})",
                                    tableName, maxId)).QuerySingle<TableMaxId>();
            }

            return maxId;
        }

        public class Query
        {
            public string id { get; set; }
        }

        public class PostForm
        {
            public string state { get; set; }
            public Form form { get; set; }
            public List<Item> list { get; set; }

        }
        public class Form
        {
            /// <summary>
            /// 
            /// </summary>
            public string FID { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int FType { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FBillNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FSTCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FCustCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FDealerCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int FTaxRate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FBiller { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FVerifier { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FVerifyDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Fremark { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FU8BillNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FStatus { get; set; }
            public string FBillerName { get; set; }
            public string FDealerName { get; set; }
            public string FCusName { get; set; }
        }
        public class Item
        {
            public string FEntryID { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FID { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FNo { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FInvCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FInvName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FInvStd { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FInvUnitCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FInvUnitName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FQty { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FPlanPrice { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FTaxPrice { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FPrice { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FAmount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FTaxAmount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int FTaxRate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FDisAmount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FSum { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FRequestDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FProject { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FEntryRemark { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FRowState { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FPrice2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FAmount2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FTaxPrice2 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public float FSum2 { get; set; }
            public float FVolume { get; set; }
            public float FTotalVolume { get; set; }
        }
        public class TableMaxId
        {
            public string tablename { get; set; }
            public int maxid { get; set; }
        }
    }
}
