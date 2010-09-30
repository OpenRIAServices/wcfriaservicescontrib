using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;

namespace EntityGraphTest.Web
{
    public class A
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string name { get; set; }
        [EntityGraph]
        [Include]
        [Association("BSet", "Id", "Id")]
        public List<B> BSet { get; set; }

        [EntityGraph]
        [Include]
        [Association("AB", "Id", "Id")]
        public B B { get; set; }
        [EntityGraph]
        [Include]
        [Association("AD", "Id", "Id", IsForeignKey = true)]
        public D D { get; set; }
    }
    public class B
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string name { get; set; }

        [EntityGraph]
        [Include]
        [Association("AB", "Id", "Id", IsForeignKey = true)]
        public A A { get; set; }
        [EntityGraph]
        [Include]
        [Association("BC", "Id", "Id")]
        public C C { get; set; }
    }
    public class C
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string name { get; set; }

        [EntityGraph]
        [Include]
        [Association("BC", "Id", "Id", IsForeignKey = true)]
        public B B { get; set; }
        [EntityGraph]
        [Include]
        [Association("CD", "Id", "Id")]
        public D D { get; set; }
    }
    public class D
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string name { get; set; }

        [EntityGraph]
        [Include]
        [Association("CD", "Id", "Id", IsForeignKey = true)]
        public C C { get; set; }
        [EntityGraph]
        [Include]
        [Association("AD", "Id", "Id")]
        public A A { get; set; }
    }


    // Implements application logic using the CCCEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public partial class EntityGraphTestDomainService : DomainService
    {
        public IQueryable<A> GetASet() { return null; }
        public void InsertA(A a) { }
        public List<B> GetBSet() { return new List<B> { new B{ Id=1} }; }
        public void InsertB(B b) { }
        public void UpdateB(B b) { }
        public void DeleteB(B b) { }
        public IQueryable<C> GetCSet() { return null; }
        public void InsertC(C c) { }
        public IQueryable<D> GetDSet() { return null; }
        public void InsertD(D d) { }
    }
}