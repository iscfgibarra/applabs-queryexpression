using System;
using System.Collections;
using System.Collections.Generic;

namespace AppLabs.QueryExpression
{
    public class WrapperExpression
    {
        #region Constantes

        public const string LeftTermToBeStringOrExpression = "El termino izquierdo debe ser una WrapperExpression o una cadena.";


        #endregion


        #region Constructor

        /// <summary>
        /// Una estructura de la forma termino-operador-termino que define una operación booleana dentro de una clausula WHERE.       
        /// </summary>
        public WrapperExpression()
        {
        }

        /// <summary>
        /// Una estructura de la forma termino-operador-termino que define una operación booleana dentro de una clausula WHERE.               
        /// </summary>
        /// <param name="left">El termino izquierdo de la operación; puede ser una cadena o una expresion anidada.</param>
        /// <param name="oper">El operador.</param>
        /// <param name="right">El termino derecho de la operación; puede ser un objeto para comparación o una expresion anidada.</param>
        public WrapperExpression(object left, Operators oper, object right)
        {
            LeftTerm = left;
            Operator = oper;
            RightTerm = right;
        }

        #endregion

        #region Public-Members

        /// <summary>
        /// Termino izquierdo de la operación; puede ser una cadena o una expresion anidada.
        /// </summary>
        public object LeftTerm;

        /// <summary>
        ///El operador
        /// </summary>
        public Operators Operator;

        /// <summary>
        /// Termino derecho de la operación; puede ser un objeto para comparación o una expresion anidada.
        /// </summary>
        public object RightTerm;

        #endregion

        #region Private-Members

        #endregion

        #region Public-Methods

