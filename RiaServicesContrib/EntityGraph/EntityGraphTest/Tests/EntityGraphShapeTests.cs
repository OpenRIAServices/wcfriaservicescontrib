using System.Linq;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;
using System;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntityGraphShapeTests : EntityGraphTest
    {
        [TestMethod]
        public void MyTestMethod()
        {
        }

        [TestMethod]
        public void SimpleGraphShapeTests()
        {
            var newB = new B { name = "NewB" };
            a.BSet.Add(newB);
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<A, B>(A => A.BSet);

            var gr = a.EntityGraph(shape);

            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(newB));
        }
        [TestMethod]
        public void CopyEqualGraphShapeTests()
        {
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<B,C>(B => B.C)
            .Edge<C,D>(C => C.D)
            .Edge<A, B>(A => A.BSet);

            var copy1 = a.Copy(shape);
            var copy2 = a.Copy(EntityGraphs.CircularGraphFull);

            var gr1 = copy1.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = copy2.EntityGraph(EntityGraphs.CircularGraphFull);

            Assert.IsTrue(gr1.IsCopyOf(gr2));
        }

        [TestMethod]
        public void CopyNotEqualGraphShapeTests()
        {
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<A, D>(A => A.DSet)
            .Edge<A, B>(A => A.BSet);

            var copy1 = a.Copy(shape);
            var copy2 = a.Copy(EntityGraphs.CircularGraphFull);

            var gr1 = copy1.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = copy2.EntityGraph(EntityGraphs.CircularGraphFull);

            Assert.IsFalse(gr1.IsCopyOf(gr2));
        }
        [TestMethod]
        public void InvalidPathExpressionTest1()
        {
            bool isValidExpression = true;
            try
            {
                var shape = new EntityGraphShape()
                .Edge<A, D>(A => A.B.C.D);
            }
            catch(Exception)
            {
                isValidExpression = false;
            }
            Assert.IsFalse(isValidExpression);
        }
        [TestMethod]
        public void InvalidPathExpressionTest2()
        {
            bool isValidExpression = true;
            try
            {
                var shape = new EntityGraphShape()
                .Edge<A, B>(A => A.BSet.First());
            }
            catch(Exception)
            {
                isValidExpression = false;
            }
            Assert.IsFalse(isValidExpression);
        }
        [TestMethod]
        public void InvalidPathExpressionTest3()
        {
            bool isValidExpression = true;
            try
            {
                var shape = new EntityGraphShape()
                .Edge<A, A>(A => A.B.ASet);
            }
            catch(Exception)
            {
                isValidExpression = false;
            }
            Assert.IsFalse(isValidExpression);
        }
        [TestMethod]
        public void EntitiyGraphShapeUnionTest()
        {
            var shape1 = new EntityGraphShape()
                .Edge<A, D>(A => A.DSet)
                .Edge<A, B>(A => A.B);
            var shape2 = new EntityGraphShape()
                .Edge<A, D>(A => A.DSet)
                .Edge<A, B>(A => A.B)
                .Edge<C, D>(C => C.D);
            var shape3 = shape1.Union(shape2);


            Assert.IsTrue(shape1.All(edge => shape3.Contains(edge)));
            Assert.IsTrue(shape2.All(edge => shape2.Contains(edge)));

            Assert.IsFalse(shape3.Any(edge => shape1.Contains(edge) == false && shape2.Contains(edge) == false));
            Assert.IsTrue(shape3.Count() == 3);
        }
    }
}
