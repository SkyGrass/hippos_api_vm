using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hippos_api
{
    public class Menu
    {
        /// <summary>
        /// menuId
        /// </summary>
        public string menuId { get; set; }
        /// <summary>
        /// metaId
        /// </summary>
        public string metaId { get; set; }
        /// <summary>
        /// 设定路由的名字，一定要填写不然使用<keep-alive>时会出现各种问题
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 对应组件
        /// </summary>
        public string component { get; set; }
        /// <summary>
        ///重定向地址，在面包屑中点击会重定向去的地址
        /// </summary>
        public string redirect { get; set; }
        /// <summary>
        /// 不在侧边栏线上
        /// </summary>
        public bool hidden { get; set; }
        /// <summary>
        /// 当你一个路由下面的 children 声明的路由大于1个时，自动会变成嵌套的模式--如组件页面
        /// 只有一个时，会将那个子路由当做根路由显示在侧边栏--如引导页面
        /// 若你想不管路由下面的 children 声明的个数都显示你的根路由
        /// 你可以设置 alwaysShow: true，这样它就会忽略之前定义的规则，一直显示根路由
        /// </summary>
        public bool alwaysShow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Meta meta { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Menu> children { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public string parentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }
        /// <summary>
        /// 是否停用
        /// </summary>
        public bool isclosed { get; set; }
        /// <summary>
        /// 初始化参数
        /// </summary>
        public string query { get; set; }
    }

    public class Meta
    {
        /// <summary>
        /// menuId
        /// </summary>
        public string menuId { get; set; }
        /// <summary>
        /// metaId
        /// </summary>
        public string metaId { get; set; }
        /// <summary>
        /// 设置该路由在侧边栏和面包屑中展示的名字
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 设置该路由的图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 设置该路由进入的权限，支持多个权限叠加
        /// </summary>
        public List<string> roles { get; set; }
        /// <summary>
        /// 角色数据，逗号分隔
        /// </summary>
        public string rolesdata { get; set; }
        /// <summary>
        /// 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
        /// </summary>
        public bool noCache { get; set; }
        /// <summary>
        /// 如果设置为false，则不会在breadcrumb面包屑中显示
        /// </summary>
        public bool breadcrumb { get; set; }
    }
}