using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;
using RiaServicesContrib.DataValidation;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class RegisterUnregisterTest : EntityGraphTest
    {
        public class RegisterUnregisterValidation : ValidationRule<ValidationResult>
        {
            public RegisterUnregisterValidation() :
                base(InputOutput<A, B>(A => A.B))
            { }
            public static bool visited = false;
            public void Validate(B b)
            {
                visited = true;
            }
        }

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
            var validator = new ValidationEngine { RulesProvider = new MEFValidationRulesProvider<ValidationResult>() };
            RegisterUnregisterValidation.visited = false;

            validator.Validate(a, "B", new List<Entity> { a });
            Assert.IsTrue(RegisterUnregisterValidation.visited);
        }
        [TestMethod]
        public void UnregisterTest()
        {
            MEFValidationRules.UnregisterType(typeof(RegisterUnregisterValidation));
            var validator = new ValidationEngine { RulesProvider = new MEFValidationRulesProvider<ValidationResult>() };
            RegisterUnregisterValidation.visited = false;

            validator.Validate(a, "B", new List<Entity> { a });
            Assert.IsFalse(RegisterUnregisterValidation.visited);
        }
    }
}
