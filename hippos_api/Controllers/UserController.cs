using Ams.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace hippos_api.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllUserList(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select * from dbo.v_userTab where 1=1 and username !='sa'");
                int page = model.page ?? 1;
                int limit = model.limit ?? 20;
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容
                    string role = model.role ?? "all"; //默认角色
                    string isclosed = model.isclosed ?? "all";//是否停用

                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and (username like '%{1}%' or name like '%{1}%' or cuscode like '%{1}%' or cusname like '%{1}%') ", sqlwhere, searchword);
                    }
                    if (role != "all" && role != "")
                    {
                        sqlwhere = string.Format(@" {0} and role = '{1}' ", sqlwhere, role);
                    }
                    if (isclosed != "all")
                    {
                        sqlwhere = string.Format(@" {0} and isnull(isclosed,0)= '{1}' ", sqlwhere, isclosed);
                    }
                }

                var userlist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", new
                {
                    items = userlist.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                    total = userlist.Count
                });
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }


        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllTraderList(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select * from dbo.usertab where 1=1 and  role = 'trader'");
                int page = model.page ?? 1;
                int limit = model.limit ?? 20;
                if (model != null)
                {
                    string searchword = model.searchword ?? string.Empty; //检索内容
                    string isclosed = model.isclosed ?? "all";//是否停用

                    if (!string.IsNullOrEmpty(searchword))
                    {
                        sqlwhere = string.Format(@"{0} and (username like '%{1}%' or name like '%{1}%') ", sqlwhere, searchword);
                    }
                    if (isclosed != "all")
                    {
                        sqlwhere = string.Format(@" {0} and isnull(isclosed,0)= '{1}' ", sqlwhere, isclosed);
                    }
                }

                var userlist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", new
                {
                    items = userlist.Skip<dynamic>((page - 1) * limit).Take<dynamic>(limit).ToList<dynamic>(),
                    total = userlist.Count
                });
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateUser(UserTab model)
        {
            try
            {
                if (model != null)
                {
                    string userId = Guid.NewGuid().ToString().ToLower();
                    model.userId = userId;
                    if (Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select top 1 * from usertab where username ='{0}'", model.username)).QuerySingle<dynamic>() == null)
                    {
                        var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Insert("UserTab").
                                      Column("userId", model.userId).
                                      Column("name", model.name).
                                      Column("username", model.username).
                                      Column("password", CommonMethod.MD5Encrypt32("123456")).
                                       Column("cuscode", model.cuscode).
                                      Column("role", model.role).
                                      Column("isclosed", model.isclosed).
                                      Column("avatar", model.avatar).
                                      Column("introduction", model.introduction).Execute();

                        if (effectRow > 0)
                            return Success("success", model);
                        else
                            return Error("新增用户失败!");
                    }
                    else
                    {
                        return Error("当前用户名已经存在!");
                    }
                }
                else
                {
                    return Error("未能查询到表单!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateUser(UserTab model)
        {
            try
            {
                if (model != null)
                {
                    if (model.role == "customer")
                    {
                        var cusExist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from v_customer where ccuscode ='{0}'", model.cuscode))
                              .QuerySingle<dynamic>();
                        if (cusExist == null)
                            return Error("绑定的U8客户不存在!");
                    }
                    else
                    {
                        model.cuscode = string.Empty;
                    }
                }
                else
                {
                    return Error("操作失败!");
                }

                var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("UserTab").
                                  Column("name", model.name).
                                  Column("role", model.role).
                                  Column("isclosed", model.isclosed).
                                  Column("avatar", model.avatar).
                                  Column("cuscode", model.cuscode).
                                  Column("introduction", model.introduction).Where("userId", model.userId).Execute();
                if (effectRow > 0)
                    return Success("操作成功!");
                else
                    return Error("操作失败!");

            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 启用/停用 用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DelUser(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    string userId = model.userId ?? string.Empty;
                    var effectRow = 0;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(
                            string.Format(@"select * from dbo.usertab where userId='{0}'", userId))
                            .QuerySingle<UserTab>();
                        if (user != null)
                        {
                            user.isclosed = !user.isclosed;
                            effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("UserTab").
                                Column("isclosed", user.isclosed).Where("userId", user.userId).Execute();
                        }
                        else
                        {
                            return Error("操作失败!");
                        }
                    }
                    else
                    {
                        return Error("操作失败!");
                    }
                    if (effectRow > 0)
                        return Success("操作成功!");
                    else
                        return Error("操作失败!");
                }
                else
                {
                    return Error("未能查询到用户!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 删除 用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult RemoveUser(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    string userId = model.userId ?? string.Empty;
                    var effectRow = 0;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(
                            string.Format(@"select * from dbo.usertab where userId='{0}'", userId))
                            .QuerySingle<UserTab>();

                        if (user != null)
                        {
                            if (user.username.ToLower() == "admin")
                            {
                                return Error("管理员禁止删除! \r\n 操作失败!");
                            }
                            var haveRecord = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select top 1 * from t_order where fbiller = '{0}'", user.username)).QuerySingle<dynamic>();
                            if (haveRecord != null)
                            {
                                return Error("当前用户存在订单记录 \r\n 操作失败!");
                            }
                            else
                            {
                                effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Delete("UserTab").Where("userId", user.userId).Execute();
                            }
                        }
                        else
                        {
                            return Error("操作失败!");
                        }
                    }
                    else
                    {
                        return Error("操作失败!");
                    }
                    if (effectRow > 0)
                        return Success("操作成功!");
                    else
                        return Error("操作失败!");
                }
                else
                {
                    return Error("未能查询到用户!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 初始化用户密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ResetUserPwd(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    string userId = model.userId ?? string.Empty;
                    var effectRow = 0;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(
                            string.Format(@"select * from dbo.usertab where userId='{0}'", userId))
                            .QuerySingle<UserTab>();
                        if (user != null)
                        {
                            user.password = CommonMethod.MD5Encrypt32("123456");
                            effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("UserTab").
                                Column("password", user.password).Where("userId", user.userId).Execute();

                            if (effectRow > 0)
                                return Success("操作成功!");
                            else
                                return Error("操作失败!");
                        }
                        else
                        {
                            return Error("操作失败!");
                        }
                    }
                    else
                    {
                        return Error("操作失败!");
                    }
                }
                else
                {
                    return Error("未能查询到用户!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ModifyPassword(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    string userId = model.userId ?? string.Empty;
                    string newpassword = model.newpassword ?? string.Empty;
                    var effectRow = 0;
                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(newpassword))
                    {
                        var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(
                            string.Format(@"select * from dbo.usertab where userId='{0}'", userId))
                            .QuerySingle<UserTab>();
                        if (user != null)
                        {
                            user.password = CommonMethod.MD5Encrypt32(newpassword);
                            effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("UserTab").
                                Column("password", user.password).Where("userId", user.userId).Execute();
                        }
                        else
                        {
                            return Error("操作失败!");
                        }
                    }
                    else
                    {
                        return Error("操作失败!");
                    }
                    if (effectRow > 0)
                        return Success("操作成功!");
                    else
                        return Error("操作失败!");
                }
                else
                {
                    return Error("未能查询到用户!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }
        public class UserTab
        {
            [PrimaryKey]
            public string userId { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string role { get; set; }
            public bool isclosed { get; set; }
            public string avatar { get; set; }
            public string introduction { get; set; }
            public string cuscode { get; set; }
        }
    }
}
