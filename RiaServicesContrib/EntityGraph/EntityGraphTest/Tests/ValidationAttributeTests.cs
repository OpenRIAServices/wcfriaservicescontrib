using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityValidator;
using EntityGraph.EntityValidator.RIA;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class ValidationAttributeTests : EntityGraphTest
    {
        public class MyTestClass : Entity
        {
            private int? _intProperty;
            [RIA.EntityValidator.RIA.Required(ErrorMessage="Int parameter {0} is Missing")]
            public int? IntProperty
            {
                get
                {
                    return _intProperty;
                }
                set
                {
                    _intProperty = value;
                    RaiseDataMemberChanged("IntProperty");
                }
            }
            private string _stringProperty;
            [RIA.EntityValidator.RIA.Required(ErrorMessage="String parameter {0} is Missing")]
            public string StringProperty
            {
                get
                {
                    return _stringProperty;
                }
                set
                {
                    _stringProperty = value;
                    RaiseDataMemberChanged("StringProperty");
                }
            }

            private string _regExprProperty;
            [RIA.EntityValidator.RIA.RegularExpression(@"^[a-zA-Z''-'\s]{5,40}$", ErrorMessage = "Regular expression {0} mismatch")]
            public string RegExprProperty
            {
                get
                {
                    return _regExprProperty;
                }
                set
                {
                    _regExprProperty = value;
                    RaiseDataMemberChanged("RegExprProperty");
                }
            }
        }
        [TestMethod]
        public void RequiredAttributeIntTest()
        {
            var entity = new MyTestClass();
            var validatior = new EntityValidator(entity);
            entity.IntProperty = null;
            Assert.IsTrue(entity.HasValidationErrors);
            entity.IntProperty = 0;
            Assert.IsFalse(entity.HasValidationErrors);
        }
        [TestMethod]
        public void RequiredAttributeStringTest()
        {
            var entity = new MyTestClass();
            var validatior = new EntityValidator(entity);
            entity.StringProperty = null;
            Assert.IsTrue(entity.HasValidationErrors);
            entity.StringProperty = "";
            Assert.IsFalse(entity.HasValidationErrors);
        }
        [TestMethod]
        public void RegularExpressionAttributeTest()
        {
            var entity = new MyTestClass();
            var validatior = new EntityValidator(entity);
            entity.RegExprProperty = "abc";
            Assert.IsTrue(entity.HasValidationErrors);
            entity.RegExprProperty = "abcdef";
            Assert.IsFalse(entity.HasValidationErrors);
        }
    }
}
