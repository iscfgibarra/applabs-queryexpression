using System;

namespace AppLabs.QueryExpression
{
    public class MssqlHelper
    {
        public static string SanitizeString(string val)
        {
            string ret = "";

            //
            // null, validar el rango ASCII
            //
            for (int i = 0; i < val.Length; i++)
            {
                if (((int)(val[i]) == 10) ||      // Preservar el retorno de carro
                    ((int)(val[i]) == 13))        // y la linea nueva
                {
                    ret += val[i];
                }
                else if ((int)(val[i]) < 32)
                {
                    continue;
                }
                else
                {
                    ret += val[i];
                }
            }

            //
            // doble dash
            //
            while (true)
            {
                var doubleDash = ret.IndexOf("--", StringComparison.Ordinal);
                if (doubleDash < 0)
                {
                    break;
                }
                else
                {
                    ret = ret.Remove(doubleDash, 2);
                }
            }

            //
            // Abrir Comentarios
            // 
            while (true)
            {
                var openComment = ret.IndexOf("/*", StringComparison.Ordinal);
                if (openComment < 0) break;
                else
                {
                    ret = ret.Remove(openComment, 2);
                }
            }

            //
            // Cerrar comentarios
            //
            while (true)
            {
                var closeComment = ret.IndexOf("*/", StringComparison.Ordinal);
                if (closeComment < 0) break;
                else
                {
                    ret = ret.Remove(closeComment, 2);
                }
            }

            //
            // in-string replacement
            //
            ret = ret.Replace("'", "''");
            return ret;
        }
    }
}
