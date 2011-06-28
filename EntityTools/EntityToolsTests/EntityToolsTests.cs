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
using System.ComponentModel;
using System.Collections.Generic;

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

        [TestMethod]
        public void Entity_ExtractState_ChangesOnlyTest()
        {
            EntityToolsDomainContext ctx = new EntityToolsDomainContext();

            Class newClass = new Class()
            {
                Name = "Test New Class",
                Description = "This is a new class for the test",
                Id = Guid.NewGuid()
            };

            ctx.Classes.Add(newClass);
            ((IChangeTracking)newClass).AcceptChanges();

            newClass.Name = "My New Name";

            IDictionary<string, object> changedState = newClass.ExtractState(ExtractType.ChangesOnlyState);
            Assert.AreEqual(1, changedState.Count);
            Assert.IsTrue(changedState.ContainsKey("Name"));
            Assert.AreEqual("My New Name", changedState["Name"]);
        }

        [TestMethod]
        public void Entity_ExtractState_ModifiedStateTest()
        {
            EntityToolsDomainContext ctx = new EntityToolsDomainContext();

            Class newClass = new Class()
            {
                Name = "Test New Class",
                Description = "This is a new class for the test",
                Id = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            ctx.Classes.Add(newClass);
            //((IChangeTracking)newClass).AcceptChanges();

            IDictionary<string, object> changedState = newClass.ExtractState(ExtractType.ModifiedState);
            Assert.AreEqual(4, changedState.Count);
            Assert.IsTrue(changedState.ContainsKey("Name"));
            Assert.IsTrue(changedState.ContainsKey("Description"));
            Assert.IsTrue(changedState.ContainsKey("Id"));
            Assert.IsTrue(changedState.ContainsKey("DepartmentId"));
            Assert.AreEqual(newClass.Name, changedState["Name"]);
            Assert.AreEqual(newClass.Id, changedState["Id"]);
            Assert.AreEqual(newClass.Description, changedState["Description"]);
            Assert.AreEqual(newClass.DepartmentId, changedState["DepartmentId"]);
        }

        [TestMethod]
        public void Entity_ExtractState_OriginalStateTest()
        {
            EntityToolsDomainContext ctx = new EntityToolsDomainContext();

            Class newClass = new Class()
            {
                Name = "Test New Class",
                Description = "This is a new class for the test",
                Id = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            ctx.Classes.Add(newClass);
            ((IChangeTracking)newClass).AcceptChanges();

            newClass.Name = "This has been updated";
            newClass.Description = "This is the new description";

            IDictionary<string, object> changedState = newClass.ExtractState(ExtractType.OriginalState);
            Assert.AreEqual(4, changedState.Count);
            Assert.IsTrue(changedState.ContainsKey("Name"));
            Assert.IsTrue(changedState.ContainsKey("Description"));
            Assert.IsTrue(changedState.ContainsKey("Id"));
            Assert.IsTrue(changedState.ContainsKey("DepartmentId"));
            Assert.AreEqual("Test New Class", changedState["Name"]);
            Assert.AreEqual(newClass.Id, changedState["Id"]);
            Assert.AreEqual("This is a new class for the test", changedState["Description"]);
            Assert.AreEqual(newClass.DepartmentId, changedState["DepartmentId"]);
        }
    }
}
