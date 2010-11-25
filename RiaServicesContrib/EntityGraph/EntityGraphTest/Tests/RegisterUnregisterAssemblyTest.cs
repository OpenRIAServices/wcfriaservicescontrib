using System.ComponentModel.DataAnnotations;
using EntityGraph.EntityValidator.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
    public class RegisterUnregisterTest : ValidationRule<A, ValidationResult>
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
    public class RegisterUnregisterAssemblyTest : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            MEFValidationRules.RegisterType(typeof(RegisterUnregisterTest));
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            MEFValidationRules.UnregisterType(typeof(RegisterUnregisterTest));
        }
        [TestMethod]
        public void RegisterTest()
        {
            var validator = new EntityValidator<A>(new MEFValidationRulesProvider<A, ValidationResult>(), a);
            RegisterUnregisterTest.visited = false;

            a.B = null;
            Assert.IsTrue(RegisterUnregisterTest.visited);
        }
        [TestMethod]
        public void UnregisterTest()
        {
            MEFValidationRules.UnregisterType(typeof(RegisterUnregisterTest));
            var validator = new EntityValidator<A>(new MEFValidationRulesProvider<A, ValidationResult>(), a);
            RegisterUnregisterTest.visited = false;

            a.B = null;
            Assert.IsFalse(RegisterUnregisterTest.visited);
        }
    }
}
