/*
 * Author : Brian Tu (RTU)
 * Last modify : -
 * 
 * Trying to make a better class that can bind variables with settings in ini.
 * But the members of classes could not be passed by ref, so it's useless now.
 */

using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    class SettingManip
    {
        struct Item<T>
        {
            public T obj;
            public String path, section, key;

            public Item(ref T o, String p, String s, String k)
            {
                obj = o;
                path = p;
                section = s;
                key = k;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        ArrayList list = new ArrayList();

        public void Bond<T>(ref T obj, String path, String section, String key)
        {
            list.Add(new Item<T>(ref obj, path, section, key));
        }

        public void Read(String path = null, String section = null, String key = null)
        {
            StringBuilder str = new StringBuilder();

            for(int i=0;i<list.Count;i++)
            {
                Console.WriteLine(list[i]);
                //((Item)list[i]).obj = 
                /*
                if ((path == null || i.path == path) && ( section == null || i.section == section ) && ( key == null || i.key == key ))
                {
                    GetPrivateProfileString(i.section, i.key, null, str, 256, i.path);

                    //if(i.type == typeof(String))
                }*/
            }
        }
    }
}
