using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Xml.Serialization;

namespace EntityGraphTest.Web
{
    public class A
    {
        [EntityGraph("MyGraph")]
        [Association("B_A", "BId", "Id", IsForeignKey = true)]
        [XmlIgnore()]
        public B B { get; set; }

        [DataMember()]
        public Nullable<int> BId { get; set; }

        [EntityGraph]
        [Association("A_B", "Id", "AId")]
        [XmlIgnore()]
        public List<B> BSet { get; set; }

        [Association("A_D", "Id", "AId")]
        [XmlIgnore()]
        public List<D> DSet { get; set; }

        [DataMember()]
        [Key()]
        public int Id { get; set; }

        [DataMember()]
        [Required()]
        public string name { get; set; }
    }

    public class B
    {
        [EntityGraph]
        [Association("A_B", "AId", "Id", IsForeignKey = true)]
        [XmlIgnore()]
        public A A { get; set; }

        [DataMember()]
        [RoundtripOriginal()]
        public Nullable<int> AId { get; set; }

        [Association("B_A", "Id", "BId")]
        [XmlIgnore()]
        public List<A> ASet { get; set; }

        [EntityGraph("MyGraph")]
        [Association("C_B", "CId", "Id", IsForeignKey = true)]
        [XmlIgnore()]
        public C C { get; set; }

        [DataMember()]
        public Nullable<int> CId { get; set; }

        [DataMember()]
        [Key()]
        public int Id { get; set; }

        [DataMember()]
        public string name { get; set; }

    }

    public class C
    {
        [Association("C_B", "Id", "CId")]
        [XmlIgnore()]
        public List<B> BSet { get; set; }

        [EntityGraph("MyGraph")]
        [Association("D_C", "DId", "Id", IsForeignKey = true)]
        [XmlIgnore()]
        public D D { get; set; }

        [DataMember()]
        public Nullable<int> DId { get; set; }

        [DataMember()]
        [Key()]
        public int Id { get; set; }

        [DataMember()]
        [Required()]
        public string name { get; set; }
    }

    public class D
    {
        [EntityGraph("MyGraph")]
        [Association("A_D", "AId", "Id", IsForeignKey = true)]
        [XmlIgnore()]
        public A A { get; set; }

        [DataMember()]
        public Nullable<int> AId { get; set; }

        [Association("D_C", "Id", "DId")]
        [XmlIgnore()]
        public List<C> CSet { get; set; }

        [DataMember()]
        [Key()]
        public int Id { get; set; }

        [DataMember()]
        public string name { get; set; }
    }

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