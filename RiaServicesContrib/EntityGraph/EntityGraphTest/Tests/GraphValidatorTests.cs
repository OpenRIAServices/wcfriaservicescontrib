using System.ComponentModel.DataAnnotations;
using EntityGraph.EntityValidator.RIA;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
    public class AValidator : ValidationRule<A>
    {
        public static bool IsValidated = false;

        public override ValidationRuleDependencies<A> Signature
        {
            get {
                return new ValidationRuleDependencies<A>
                {
                    a => a.B.name,
                    a => a.B.C.name
                };
            }
        }
        [ValidateMethod]
        public void ValidateMe(string nameOfB, string nameOfC) {
            IsValidated = true;
            if(nameOfB != nameOfC)
            {
                this.Result = new ValidationResult("Invalid names");
            }
            else
                this.Result = ValidationResult.Success;
        }
    }

    [TestClass]
    public class GraphValidatorTests : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            MEFValidationRules.RegisterType(typeof(AValidator));
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            MEFValidationRules.UnregisterType(typeof(AValidator));
        }
        /// <summary>
        /// Checks if validation framework works correctly when one of the dependency paths
        /// of a valudation rule includes null. In this case the path A.B.C.name includes null,
        /// becasue C equals null.
        /// </summary>
        [TestMethod]
        public void EntityGraphValidatorWithNullElelements()
        {
            A a = new A();
            B b = new B { name = "hi" };
            C c = new C { name = "Hello" };

            a.B = b;

            var gr = a.EntityGraph();
            Assert.IsFalse(b.HasValidationErrors);

            // Now we attach C to the entity graph, which should
            // trigger the validation rule.
            b.C = c;
            Assert.IsTrue(b.HasValidationErrors);
        }
        [TestMethod]
        public void AValidatorTest() {
            var gr = a.EntityGraph();
            AValidator.IsValidated = false;
            b.name = "hello";
            Assert.IsTrue(b.HasValidationErrors);
            Assert.IsTrue(AValidator.IsValidated);
        }
        [TestMethod]
        public void AValidatorTest2() {
            var gr = a.EntityGraph();
            AValidator.IsValidated = false;
            b.name = "hello";
            Assert.IsTrue(b.HasValidationErrors);
            Assert.IsTrue(c.HasValidationErrors);
            c.name = b.name;
            Assert.IsFalse(b.HasValidationErrors);
            Assert.IsFalse(c.HasValidationErrors);
        }
    }
}
