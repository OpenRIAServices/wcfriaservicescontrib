using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using EntityGraph;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;

namespace EntityGraphTest.Tests
{
    public class AValidator : GraphValidationRule<A>
    {
        public static bool IsValidated = false;


        public override ValidationRuleDependencies<EntityGraph<A, Entity, ValidationResult>> Signature
        {
            get {
                return new ValidationRuleDependencies<EntityGraph<A, Entity, ValidationResult>>
                {
                    a => a.Source.B.name,
                    a => a.Source.B.C.name
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
        
        public void MyValidate(EntityGraph<A> graph) {
            IsValidated = true;
            if(graph.Source.B.name != graph.Source.B.C.name)
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
        [TestMethod]
        public void AValidatorTest() {
            var gr = a.EntityGraph();
            MEFValidationRules.RegisterType(typeof(AValidator));
            AValidator.IsValidated = false;
            b.name = "hello";
            Assert.IsTrue(b.HasValidationErrors);
            Assert.IsTrue(AValidator.IsValidated);
        }
        [TestMethod]
        public void AValidatorTest2() {
            var gr = a.EntityGraph();
            MEFValidationRules.RegisterType(typeof(AValidator));
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
