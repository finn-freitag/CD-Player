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
        public static string GetTitle()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);
            if (attribs.Length > 0)
            {
                return ((AssemblyTitleAttribute)attribs[0]).Title;
            }
            return "";
        }

        public static string GetDescription()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
            if (attribs.Length > 0)
            {
                return ((AssemblyDescriptionAttribute)attribs[0]).Description;
            }
            return "";
        }

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

        public static Dictionary<string, string> GetImageCredits()
        {
            Assembly currentAssem = typeof(Program).Assembly;
            object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyImageCreditsAttribute), true);
            Dictionary<string, string> imageCredits = new Dictionary<string, string>();
            foreach(object attrib in attribs)
            {
                imageCredits.Add(((AssemblyImageCreditsAttribute)attrib).ContributorName, ((AssemblyImageCreditsAttribute)attrib).ContributorURL);
            }
            return imageCredits;
        }
    }
}
