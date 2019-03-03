using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace hippos_api
{
    public class CommonMethod
    {
        /// <summary>
        /// 构建条路由的meta属性
        /// </summary>
        /// <param name="currentTarget">路由数据对象</param>
        /// <param name="metas">meta数据集</param>
        /// <returns>当前条路由数据对象</returns>
        public static List<Menu> BuildMenuMeta(List<Menu> menus, List<Meta> metas)
        {
            menus.ForEach(f =>
            {
                var meta = metas.Find(m => f.menuId == m.menuId && f.metaId == m.metaId);
                if (meta != null)
                {
                    f.meta = meta;

                    if (meta.rolesdata != null && meta.rolesdata.Length > 0)
                    {
                        f.meta.roles = new List<string>(meta.rolesdata.Split(','));
                    }
                }
            });
            return menus;
        }
        /// <summary>
        /// 构建单条路由的children属性
        /// </summary>
        /// <param name="currentTarget">当前条路由数据对象</param>
        /// <param name="menus">全部路由数据集</param>
        /// <returns>当前条路由数据对象</returns>
        public static List<Menu> BuildMenuChildren(List<Menu> menus)
        {
            List<Menu> list = new List<Menu>();
            menus.ForEach(f =>
            {
                if (f.parentId == null)
                {
                    list.Add(f);
                }
                else
                {
                    BuildMenuChildren(f, menus, list);
                }
            });

            return list;
        }
        public static List<Menu> BuildMenuChildren(Menu menu, List<Menu> menus, List<Menu> list)
        {
            var father = menus.Find(m => m.menuId == menu.parentId);   //找父级
            if (father != null)
            {
                if (father.children == null)
                {
                    father.children = new List<Menu>();
                }
                father.children.Add(menu); //加入子节点

                if (!list.Exists(f => f.menuId == father.menuId))
                {
                    list.Add(father);
                    //var temp = list.Find(f => f.menuId == father.menuId);
                    //temp.children.Add(father);
                }
                BuildMenuChildren(father, menus, list);
            }
            else
            {
                //数据异常，自动忽略
            }
            return list;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }
    }
}