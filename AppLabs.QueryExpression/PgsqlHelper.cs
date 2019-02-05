using System;

namespace AppLabs.QueryExpression
{
    public class PgsqlHelper
    {
        public static string SanitizeString(string val)
        {
            string tag = "$" + EscapeString(val, 2) + "$";
            return tag + val + tag;
        }

        private static string EscapeString(string val, int numChar)
        {
            string ret = "";
            Random random = new Random();
            if (numChar < 1) return ret;

            while (true)
            {
                ret = "";
                random = new Random();

                int valid = 0;
                int num = 0;

                for (int i = 0; i < numChar; i++)
                {
                    num = 0;
                    valid = 0;
                    while (valid == 0)
                    {
                        num = random.Next(126);
                        if (((num > 64) && (num < 91)) ||
                            ((num > 96) && (num < 123)))
                        {
                            valid = 1;
                        }
                    }
                    ret += (char)num;
                }

                if (!val.Contains("$" + ret + "$")) break;
            }

            return ret;
        }

    }
}
