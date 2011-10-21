﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;


namespace Test
{
    public class E
    {
        public int Id { get; set; }

        public F F { get; set; }
        public Nullable<int> FId { get; set; }
    }
    public class F
    {
        public int Id { get; set; }
        public List<E> ESet { get; set; }
    }

    public class GH
    {
        public int GId { get; set; }

        public G G { get; set; }

        public int HId { get; set; }
        public H H { get; set; }

    }
    public class G
    {
        public int Id { get; set; }
        public List<GH> GHSet { get; set; }
    }

    public class H
    {
        public int Id { get; set; }
        public List<GH> GHSet { get; set; }
        public string Name { get; set; }
    }
}

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class CopyToTests : EntityGraphTest
    {
        [TestMethod]
        [Description("Tests importing a shape of entities in namespace 'Test' into a shape of entities in namespace EntityGraphTests.Web (with base class Entity).")]
        public void CopyToTest1()
        {
            var shape = new EntityGraphShape().Edge<G, GH>(x => x.GHSet);

            var g = new Test.G { GHSet = new List<Test.GH> { new Test.GH() } };
            var result = shape.CopyTo<object, Entity>(g, new AssemblyTypeMapper<H>());

            Assert.IsTrue(result != null);
            Assert.IsTrue(result is G);
            Assert.IsTrue(((G)result).GHSet.Count() == g.GHSet.Count());
        }
        [TestMethod]
        [Description("Tests exporting a shape of objects in namespace EntityGraphTests.Web (with base class Entity) to a shape of entities in namespace 'Test'.")]
        public void CopyToTest2()
        {
            var shape = new EntityGraphShape().Edge<G, GH>(x => x.GHSet);

            var g = new G();
            g.GHSet.Add(new GH());

            var result = shape.CopyTo<Entity, object>(g, new AssemblyTypeMapper<Test.E>());

            Assert.IsTrue(result != null);
            Assert.IsTrue(result is Test.G);
            Assert.IsTrue(((Test.G)result).GHSet.Count() == g.GHSet.Count());
        }
    }
}
