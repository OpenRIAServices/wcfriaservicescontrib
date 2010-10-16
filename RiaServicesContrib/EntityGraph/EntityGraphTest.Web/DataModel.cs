using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EntityGraphTest.Web
{
    #region Simple associations
    /// <summary>
    /// E      F
    /// .F*--1 .ESet
    /// 
    /// G           GH      H
    /// .GHSet 1--* .G
    ///             .H *--1 .GHSet
    /// </summary>
    public class E
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [EntityGraph("MyGraph")]
        [Association("E_F", "FId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public F F { get; set; }
        [DataMember]
        public Nullable<int> FId { get; set; }
    }
    public class F
    {
        [DataMember]
        [Key]
        public int Id { get; set; }
        [EntityGraph("MyGraph")]
        [EntityGraph("MyGraph1")]
        [Association("E_F", "Id", "FId")]
        [XmlIgnore]
        public List<E> ESet { get; set; }
    }

    public class GH
    {
        [DataMember]
        [Key]
        public int GId { get; set; }

        [EntityGraph("MyGraph1")]
        [Association("G_GH", "GId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public G G { get; set; }

        [DataMember]
        [Key]
        public int HId { get; set; }

        [EntityGraph("MyGraph2")]
        [Association("H_GH", "HId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public H H { get; set; }

    }
    public class G
    {
        [DataMember]
        [Key]
        public int Id { get; set; }
        [EntityGraph("MyGraph1")]
        [Association("G_GH", "Id", "GId")]
        [XmlIgnore]
        public List<GH> GHSet { get; set; }
    }

    public class H
    {
        [DataMember]
        [Key]
        public int Id { get; set; }
        [EntityGraph("MyGraph2")]
        [Association("H_GH", "Id", "HId")]
        [XmlIgnore]
        public List<GH> GHSet { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    #endregion

    #region Circular associations
    /// <summary>
    ///  A                  B      C          D
    ///  .B           *--1 .ASet
    ///  .BNotInGraph *--1
    ///  .BSet        1--* .A    
    ///  .DSet        1--*                    .A 
    ///                    .C *--1 .BSet
    ///                            .D    *--1 .CSet
    /// </summary>
    public class A
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [EntityGraph("MyGraph")]
        [Association("B_A", "BId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public B B { get; set; }

        [DataMember]
        public Nullable<int> BId { get; set; }

        [Association("BNotInGraph_A", "BNotInGraphId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public B BNotInGraph { get; set; }

        [DataMember]
        public Nullable<int> BNotInGraphId { get; set; }

        [EntityGraph]
        [Association("A_B", "Id", "AId")]
        [XmlIgnore]
        public List<B> BSet { get; set; }

        [Association("A_D", "Id", "AId")]
        [XmlIgnore]
        public List<D> DSet { get; set; }

        [DataMember]
        [Required]
        public string name { get; set; }

        [DataMember]
        [Required]
        public string lastName { get; set; }
    }

    public class B
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [EntityGraph]
        [Association("A_B", "AId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public A A { get; set; }

        [DataMember]
        [RoundtripOriginal]
        public Nullable<int> AId { get; set; }

        [Association("B_A", "Id", "BId")]
        [XmlIgnore]
        public List<A> ASet { get; set; }

        [EntityGraph("MyGraph")]
        [Association("C_B", "CId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public C C { get; set; }

        [DataMember]
        public Nullable<int> CId { get; set; }

        [DataMember]
        public string name { get; set; }
    }

    public class C
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [Association("C_B", "Id", "CId")]
        [XmlIgnore]
        public List<B> BSet { get; set; }

        [EntityGraph("MyGraph")]
        [Association("D_C", "DId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public D D { get; set; }

        [DataMember]
        public Nullable<int> DId { get; set; }

        [DataMember]
        [Required]
        public string name { get; set; }
    }

    public class D
    {
        [DataMember]
        [Key]
        public int Id { get; set; }

        [EntityGraph("MyGraph")]
        [Association("A_D", "AId", "Id", IsForeignKey = true)]
        [XmlIgnore]
        public A A { get; set; }

        [DataMember]
        public Nullable<int> AId { get; set; }

        [Association("D_C", "Id", "DId")]
        [XmlIgnore]
        public List<C> CSet { get; set; }

        [DataMember]
        public string name { get; set; }
    }
    #endregion
}