using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using EntityGraph;
using EntityGraph.EntityValidator.RIA;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;
using System.Diagnostics;

namespace EntityGraphTest.Tests
{
    public class EntityValidatorTest1 : ValidationRule<A, ValidationResult>
    {
        public static bool visited = false;
        public override ValidationRuleDependencies<A> Signature
        {
            get
            {
                return new ValidationRuleDependencies<A>
                {
                    A => A.BSet,
                    A => A.B
                };
            }
        }
        public void Validate(IEnumerable<B> bset, B b){
            visited = true;
        }
    }
    public class EntityValidatorTest : ValidationRule<A, ValidationResult>
    {
        public override ValidationRuleDependencies<A> Signature {
            get {
                return new ValidationRuleDependencies<A>
                {
                    a => a.B.name,
                    a => a.name
                };
            }
        }

        [ValidateMethod]
        public void MyValidate(string nameofB, string nameOfA) {
            Result = new ValidationResult("Yes");
        }
    }

    [TestClass]
    public class EntityValidatorTests : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            MEFValidationRules.RegisterType(typeof(EntityValidatorTest1));
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            MEFValidationRules.UnregisterType(typeof(EntityValidatorTest1));
        }
        [TestMethod]
        public void EntityValidatorSimpleTest() {
            var gr = a.EntityGraph();
            var v = new EntityValidatorTest();
            v.InvokeValidate(gr.Source);
            Assert.IsTrue(v.Result != null);
        }
        [TestMethod]
        public void EntityValidatorValidationResultChangedTest() {
            var gr = a.EntityGraph();
            var v = new EntityValidatorTest();
            v.ValidationResultChanged += (sender, args) =>
            {
                Assert.IsTrue(sender == v);
                Assert.IsTrue(args.Result != null);
            };
            v.InvokeValidate(gr.Source);
            Assert.IsTrue(v.Result != null);
        }
        [TestMethod]
        public void EntityValidatorTest()
        {
            var validator = new MEFEntityValidator<A>(a);

            var b = new B();

            EntityValidatorTest1.visited = false;
            a.BSet.Add(b);
            Assert.IsTrue(EntityValidatorTest1.visited);
            EntityValidatorTest1.visited = false;
            a.B = b;
            Assert.IsTrue(EntityValidatorTest1.visited);
        }
    }
}
