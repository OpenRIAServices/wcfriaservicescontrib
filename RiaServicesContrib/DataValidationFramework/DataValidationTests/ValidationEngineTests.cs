using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DataValidation;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace DataValidationTests
{

    [TestClass]
    public class ValidationEngineTests : DataValidationTest
    {
        public class AValidator : ValidationRule<ValidationResult>
        {
            public AValidator()
                : base(
                    InputOutput<A, string>(A => A.B.name),
                    InputOutput<A, string>(A => A.B.C.name)
                )
            { }

            public static ValidationResult TestResult { get; set; }

            [ValidateMethod]
            public ValidationResult ValidateMe(string nameOfB, string nameOfC)
            {
                if(nameOfB != nameOfC)
                {
                    TestResult = new ValidationResult("Invalid names");
                    return ValidationResult.Success;
                }
                else
                {
                    TestResult = ValidationResult.Success;
                    return ValidationResult.Success;
                }
            }
        }
        public class PermutationsOneParameterValidator : ValidationRule<ValidationResult>
        {
            public PermutationsOneParameterValidator() :
                base(InputOutput<A, string>(A => A.name)) 
            { }
            public ValidationResult Validate(string arg)
            {
                return new ValidationResult(arg);
            }
        }
        public class PermutationsTwoParameterValidator : ValidationRule<ValidationResult>
        {
            public PermutationsTwoParameterValidator() :
                base(                
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(A => A.lastName)
                ) 
            { }
            public ValidationResult Validate(string arg1, string arg2)
            {
                return new ValidationResult(arg1);
            }
        }
        public class PermutationsTwoParameterNamesValidator : ValidationRule<ValidationResult>
        {
            public PermutationsTwoParameterNamesValidator() :
                base(
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(B => B.name)
                ) 
            { }
            public ValidationResult Validate(string arg1, string arg2)
            {
                return new ValidationResult(arg1 + arg2);
            }
        }
        public class PermutationsThreeParameterNamesValidator : ValidationRule<ValidationResult>
        {
            public PermutationsThreeParameterNamesValidator() :
                base(
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(B => B.name),
                    InputOutput<A, string>(C => C.name)
                ) 
            { }
            public ValidationResult Validate(string arg1, string arg2, string arg3)
            {
                return new ValidationResult(arg1 + arg2 + arg3);
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
        public void AValidatorTest()
        {
            AValidator.TestResult = ValidationResult.Success;
            b.name = "hello";
            var validator = new ValidationEngine { RulesProvider = new MEFValidationRulesProvider<ValidationResult>() };
            validator.Validate(b, "name", new List<Entity> { a, b, c, d });
            Assert.IsFalse(AValidator.TestResult == ValidationResult.Success);
        }
        [TestMethod]
        public void AValidatorTest2()
        {
            AValidator.TestResult = ValidationResult.Success;
            b.name = "hello";
            var validator = new ValidationEngine { RulesProvider = new MEFValidationRulesProvider<ValidationResult>() };
            validator.Validate(b, "name", new List<Entity> { a, b, c, d });
            Assert.IsFalse(AValidator.TestResult == ValidationResult.Success);
            c.name = b.name;
            validator.Validate(c, "name", new List<Entity> { a, b, c, d });
            Assert.IsTrue(AValidator.TestResult == ValidationResult.Success);
        }

        [Description("Test that a validation rule is invoked for any matching object.")]
        [TestMethod]
        public void PermutationsOneParameterValidatorTest()
        {
            var a1 = new A { name = "a1" };
            var a2 = new A { name = "a2" };
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsOneParameterValidator() } };
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(a1.ValidationErrors.Count == 1);
            Assert.IsTrue(a2.ValidationErrors.Count == 1);
        }
        [Description("Test that a validation rule is invoked for any matching object. Should't make a difference in case of two parameters with the same name")]
        [TestMethod]
        public void PermutationsTwoParameterValidatorTest()
        {
            var a1 = new A { name = "a1" };
            var a2 = new A { name = "a2" };
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsTwoParameterValidator() } };
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(a1.ValidationErrors.Count == 1);
            Assert.IsTrue(a2.ValidationErrors.Count == 1);
        }
        [Description("Test permutations in case of two different parameter names")]
        [TestMethod]
        public void PermutationsTwoParameterNamesValidatorTest()
        {
            var a1 = new A { name = "a1" };
            var a2 = new A { name = "a2" };
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsTwoParameterNamesValidator() } };

            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(a1.ValidationErrors.Count == 2);
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a1a2"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a2a1"));
            Assert.IsTrue(a2.ValidationErrors.Count == 2);
            Assert.IsTrue(a2.ValidationErrors.Any(e => e.ErrorMessage == "a1a2"));
            Assert.IsTrue(a2.ValidationErrors.Any(e => e.ErrorMessage == "a2a1"));
        }
        [Description("Test permutations in case of three different parameter names with two objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest1()
        {
            var a1 = new A { name = "a1" };
            var a2 = new A { name = "a2" };
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsThreeParameterNamesValidator() } };
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(a1.ValidationErrors.Count == 0);
            Assert.IsTrue(a2.ValidationErrors.Count == 0);
        }
        [Description("Test permutations in case of three different parameter names with three objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest2()
        {
            var a1 = new A { name = "a1" };
            var a2 = new A { name = "a2" };
            var a3 = new A { name = "a3" };
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsThreeParameterNamesValidator() } };
            validator.Validate(new List<Entity> { a1, a2, a3 });
            Assert.IsTrue(a1.ValidationErrors.Count == 6);
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a1a2a3"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a1a3a2"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a2a1a3"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a2a3a1"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a3a1a2"));
            Assert.IsTrue(a1.ValidationErrors.Any(e => e.ErrorMessage == "a3a2a1"));
            
            Assert.IsTrue(a2.ValidationErrors.Count == 6);
            Assert.IsTrue(a3.ValidationErrors.Count == 6);
        }
        [TestMethod]
        public void SingleEntityValidation1()
        {
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new AValidator() } };
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(a, "name");
            Assert.IsTrue(AValidator.TestResult == ValidationResult.Success);
        }
        [TestMethod]
        public void SingleEntityValidation2()
        {
            var a = new A();
            var validator = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { new PermutationsOneParameterValidator() } };
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(a, "name");
            Assert.IsTrue(a.ValidationErrors.Count() == 1);
        }
    }
}
