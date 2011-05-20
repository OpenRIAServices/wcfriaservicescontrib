using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing;
using EntityToolsTests.Web;
using System.Linq;
using RiaServicesContrib;
using RiaServicesContrib.Extensions;

namespace EntityToolsTests
{
    [TestClass]
    public class EntityToolsTests : SilverlightTest
    {
        [Asynchronous]
        [TestMethod]
        public void EntityCloneTest()
        {
            var ctx = new EntityToolsDomainContext();
            var loadOp = ctx.Load(ctx.GetPersonSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    var existingPerson = loadOp.Entities.First();
                    existingPerson.Name = "new name";
                    Person newPerson = new Person();
                    newPerson.ApplyState(
                        existingPerson.ExtractState(ExtractType.OriginalState),
                        existingPerson.ExtractState(ExtractType.ModifiedState));
                    newPerson.Id = Guid.NewGuid();
                    ctx.Persons.Add(newPerson);
                }
            );
            EnqueueTestComplete();
        }
    }
}
