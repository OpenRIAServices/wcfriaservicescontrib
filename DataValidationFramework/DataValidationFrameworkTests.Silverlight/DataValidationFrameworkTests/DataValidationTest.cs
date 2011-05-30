using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataValidationTests
{
    public abstract class DataValidationTest : SilverlightTest
    {
        protected A a;
        protected B b;
        protected C c;
        protected D d;
        [TestInitialize]
        virtual public void TestSetup()
        {
            a = new A { name = "A" };
            b = new B { name = "B" };
            c = new C { name = "C" };
            d = new D { name = "D" };

            a.B = b;
            b.C = c;
            c.D = d;
            d.A = a;
        }
        [TestCleanup]
        virtual public void TestCleanup()
        {
        }
    }
}
