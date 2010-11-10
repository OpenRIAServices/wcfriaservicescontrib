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
    public class EntityValidatorTest : GraphValidationRule<A>
    {
        public override ValidationRuleDependencies<EntityGraph<A, Entity, ValidationResult>> Signature {
            get {
                return new ValidationRuleDependencies<EntityGraph<A, Entity, ValidationResult>>
                {
                    a => a.Source.B.name,
                    a => a.Source.name
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
        [TestMethod]
        public void EntityValidatorSimpleTest() {
            var gr = a.EntityGraph();
            var v = new EntityValidatorTest();
            v.InvokeValidate(gr);
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
            v.InvokeValidate(gr);
            Assert.IsTrue(v.Result != null);
        }
        [TestMethod]
        public void EntityValidatorTest()
        {
            MEFValidationRules.RegisterType(typeof(EntityValidatorTest1));
            var validator = new EntityValidator<A>(new MEFValidationRulesProvider<A, ValidationResult>(), a);

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
