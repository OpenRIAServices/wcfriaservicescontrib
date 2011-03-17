using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.Validation;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class ValidationEngineTests : EntityGraphTest
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
            public void ValidateMe(string nameOfB, string nameOfC)
            {
                if(nameOfB != nameOfC)
                {
                    TestResult = new ValidationResult("Invalid names");
                }
                else
                {
                    TestResult = ValidationResult.Success;
                }
            }
        }
        public class PermutationsOneParameterValidator : ValidationRule<ValidationResult>
        {
            internal static List<RuleBinding<ValidationResult>> Bindings { get; set; }
            public PermutationsOneParameterValidator() :
                base(InputOutput<A, string>(A => A.name)) 
            { }
            public void Validate(string arg)
            {
                Bindings.Add(RuleBinding);
            }
        }
        public class PermutationsTwoParameterValidator : ValidationRule<ValidationResult>
        {
            internal static List<RuleBinding<ValidationResult>> Bindings { get; set; }
            public PermutationsTwoParameterValidator() :
                base(                
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(A => A.lastName)
                ) 
            { }
            public void Validate(string arg1, string arg2)
            {
                Bindings.Add(RuleBinding);
            }
        }
        public class PermutationsTwoParameterNamesValidator : ValidationRule<ValidationResult>
        {
            internal static List<RuleBinding<ValidationResult>> Bindings { get; set; }
            public PermutationsTwoParameterNamesValidator() :
                base(
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(B => B.name)
                ) 
            { }
            public void Validate(string arg1, string arg2)
            {
                Bindings.Add(RuleBinding);
            }
        }
        public class PermutationsThreeParameterNamesValidator : ValidationRule<ValidationResult>
        {
            internal static List<RuleBinding<ValidationResult>> Bindings { get; set; }
            public PermutationsThreeParameterNamesValidator() :
                base(
                    InputOutput<A, string>(A => A.name),
                    InputOutput<A, string>(B => B.name),
                    InputOutput<A, string>(C => C.name)
                ) 
            { }
            public void Validate(string arg1, string arg2, string arg3)
            {
                Bindings.Add(RuleBinding);
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
            var validator = new ValidationEngine<Entity, ValidationResult>(new MEFValidationRulesProvider<ValidationResult>());
            validator.Validate(b, "name", new List<Entity> { a, b, c, d });
            Assert.IsFalse(AValidator.TestResult == ValidationResult.Success);
        }
        [TestMethod]
        public void AValidatorTest2()
        {
            AValidator.TestResult = ValidationResult.Success;
            b.name = "hello";
            var validator = new ValidationEngine<Entity, ValidationResult>(new MEFValidationRulesProvider<ValidationResult>());
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
            var a1 = new A();
            var a2 = new A();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult>{ new PermutationsOneParameterValidator() });
            PermutationsOneParameterValidator.Bindings = new List<RuleBinding<ValidationResult>>();
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(PermutationsOneParameterValidator.Bindings.Count == 2);
            Assert.IsTrue(PermutationsOneParameterValidator.Bindings.Any(b => b.DependencyBindings[0].TargetOwnerObject == a1));
            Assert.IsTrue(PermutationsOneParameterValidator.Bindings.Any(b => b.DependencyBindings[0].TargetOwnerObject == a2));
        }
        [Description("Test that a validation rule is invoked for any matching object. Should't make a difference in case of two parameters with the same name")]
        [TestMethod]
        public void PermutationsTwoParameterValidatorTest()
        {
            var a1 = new A();
            var a2 = new A();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new PermutationsTwoParameterValidator() });
            PermutationsTwoParameterValidator.Bindings = new List<RuleBinding<ValidationResult>>();
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(PermutationsTwoParameterValidator.Bindings.Count == 2);
            Assert.IsTrue(PermutationsTwoParameterValidator.Bindings.Any(b => b.DependencyBindings[0].TargetOwnerObject == a1));
            Assert.IsTrue(PermutationsTwoParameterValidator.Bindings.Any(b => b.DependencyBindings[0].TargetOwnerObject == a2));
        }
        [Description("Test permutations in case of two different parameter names")]
        [TestMethod]
        public void PermutationsTwoParameterNamesValidatorTest()
        {
            var a1 = new A();
            var a2 = new A();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new PermutationsTwoParameterNamesValidator() });
            PermutationsTwoParameterNamesValidator.Bindings = new List<RuleBinding<ValidationResult>>();

            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(PermutationsTwoParameterNamesValidator.Bindings.Count == 2);
            Assert.IsTrue(PermutationsTwoParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a1 &&
                     b.DependencyBindings[1].TargetOwnerObject == a2
                ));
            Assert.IsTrue(PermutationsTwoParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a2 &&
                     b.DependencyBindings[1].TargetOwnerObject == a1
                ));
        }
        [Description("Test permutations in case of three different parameter names with two objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest1()
        {
            var a1 = new A();
            var a2 = new A();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new PermutationsThreeParameterNamesValidator() });
            PermutationsThreeParameterNamesValidator.Bindings = new List<RuleBinding<ValidationResult>>();
            validator.Validate(new List<Entity> { a1, a2 });
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Count == 0);
        }
        [Description("Test permutations in case of three different parameter names with three objects")]
        [TestMethod]
        public void PermutationsThreeParameterNamesValidatorTest2()
        {
            var a1 = new A();
            var a2 = new A();
            var a3 = new A();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new PermutationsThreeParameterNamesValidator() });
            PermutationsThreeParameterNamesValidator.Bindings = new List<RuleBinding<ValidationResult>>();
            validator.Validate(new List<Entity> { a1, a2, a3 });
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Count == 6);

            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a1 &&
                     b.DependencyBindings[1].TargetOwnerObject == a2 &&
                     b.DependencyBindings[2].TargetOwnerObject == a3
                ));
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a1 &&
                     b.DependencyBindings[1].TargetOwnerObject == a3 &&
                     b.DependencyBindings[2].TargetOwnerObject == a2
                ));
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a2 &&
                     b.DependencyBindings[1].TargetOwnerObject == a1 &&
                     b.DependencyBindings[2].TargetOwnerObject == a3
                ));
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a2 &&
                     b.DependencyBindings[1].TargetOwnerObject == a3 &&
                     b.DependencyBindings[2].TargetOwnerObject == a1
                ));
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a3 &&
                     b.DependencyBindings[1].TargetOwnerObject == a1 &&
                     b.DependencyBindings[2].TargetOwnerObject == a2
                ));
            Assert.IsTrue(PermutationsThreeParameterNamesValidator.Bindings.Any(
                b => b.DependencyBindings[0].TargetOwnerObject == a3 &&
                     b.DependencyBindings[1].TargetOwnerObject == a2 &&
                     b.DependencyBindings[2].TargetOwnerObject == a1
                ));
        }
        [TestMethod]
        public void SingleEntityValidation1()
        {
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new AValidator() });
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(a, "name");
            Assert.IsTrue(AValidator.TestResult == ValidationResult.Success);            
        }
        [TestMethod]
        public void SingleEntityValidation2()
        {
            PermutationsOneParameterValidator.Bindings = new List<RuleBinding<ValidationResult>>();
            var validator = new ValidationEngine<Entity, ValidationResult>(new SimpleValidationRulesProvider<ValidationResult> { new PermutationsOneParameterValidator() });
            AValidator.TestResult = ValidationResult.Success;
            validator.Validate(a, "name");
            Assert.IsTrue(PermutationsOneParameterValidator.Bindings.Count() == 1);
        }
    }
}
