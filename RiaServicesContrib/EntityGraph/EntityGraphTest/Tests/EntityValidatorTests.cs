using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using EntityGraph;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
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
    }
}
