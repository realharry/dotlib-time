using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotTime.Core
{
    // tbd: Use enum???
    public static class TermTypes
    {
        // TBD
        public const string TERM_MONTHLY = "monthly";
        public const string TERM_WEEKLY = "weekly";
        public const string TERM_DAILY = "daily";
        public const string TERM_HOURLY = "hourly";
        public const string TERM_CUMULATIVE = "cumulative";
        public const string TERM_CURRENT = "current";

        private static readonly ISet<string> sTypes = new HashSet<string>();
        static TermTypes()
        {
            sTypes.Add(TERM_MONTHLY);
            sTypes.Add(TERM_WEEKLY);
            sTypes.Add(TERM_DAILY);
            sTypes.Add(TERM_HOURLY);
            sTypes.Add(TERM_CUMULATIVE);
            sTypes.Add(TERM_CURRENT);
        }

        public static bool isValid(string type)
        {
            if (sTypes.Contains(type)) {
                return true;
            } else {
                return false;
            }
        }

        public static string getDefaultType()
        {
            // temporary
            //return TERM_CURRENT;
            return TERM_DAILY;
        }
    }
}
