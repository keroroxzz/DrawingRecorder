/*
 * Author : Brian Tu (RTU)
 * Last modify : -
 * 
 * Manage the ini file.
 */

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace DRnamespace
{
    class IniManip
    {
        private readonly int buffer;
        private readonly string path;
        private readonly StringBuilder str;

        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        public IniManip(string path)
        {
            buffer = 512;
            this.path = System.Windows.Forms.Application.StartupPath + path;
            str = new StringBuilder();
        }

        public string Get(string section, string key, string def)
        {
            GetPrivateProfileString(section, key, def, str, buffer, path);
            return str.ToString();
        }
        public bool GetBool(string section, string key, bool def)
        {
            GetPrivateProfileString(section, key, "", str, buffer, path);

            String s = str.ToString();
            return s.ToLower() == "true" ? true : ( s == "" ? def : false );
        }

        public int GetInt(string section, string key, int def)
        {
            GetPrivateProfileString(section, key, "", str, buffer, path);
            return str.Length == 0 ? def : Convert.ToInt32(str.ToString());
        }

        public void Write(string section, string key, string content)
        {
            WritePrivateProfileString( section, key, content, path);
        }
    }
}
