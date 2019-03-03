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
    /// 角色控制器
    /// </summary>
    public class RoleController : ControllerBase
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetAllRoleList()
        {
            try
            {
                string sqlwhere = string.Format(@"select * from dbo.roletab where rolecode !='sa' ");
                var userlist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", new
                {
                    items = userlist,
                    total = userlist.Count
                });
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult GetRoleForSelect(dynamic model)
        {
            try
            {
                string sqlwhere = string.Format(@"select rolecode  'key', rolename as display_name from dbo.roletab where isnull(isclosed,0) = 0 and rolecode !='sa'");
                var userlist = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(sqlwhere).QueryMany<dynamic>();

                return Success("success", userlist);
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateRole(RoleTab model)
        {
            try
            {
                if (model != null)
                {
                    string roleId = Guid.NewGuid().ToString().ToLower();
                    model.roleId = roleId;
                    if (Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select top 1 * from roletab where rolecode ='{0}'", model.rolecode)).QuerySingle<dynamic>() == null)
                    {
                        var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Insert("RoleTab").
                                  Column("roleId", model.roleId).
                                  Column("rolename", model.rolename).
                                  Column("rolecode", model.rolecode).
                                  Column("isclosed", model.isclosed).
                                  Column("roledescription", model.roledescription).Execute();

                        if (effectRow > 0)
                            return Success("success", model);
                        else
                            return Error("新增角色失败!");
                    }
                    else
                    {
                        return Error("当前角色名已经存在!");
                    }
                }
                else
                {
                    return Error("未能查询到角色!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateRole(RoleTab model)
        {
            try
            {
                if (model != null)
                {
                    var effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("RoleTab").
                                  Column("rolename", model.rolename).
                                  Column("isclosed", model.isclosed).
                                  Column("roledescription", model.roledescription).Where("roleId", model.roleId).Execute();
                    if (effectRow > 0)
                        return Success("操作成功!");
                    else
                        return Error("操作失败!");
                }
                else
                {
                    return Error("未能查询到角色!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }

        /// <summary>
        /// 启用/停用 角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DelRole(dynamic model)
        {
            try
            {
                if (model != null)
                {
                    string roleId = model.roleId ?? string.Empty;
                    var effectRow = 0;
                    if (!string.IsNullOrEmpty(roleId))
                    {
                        var role = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(
                            string.Format(@"select * from dbo.roletab where roleId='{0}'", roleId))
                            .QuerySingle<RoleTab>();
                        if (role != null)
                        {
                            role.isclosed = !role.isclosed;
                            effectRow = Db.Context(APP.DB_DEFAULT_CONN_NAME).Update("RoleTab").
                                Column("isclosed", role.isclosed).Where("roleId", role.roleId).Execute();
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
                    return Error("未能查询到角色!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }
        public class RoleTab
        {
            [PrimaryKey]
            public string roleId { get; set; }
            public string rolename { get; set; }
            public string rolecode { get; set; }
            public bool isclosed { get; set; }
            public string roledescription { get; set; }
        }
    }
}
