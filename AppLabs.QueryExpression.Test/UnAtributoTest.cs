using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppLabs.QueryExpression.Test
{
    [TestClass]
    public class UnAtributoTest
    {
        [TestMethod]
        public void Igual_UnSoloAtributo()
        {
            string esperado = "(IdTema = 1)";

            var wrapper = new WrapperExpression("IdTema", Operators.Equals, 1);
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }

        [TestMethod]
        public void Diferente_UnSoloAtributo()
        {
            string esperado = "(IdTema <> 1)";

            var wrapper = new WrapperExpression("IdTema", Operators.NotEquals, 1);
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }


        [TestMethod]
        public void In_UnSoloAtributo()
        {
            string esperado = "(IdTema IN (1,2,4,5))";

            var wrapper = new WrapperExpression("IdTema", Operators.In, new List<int>{ 1,2,4,5 });
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }


        [TestMethod]
        public void In_Texto_UnSoloAtributo()
        {
            string esperado = "(IdTema IN ('Rojo','Verde','Blanco','Azul'))";

            var wrapper = new WrapperExpression("IdTema", Operators.In, new List<string> {  "Rojo", "Verde", "Blanco", "Azul" });
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }


        [TestMethod]
        public void NotIn_UnSoloAtributo()
        {
            string esperado = "(IdTema NOT IN (1,2,4,5))";

            var wrapper = new WrapperExpression("IdTema", Operators.NotIn, new List<int> { 1, 2, 4, 5 });
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }


        [TestMethod]
        public void Contains_UnSoloAtributo()
        {
            string esperado = "((IdTema LIKE '%34' OR IdTema LIKE '%34%' OR IdTema LIKE '34%'))";

            var wrapper = new WrapperExpression("IdTema", Operators.Contains, "34");
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }


        [TestMethod]
        public void NotContains_UnSoloAtributo()
        {
            string esperado = "((IdTema NOT LIKE '%34' OR IdTema NOT LIKE '%34%' OR IdTema NOT LIKE '34%'))";

            var wrapper = new WrapperExpression("IdTema", Operators.ContainsNot, "34");
            string where = wrapper.ToWhereClause();

            Assert.AreEqual(esperado, where);
        }
    }
}
