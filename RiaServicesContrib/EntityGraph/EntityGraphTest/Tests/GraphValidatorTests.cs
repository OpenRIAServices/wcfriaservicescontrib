using System.ComponentModel.DataAnnotations;
using EntityGraph.RIA;
using EntityGraph.Validation;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using EntityGraph.RIA.Validation;

namespace EntityGraphTest.Tests
{

    [TestClass]
    public class GraphValidatorTests : EntityGraphTest
    {
        public class AValidator : ValidationRule
        {
            public AValidator() :
                base(new Signature()
                .InputOutput<A, string>(A => A.B.name)
                .InputOutput<A, string>(A => A.B.C.name))
            {
            }

            public static bool IsValidated = false;

            [ValidateMethod]
            public void ValidateMe(string nameOfB, string nameOfC)
            {
                IsValidated = true;
                if(nameOfB != nameOfC)
                {
                    this.Result = new ValidationResult("Invalid names");
                }
                else
                    this.Result = ValidationResult.Success;
            }
        }
        public class MultiPropertyValidator : ValidationRule
        {
            public MultiPropertyValidator() :
                base(
                new Signature()
                .InputOutput<A, string>(A => A.name)
                .InputOutput<A, string>(A => A.lastName)) { }

            public void Validate(string name, string lastName)
            {
                if(name == lastName)
                {
                    this.Result = new ValidationResult("Name and LastName cannot be the same", new string[] { "dummy", "members" });
                }
                else
                {
                    this.Result = ValidationResult.Success;
                }
            }
        }
        public class InputOutputInputOnlyValidator : ValidationRule
        {
            public InputOutputInputOnlyValidator() :
                base(
                new Signature()
                .InputOutput<A, string>(A => A.name)
                .InputOnly<A, string>(A => A.lastName)) { }
            public void Validate(string name, string lastName)
            {
                if(name == lastName)
                {
                    this.Result = new ValidationResult("Name and LastName cannot be the same", new string[] { "dummy", "members" });
                }
                else
                {
                    this.Result = ValidationResult.Success;
                }
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
            MEFValidationRules.UnregisterType(typeof(MultiPropertyValidator));
            MEFValidationRules.UnregisterType(typeof(InputOutputInputOnlyValidator));
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
        [TestMethod]
        public void MultiPropertyValidatorTest()
        {
            MEFValidationRules.RegisterType(typeof(MultiPropertyValidator));
            a.name = "John";
            a.lastName = "John";
            var gr = a.EntityGraph();
            Assert.IsTrue(a.HasValidationErrors);
            var validationError = a.ValidationErrors.Single();
            Assert.IsTrue(validationError.MemberNames.Contains("name"));
            Assert.IsTrue(validationError.MemberNames.Contains("lastName"));
            Assert.IsTrue(validationError.MemberNames.Count() == 2);
            a.lastName = "Doe";
            Assert.IsFalse(a.HasValidationErrors);
            MEFValidationRules.UnregisterType(typeof(MultiPropertyValidator));
        }
        [TestMethod]
        public void InputOutputInputOnlyValidatorTest()
        {
            MEFValidationRules.RegisterType(typeof(InputOutputInputOnlyValidator));
            a.name = "John";
            a.lastName = "John";
            var gr = a.EntityGraph();
            Assert.IsTrue(a.HasValidationErrors);
            var validationError = a.ValidationErrors.Single();
            Assert.IsTrue(validationError.MemberNames.Contains("name"));
            Assert.IsFalse(validationError.MemberNames.Contains("lastName"));
            Assert.IsTrue(validationError.MemberNames.Count() == 1);
            a.lastName = "Doe";
            Assert.IsFalse(a.HasValidationErrors);
            MEFValidationRules.UnregisterType(typeof(InputOutputInputOnlyValidator));
        }
    }
}
