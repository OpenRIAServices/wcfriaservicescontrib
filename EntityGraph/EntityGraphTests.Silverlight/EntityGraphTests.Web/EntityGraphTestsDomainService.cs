using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;

namespace EntityGraphTests.Web
{
    [EnableClientAccess()]
    public partial class EntityGraphTestsDomainService : DomainService
    {
        public IQueryable<A> GetASet() { return null; }
        public void InsertA(A a) { }
        public void UpdateA(A a) { }
        public List<B> GetBSet() { return new List<B> { new B { Id = 1, AId = -22, A = new A { Id = -22 }, CId = -23, C = new C { Id = -23 } } }; }
        public void InsertB(B b) { }
        public void UpdateB(B b) { }
        public void DeleteB(B b) { }
        public IQueryable<C> GetCSet() { return null; }
        public void InsertC(C c) { }
        public void UpdateC(C c) { }
        public void DeleteC(C c) { }
        public IQueryable<D> GetDSet() { return null; }
        public void InsertD(D d) { }
        public void UpdateD(D d) { }

        public IQueryable<E> GetESet() { return null; }
        public void InsertE(E e) { }
        public void UpdateE(E e) { }
        public IQueryable<F> GetFSet() { return null; }
        public void InsertF(F f) { }
        public void UpdateF(F f) { }
        public IQueryable<G> GetGSet() { return null; }
        public void InsertG(G g) { }
        public void UpdateG(G g) { }
        public IQueryable<H> GetHSet() { return null; }
        public void InsertH(H H) { }
        public void UpdateH(H h) { }
        public IQueryable<GH> GetGHSet() { return null; }
        public void InsertGH(GH gh) { }
        public void UpdateGH(GH gh) { }
        public IQueryable<I> GetISet() { return null; }
    }
}