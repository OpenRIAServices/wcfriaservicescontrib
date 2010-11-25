using System.ComponentModel.DataAnnotations;
using EntityGraph.EntityValidator.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
    public class RegisterUnregisterValidation : ValidationRule<A, ValidationResult>
    {
        public static bool visited = false;
        public override ValidationRuleDependencies<A> Signature
        {
            get
            {
                return new ValidationRuleDependencies<A>
                {
                    A => A.B
                };
            }
        }
        public void Validate(B b)
        {
            visited = true;
        }
    }

    [TestClass]
    public class RegisterUnregisterTest : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            MEFValidationRules.RegisterType(typeof(RegisterUnregisterValidation));
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            MEFValidationRules.UnregisterType(typeof(RegisterUnregisterValidation));
        }
        [TestMethod]
        public void RegisterTest()
        {
            var validator = new EntityValidator<A>(new MEFValidationRulesProvider<A, ValidationResult>(), a);
            RegisterUnregisterValidation.visited = false;

            a.B = null;
            Assert.IsTrue(RegisterUnregisterValidation.visited);
        }
        [TestMethod]
        public void UnregisterTest()
        {
            MEFValidationRules.UnregisterType(typeof(RegisterUnregisterValidation));
            var validator = new EntityValidator<A>(new MEFValidationRulesProvider<A, ValidationResult>(), a);
            RegisterUnregisterValidation.visited = false;

            a.B = null;
            Assert.IsFalse(RegisterUnregisterValidation.visited);
        }
    }
}
