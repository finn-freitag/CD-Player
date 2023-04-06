using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CD_Player
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    [ComVisible(true)]
    public class AssemblyImageCreditsAttribute : Attribute
    {
        private string contributorName;
        public string ContributorName
        {
            get
            {
                return contributorName;
            }
        }

        private string contributorURL;
        public string ContributorURL
        {
            get
            {
                return contributorURL;
            }
        }

        public AssemblyImageCreditsAttribute(string contributorName, string url)
        {
            this.contributorName = contributorName;
            this.contributorURL = url;
        }
    }
}
