using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;

namespace EntityGraphTest.Web {
    [EnableClientAccess()]
    public partial class EntityGraphTestDomainService : DomainService
    {
        public IQueryable<A> GetASet() { return null; }
        public void InsertA(A a) { }
        public void UpdateA(A a) { }
        public List<B> GetBSet() { return new List<B> { new B { Id = 1 } }; }
        public void InsertB(B b) { }
        public void UpdateB(B b) { }
        public void DeleteB(B b) { }
        public IQueryable<C> GetCSet() { return null; }
        public void InsertC(C c) { }
        public void UpdateC(C c) { }
        public IQueryable<D> GetDSet() { return null; }
        public void InsertD(D d) { }
        public void UpdateD(D d) { }
    }
}