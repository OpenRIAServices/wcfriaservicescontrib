using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Threading;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DataValidation;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace DataValidationTests
{
    public class AsyncValidator : AsyncValidationRule
    {
        public AsyncValidator()
            : base(InputOutput<A, string>(A => A.name)
        )
        { }
        public ValidationOperation Validate(string name)
        {
            var operation = new ValidationOperation();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100 Milliseconds 
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                operation.Result = new ValidationResult(name);
            };
            timer.Start();

            return operation;
        }
    }
    [TestClass]
    public class AsyncValidationTests : DataValidationTest
    {
        [Asynchronous]
        [TestMethod]
        public void AsyncValidationTest()
        {
            var a1 = new A { name = "a1" };
            var validator = new AsyncValidator();

            var vEngine = new ValidationEngine { RulesProvider = new SimpleValidationRulesProvider<ValidationResult> { validator } };
            EnqueueConditional(() => vEngine.EntitiesInError.Count() != 0);
            vEngine.Validate(new List<Entity> { a1 });

            EnqueueCallback(
                () =>
                {
                    Assert.IsTrue(vEngine.EntitiesInError.Single() == a1);
                    Assert.IsTrue(a1.ValidationErrors.Count == 1);
                });
            Assert.IsFalse(a1.ValidationErrors.Count == 1);
            EnqueueTestComplete();
        }
    }
}