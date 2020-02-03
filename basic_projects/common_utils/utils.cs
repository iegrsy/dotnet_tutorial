using System;
using System.Collections.Generic;
using System.Linq;

namespace common_utils
{
    public class Utils
    {
        public static Dictionary<string, int> CountWords(string str)
        {
            str = str.Replace(",", " ").Replace(".", " ").Replace(System.Environment.NewLine, " ");
            string[] splt = str.Split(' ');

            Dictionary<string, int> map = new Dictionary<string, int>();
            foreach (var s in splt)
            {
                if (!map.ContainsKey(s))
                    map.Add(s, 1);
                else
                    map[s]++;
            }

            return map;
        }

        public static List<KeyValuePair<string, int>> ShortDictionary4Value(Dictionary<string, int> map)
        {
            var list = map.ToList();
            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            return list;
        }

        public static List<KeyValuePair<string, int>> ShortDictionary4Key(Dictionary<string, int> map)
        {
            var list = map.ToList();
            list.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));
            return list;
        }
    }
}
