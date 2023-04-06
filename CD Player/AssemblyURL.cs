using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CD_Player
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    [ComVisible(true)]
    public class AssemblyURLAttribute : Attribute
    {
        private string m_url;
        public string URL
        {
            get
            {
                return m_url;
            }
        }

        public AssemblyURLAttribute(string url)
        {
            m_url = url;
        }
    }
}
