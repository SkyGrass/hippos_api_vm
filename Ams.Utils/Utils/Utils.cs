using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ams.Utils
{
    public static class Utils
    {
        public static List<List<T>> GetListGroup<T>(this List<T> list, int groupNum)
        {
            List<List<T>> listGroup = new List<List<T>>();
            for (int i = 0; i < list.Count(); i += groupNum)
            {
                listGroup.Add(list.Skip(i).Take(groupNum).ToList());
            }
            return listGroup;
        }
    }
}
