using Ams.Core;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace hippos_api.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    public class MenuController : ControllerBase
    {
        /// <summary>
        /// 菜单列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetMenuList()
        {
            //全部菜单
            var menus = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from dbo.menutab where 1=1 order by parentid,sort")).QueryMany<Menu>();

            //全部meta
            var metas = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from dbo.menumetatab")).QueryMany<Meta>();

            //待返回菜单数据格式
            List<Menu> menulist = new List<Menu>();
            menulist = CommonMethod.BuildMenuMeta(menus, metas);
            menulist = CommonMethod.BuildMenuChildren(menus);
            return Success("success", menulist);
        }

        /// <summary>
        /// 根据角色获取菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetMenuByRole(M m)
        {
            try
            {
                if (m != null && m.roles.Count > 0)
                {
                    string role = m.roles[0];
                    //全部菜单
                    var menus = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from dbo.menutab where isnull(isclosed,0) =0 order by parentid,sort")).QueryMany<Menu>();

                    //全部meta
                    var metas = Db.Context(APP.DB_DEFAULT_CONN_NAME).Sql(string.Format(@"select * from dbo.menumetatab")).QueryMany<Meta>();

                    //待返回菜单数据格式
                    List<Menu> menulist = new List<Menu>();
                    menulist = CommonMethod.BuildMenuMeta(menus, metas);
                    menulist = CommonMethod.BuildMenuChildren(menus);
                    return Success("success", menulist);
                }
                else
                {
                    return Error("未能获取到用户角色!");
                }
            }
            catch (Exception e)
            {
                return Exception(e.Message);
            }
        }
        public class M
        {
            public List<string> roles;
        }
    }
}
