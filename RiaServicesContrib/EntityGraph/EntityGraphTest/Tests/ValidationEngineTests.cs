using System.ComponentModel.DataAnnotations;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityGraph.Validation;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class ValidationEngineTests : EntityGraphTest
    {
        public class AValidator : ValidationRule<ValidationResult>
        {
            public AValidator()
                : base(new Signature()
                .Depedency<A, string>(A => A.B.name)
                .Depedency<A, string>(A => A.B.C.name))
            { }

            public static ValidationResult Result { get; set; }

            [ValidateMethod]
            public void ValidateMe(string nameOfB, string nameOfC)
            {
                if(nameOfB != nameOfC)
                {
                    Result = new ValidationResult("Invalid names");
                }
                else
                {
                    Result = System.ComponentModel.DataAnnotations.ValidationResult.Success;
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
        }
        [TestMethod]
        public void AValidatorTest() {
            var gr = a.EntityGraph();
            AValidator.Result = ValidationResult.Success;
            b.name = "hello";
            var validator = new ValidationEngine<Entity, ValidationResult>(new MEFValidationRulesProvider<ValidationResult>());
            validator.Validate(b, "name", gr);
            Assert.IsFalse(AValidator.Result == ValidationResult.Success);
        }
        [TestMethod]
        public void AValidatorTest2() {
            var gr = a.EntityGraph();
            AValidator.Result = ValidationResult.Success;
            b.name = "hello";
            var validator = new ValidationEngine<Entity, ValidationResult>(new MEFValidationRulesProvider<ValidationResult>());
            validator.Validate(b, "name", gr);
            Assert.IsFalse(AValidator.Result == ValidationResult.Success);
            c.name = b.name;
            validator.Validate(c, "name", gr);
            Assert.IsTrue(AValidator.Result == ValidationResult.Success);
        }
    }
}