        /// <summary>
        /// Convierte una expresión a una cadena que es compatible para usarla como parte de una clausula WHERE.        
        /// </summary>
        /// <param name="dbType">Tipo de base de datos.</param>
        /// <returns>Cadena que contiene una version de la expresion como clausula utilizable en una expresion WHERE.</returns>
        public string ToWhereClause(string dbType)
        {
            if (String.IsNullOrEmpty(dbType)) throw new ArgumentNullException(nameof(dbType));
            switch (dbType.ToLower())
            {
                case "mssql":
                    return ToWhereClause(DbTypes.MsSql);

                case "mysql":
                    return ToWhereClause(DbTypes.MySql);

                case "pgsql":
                    return ToWhereClause(DbTypes.PgSql);

                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType));
            }
        }

        /// <summary>
        /// Convierte una expresión a una cadena que es compatible para usarla como parte de una clausula WHERE.
        /// </summary>
        /// <param name="dbType">Tipo de base de datos. Default MSSQL</param>
        /// <returns>Cadena que contiene una version de la expresion como clausula WHERE.</returns>
        public string ToWhereClause(DbTypes dbType = DbTypes.MsSql)
        {
            string clause = "";

            if (LeftTerm == null) return null;

            clause += "(";

            if (LeftTerm is WrapperExpression wrapperExpression)
            {
                clause += wrapperExpression.ToWhereClause(dbType) + " ";
            }
            else
            {
                if (!(LeftTerm is string))
                {
                    throw new ArgumentException(LeftTermToBeStringOrExpression);
                }

                if (Operator != Operators.Contains
                    && Operator != Operators.ContainsNot
                    && Operator != Operators.StartsWith
                    && Operator != Operators.EndsWith)
                {
                    //
                    // These operators will add the left term
                    //
                    clause += PreparedFieldname(dbType, LeftTerm.ToString()) + " ";
                }
            }

            switch (Operator)
            {
                #region Process-By-Operators

                case Operators.And:
                    #region And

                    if (RightTerm == null) return null;
                    clause += "AND ";
                    if (RightTerm is WrapperExpression expression)
                    {
                        clause += expression.ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.Or:
                    #region Or

                    if (RightTerm == null) return null;
                    clause += "OR ";
                    if (RightTerm is WrapperExpression expression1)
                    {
                        clause += expression1.ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.Equals:
                    #region Equals

                    if (RightTerm == null) return null;
                    clause += "= ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.NotEquals:
                    #region NotEquals

                    if (RightTerm == null) return null;
                    clause += "<> ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.In:
                    #region In

                    if (RightTerm == null) return null;
                    int inAdded = 0;
                    if (!IsList(RightTerm)) return null;
                    List<object> inTempList = ObjectToList(RightTerm);
                    clause += " IN ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        clause += "(";
                        foreach (object currObj in inTempList)
                        {
                            if (inAdded > 0) clause += ",";
                            if (currObj is DateTime || currObj is DateTime?)
                            {
                                clause += "'" + DbTimestamp(dbType, currObj) + "'";
                            }
                            else if (currObj is int || currObj is long || currObj is decimal)
                            {
                                clause += currObj.ToString();
                            }
                            else
                            {
                                clause += PreparedStringValue(dbType, currObj.ToString());
                            }
                            inAdded++;
                        }
                        clause += ")";
                    }
                    break;

                #endregion

                case Operators.NotIn:
                    #region NotIn

                    if (RightTerm == null) return null;
                    int notInAdded = 0;
                    if (!IsList(RightTerm)) return null;
                    List<object> notInTempList = ObjectToList(RightTerm);
                    clause += " NOT IN ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        clause += "(";
                        foreach (object currObj in notInTempList)
                        {
                            if (notInAdded > 0) clause += ",";
                            if (currObj is DateTime || currObj is DateTime?)
                            {
                                clause += "'" + DbTimestamp(dbType, currObj) + "'";
                            }
                            else if (currObj is int || currObj is long || currObj is decimal)
                            {
                                clause += currObj.ToString();
                            }
                            else
                            {
                                clause += PreparedStringValue(dbType, currObj.ToString());
                            }
                            notInAdded++;
                        }
                        clause += ")";
                    }
                    break;

                #endregion

                case Operators.Contains:
                    #region Contains

                    if (RightTerm == null) return null;
                    if (RightTerm is string)
                    {
                        clause +=
                            "(" +
                            PreparedFieldname(dbType, LeftTerm.ToString()) + " LIKE " + PreparedStringValue(dbType, "%" + RightTerm.ToString()) +
                            " OR " + PreparedFieldname(dbType, LeftTerm.ToString()) + " LIKE " + PreparedStringValue(dbType, "%" + RightTerm.ToString() + "%") +
                            " OR " + PreparedFieldname(dbType, LeftTerm.ToString()) + " LIKE " + PreparedStringValue(dbType, RightTerm.ToString() + "%") +
                            ")";
                    }
                    else
                    {
                        return null;
                    }
                    break;

                #endregion

                case Operators.ContainsNot:
                    #region ContainsNot

                    if (RightTerm == null) return null;
                    if (RightTerm is string)
                    {
                        clause +=
                            "(" +
                            PreparedFieldname(dbType, LeftTerm.ToString()) + " NOT LIKE " + PreparedStringValue(dbType, "%" + RightTerm.ToString()) +
                            " OR " + PreparedFieldname(dbType, LeftTerm.ToString()) + " NOT LIKE " + PreparedStringValue(dbType, "%" + RightTerm.ToString() + "%") +
                            " OR " + PreparedFieldname(dbType, LeftTerm.ToString()) + " NOT LIKE " + PreparedStringValue(dbType, RightTerm.ToString() + "%") +
                            ")";
                    }
                    else
                    {
                        return null;
                    }
                    break;

                #endregion

                case Operators.StartsWith:
                    #region StartsWith

                    if (RightTerm == null) return null;
                    if (RightTerm is string)
                    {
                        clause +=
                            "(" +
                            PreparedFieldname(dbType, LeftTerm.ToString()) + " LIKE " + PreparedStringValue(dbType, RightTerm.ToString() + "%") +
                            ")";
                    }
                    else
                    {
                        return null;
                    }
                    break;

                #endregion

                case Operators.EndsWith:
                    #region EndsWith

                    if (RightTerm == null) return null;
                    if (RightTerm is string)
                    {
                        clause +=
                            "(" +
                            PreparedFieldname(dbType, LeftTerm.ToString()) + " LIKE " + "%" + PreparedStringValue(dbType, RightTerm.ToString()) +
                            ")";
                    }
                    else
                    {
                        return null;
                    }
                    break;

                #endregion

                case Operators.GreaterThan:
                    #region GreaterThan

                    if (RightTerm == null) return null;
                    clause += "> ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.GreaterThanOrEqualTo:
                    #region GreaterThanOrEqualTo

                    if (RightTerm == null) return null;
                    clause += ">= ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.LessThan:
                    #region LessThan

                    if (RightTerm == null) return null;
                    clause += "< ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.LessThanOrEqualTo:
                    #region LessThanOrEqualTo

                    if (RightTerm == null) return null;
                    clause += "<= ";
                    if (RightTerm is WrapperExpression)
                    {
                        clause += ((WrapperExpression)RightTerm).ToWhereClause(dbType);
                    }
                    else
                    {
                        if (RightTerm is DateTime || RightTerm is DateTime?)
                        {
                            clause += "'" + DbTimestamp(dbType, RightTerm) + "'";
                        }
                        else if (RightTerm is int || RightTerm is long || RightTerm is decimal)
                        {
                            clause += RightTerm.ToString();
                        }
                        else
                        {
                            clause += PreparedStringValue(dbType, RightTerm.ToString());
                        }
                    }
                    break;

                #endregion

                case Operators.IsNull:
                    #region IsNull

                    clause += " IS NULL";
                    break;

                #endregion

                case Operators.IsNotNull:
                    #region IsNotNull

                    clause += " IS NOT NULL";
                    break;

                    #endregion

                    #endregion
            }

            clause += ")";

            return clause.Replace("  ", " ");
        }

        /// <summary>
        /// Display Expression in a human-readable string.
        /// </summary>
        /// <returns>String containing human-readable version of the Expression.</returns>
        public override string ToString()
        {
            string ret = "";
            ret += "(";

            if (LeftTerm is WrapperExpression) ret += ((WrapperExpression)LeftTerm).ToString();
            else ret += LeftTerm.ToString();

            ret += " " + Operator.ToString() + " ";

            if (RightTerm is WrapperExpression) ret += ((WrapperExpression)RightTerm).ToString();
            else ret += RightTerm.ToString();

            ret += ")";
            return ret;
        }

        /// <summary>
        /// Prepends a new Expression using the supplied left term, operator, and right term using an AND clause.
        /// </summary>
        /// <param name="left">The left term of the expression; can either be a string term or a nested Expression.</param>
        /// <param name="oper">The operator.</param>
        /// <param name="right">The right term of the expression; can either be an object for comparison or a nested Expression.</param>
        public void PrependAnd(object left, Operators oper, object right)
        {
            WrapperExpression e = new WrapperExpression(left, oper, right);
            PrependAnd(e);
        }

        /// <summary>
        /// Prepends the Expression with the supplied Expression using an AND clause.
        /// </summary>
        /// <param name="prepend">The Expression to prepend.</param> 
        public void PrependAnd(WrapperExpression prepend)
        {
            if (prepend == null) throw new ArgumentNullException(nameof(prepend));

            WrapperExpression orig = new WrapperExpression(this.LeftTerm, this.Operator, this.RightTerm);
            WrapperExpression e = PrependAndClause(prepend, orig);
            LeftTerm = e.LeftTerm;
            Operator = e.Operator;
            RightTerm = e.RightTerm;

            return;
        }

        /// <summary>
        /// Prepends a new Expression using the supplied left term, operator, and right term using an OR clause.
        /// </summary>
        /// <param name="left">The left term of the expression; can either be a string term or a nested Expression.</param>
        /// <param name="oper">The operator.</param>
        /// <param name="right">The right term of the expression; can either be an object for comparison or a nested Expression.</param>
        public void PrependOr(object left, Operators oper, object right)
        {
            WrapperExpression e = new WrapperExpression(left, oper, right);
            PrependOr(e);
        }

        /// <summary>
        /// Prepends the Expression with the supplied Expression using an OR clause.
        /// </summary>
        /// <param name="prepend">The Expression to prepend.</param> 
        public void PrependOr(WrapperExpression prepend)
        {
            if (prepend == null) throw new ArgumentNullException(nameof(prepend));

            WrapperExpression orig = new WrapperExpression(this.LeftTerm, this.Operator, this.RightTerm);
            WrapperExpression e = PrependOrClause(prepend, orig);
            LeftTerm = e.LeftTerm;
            Operator = e.Operator;
            RightTerm = e.RightTerm;

            return;
        }

        /// <summary>
        /// Prepends the Expression in prepend to the Expression original using an AND clause.
        /// </summary>
        /// <param name="prepend">The Expression to prepend.</param>
        /// <param name="original">The original Expression.</param>
        /// <returns>A new Expression.</returns>
        public static WrapperExpression PrependAndClause(WrapperExpression prepend, WrapperExpression original)
        {
            if (prepend == null) throw new ArgumentNullException(nameof(prepend));
            if (original == null) throw new ArgumentNullException(nameof(original));
            WrapperExpression ret = new WrapperExpression
            {
                LeftTerm = prepend,
                Operator = Operators.And,
                RightTerm = original
            };
            return ret;
        }

        /// <summary>
        /// Prepends the Expression in prepend to the Expression original using an OR clause.
        /// </summary>
        /// <param name="prepend">The Expression to prepend.</param>
        /// <param name="original">The original Expression.</param>
        /// <returns>A new Expression.</returns>
        public static WrapperExpression PrependOrClause(WrapperExpression prepend, WrapperExpression original)
        {
            if (prepend == null) throw new ArgumentNullException(nameof(prepend));
            if (original == null) throw new ArgumentNullException(nameof(original));
            WrapperExpression ret = new WrapperExpression
            {
                LeftTerm = prepend,
                Operator = Operators.Or,
                RightTerm = original
            };
            return ret;
        }

        /// <summary>
        /// Convert a List of Expression objects to a nested Expression containing AND between each Expression in the list. 
        /// </summary>
        /// <param name="exprList">List of Expression objects.</param>
        /// <returns>A nested Expression.</returns>
        public static WrapperExpression ListToNestedAndExpression(List<WrapperExpression> exprList)
        {
            if (exprList == null) throw new ArgumentNullException(nameof(exprList));
            if (exprList.Count < 1) return null;

            int evaluated = 0;
            WrapperExpression ret = null;
            WrapperExpression left = null;
            List<WrapperExpression> remainder = new List<WrapperExpression>();

            if (exprList.Count == 1)
            {
                foreach (WrapperExpression curr in exprList)
                {
                    ret = curr;
                    break;
                }

                return ret;
            }
            else
            {
                foreach (WrapperExpression curr in exprList)
                {
                    if (evaluated == 0)
                    {
                        left = new WrapperExpression();
                        left.LeftTerm = curr.LeftTerm;
                        left.Operator = curr.Operator;
                        left.RightTerm = curr.RightTerm;
                        evaluated++;
                    }
                    else
                    {
                        remainder.Add(curr);
                        evaluated++;
                    }
                }

                ret = new WrapperExpression();
                ret.LeftTerm = left;
                ret.Operator = Operators.And;
                WrapperExpression right = ListToNestedAndExpression(remainder);
                ret.RightTerm = right;

                return ret;
            }
        }

        /// <summary>
        /// Convert a List of Expression objects to a nested Expression containing OR between each Expression in the list. 
        /// </summary>
        /// <param name="exprList">List of Expression objects.</param>
        /// <returns>A nested Expression.</returns>
        public static WrapperExpression ListToNestedOrExpression(List<WrapperExpression> exprList)
        {
            if (exprList == null) throw new ArgumentNullException(nameof(exprList));
            if (exprList.Count < 1) return null;

            int evaluated = 0;
            WrapperExpression ret = null;
            WrapperExpression left = null;
            List<WrapperExpression> remainder = new List<WrapperExpression>();

            if (exprList.Count == 1)
            {
                foreach (WrapperExpression curr in exprList)
                {
                    ret = curr;
                    break;
                }

                return ret;
            }
            else
            {
                foreach (WrapperExpression curr in exprList)
                {
                    if (evaluated == 0)
                    {
                        left = new WrapperExpression();
                        left.LeftTerm = curr.LeftTerm;
                        left.Operator = curr.Operator;
                        left.RightTerm = curr.RightTerm;
                        evaluated++;
                    }
                    else
                    {
                        remainder.Add(curr);
                        evaluated++;
                    }
                }

                ret = new WrapperExpression();
                ret.LeftTerm = left;
                ret.Operator = Operators.Or;
                WrapperExpression right = ListToNestedOrExpression(remainder);
                ret.RightTerm = right;

                return ret;
            }
        }

        #endregion

        #region Private-Methods

        private string SanitizeString(DbTypes dbType, string s)
        {
            if (String.IsNullOrEmpty(s)) return String.Empty;
            string ret = "";

            switch (dbType)
            {
                case DbTypes.MsSql:
                    ret = MssqlHelper.SanitizeString(s);
                    break;
                case DbTypes.MySql:
                    ret = MySqlHelper.SanitizeString(s);
                    break;
                case DbTypes.PgSql:
                    ret = PgsqlHelper.SanitizeString(s);
                    break;
            }

            return ret;
        }

        private string PreparedFieldname(DbTypes dbType, string s)
        {
            switch (dbType)
            {
                case DbTypes.MsSql:
                    return s;

                case DbTypes.MySql:
                    return s;

                case DbTypes.PgSql:
                    return "\"" + s + "\"";
            }

            return null;
        }

        private string PreparedStringValue(DbTypes dbType, string s)
        {
            switch (dbType)
            {
                case DbTypes.MsSql:
                    return "'" + MssqlHelper.SanitizeString(s) + "'";

                case DbTypes.MySql:
                    return "'" + MySqlHelper.SanitizeString(s) + "'";

                case DbTypes.PgSql:
                    // uses $xx$ escaping
                    return PgsqlHelper.SanitizeString(s);
            }

            return null;
        }

        private string PreparedUnicodeValue(DbTypes dbType, string s)
        {
            switch (dbType)
            {
                case DbTypes.MsSql:
                    return "N" + PreparedStringValue(dbType, s);

                case DbTypes.MySql:
                    return "N" + PreparedStringValue(dbType, s);

                case DbTypes.PgSql:
                    return "U&" + PreparedStringValue(dbType, s);
            }

            return null;
        }

        private string DbTimestamp(DbTypes dbType, object ts)
        {
            DateTime dt = DateTime.Now;
            if (ts == null) return null;
            if (ts is DateTime?) dt = Convert.ToDateTime(ts);
            else if (ts is DateTime) dt = (DateTime)ts;

            switch (dbType)
            {
                case DbTypes.MsSql:
                    return dt.ToString("MM/dd/yyyy hh:mm:ss.fffffff tt");

                case DbTypes.MySql:
                    return dt.ToString("yyyy-MM-dd HH:mm:ss.ffffff");

                case DbTypes.PgSql:
                    return dt.ToString("MM/dd/yyyy hh:mm:ss.fffffff tt");

                default:
                    return null;
            }
        }

        #endregion


        /// <summary>
        /// Determina si el objeto es una lista.
        /// </summary>
        /// <param name="o">Un objeto.</param>
        /// <returns>Verdadero indica que el objeto es una lista.</returns>
        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }


        /// <summary>
        /// Converts an object to a List object.
        /// </summary>
        /// <param name="o">An object.</param>
        /// <returns>A List object.</returns>
        public static List<object> ObjectToList(object o)
        {
            if (o == null) return null;
            List<object> ret = new List<object>();
            var enumerator = ((IEnumerable)o).GetEnumerator();
            while (enumerator.MoveNext())
            {
                ret.Add(enumerator.Current);
            }
            return ret;
        }



    }
}