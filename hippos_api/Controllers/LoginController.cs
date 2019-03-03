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
    /// 登录控制器
    /// </summary>
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Login(dynamic model)
        {
            try
            {
                var token = Guid.NewGuid().ToString().ToLower(); //token
                string sqlwhere = string.Format(@"select top 1 *,'{0}' as token from dbo.usertab where 1=1 and isnull(isclosed,0)=0", token);
                if (model != null)
                {
                    string username = model.username ?? string.Empty; //检索内容
                    string password = model.password ?? string.Empty; //检索内容

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        sqlwhere = string.Format(@"{0} and username ='{1}' and password ='{2}'", sqlwhere, username, CommonMethod.MD5Encrypt32(password));
                    }
                    var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QuerySingle<UserModel>();


                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(user.role))
                        {
                            user.roles = new List<string>(user.role.Split(','));
                        }
                        else
                        {
                            user.roles = new List<string>();
                        }

                        var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Insert("t_token")
                        .Column("userId", user.userId).
                        Column("token", token).
                        Column("recordtime", DateTime.Now.ToString()).
                        Column("experiestime", DateTime.Now.AddHours(8).ToString()).
                        Column("ishanderoff", false).Execute();

                        return Success("success", user);
                    }
                    else
                    {
                        return Error("用户名或者密码错误,请重试!");
                    }
                }
                else
                {
                    return Error("用户名或者密码错误,请重试!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult info(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var user = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select *,'{0}' as token from UserTab where userId in (
                        select userId from t_token where token = '{0}' and experiestime > = GETDATE() and ishanderoff = 0
                        ) and isclosed  = 0", token)).QuerySingle<UserModel>();
                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(user.role))
                        {
                            user.roles = new List<string>(user.role.Split(','));
                        }
                        else
                        {
                            user.roles = new List<string>();
                        }

                        return Success("success", user);
                    }
                    else
                    {
                        return Error("未能查询到用户!");
                    }
                }
                else
                {
                    return Error("token缺失!");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IHttpActionResult logout(UserModel model)
        {
            var token = model.token ?? string.Empty;
            if (!string.IsNullOrEmpty(token))
            {
                Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"update t_token set ishanderoff = 1 where token = '{0}' ", token)).Execute();
            }
            return Success("ok");
        }

        public class UserModel
        {
            public string userId { get; set; }
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string role { get; set; }
            public List<string> roles { get; set; }
            public bool isclosed { get; set; }
            public string avatar { get; set; }
            public string introduction { get; set; }
            public string cuscode { get; set; }
            public string token { get; set; }
        }
    }
}
