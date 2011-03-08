using System.ComponentModel.DataAnnotations;
using EntityGraph.RIA;
using EntityGraph.RIA.EntityValidator;
using EntityGraph.Validation;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{

    [TestClass]
    public class GraphValidatorTests : EntityGraphTest
    {
        public class AValidator : ValidationRule
        {
            public AValidator() :
                base(new Signature()
                .Depedency<A, string>(A => A.B.name)
                .Depedency<A, string>(A => A.B.C.name))
            {
            }

            public static bool IsValidated = false;

            [ValidateMethod]
            public void ValidateMe(string nameOfB, string nameOfC)
            {
                IsValidated = true;
                if(nameOfB != nameOfC)
                {
                    this.ValidationResult = new ValidationResult("Invalid names");
                }
                else
                    this.ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult.Success;
            }
        }
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
