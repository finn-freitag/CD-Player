using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CD_Player
{
    public static class AssemblyInfoHelper
    {
        public static string GetCompany()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if (attribs.Length > 0)
            {
                return ((AssemblyCompanyAttribute)attribs[0]).Company;
            }
            return "";
        }

        public static string GetCopyright()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            if (attribs.Length > 0)
            {
                return ((AssemblyCopyrightAttribute)attribs[0]).Copyright;
            }
            return "";
        }

        public static string GetURL()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyURLAttribute), true);
            if (attribs.Length > 0)
            {
                return ((AssemblyURLAttribute)attribs[0]).URL;
            }
            return "";
        }
    }
}
