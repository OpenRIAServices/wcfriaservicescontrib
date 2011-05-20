using RiaServicesContrib.DomainServices.Client;
using RiaServicesContrib;
using EntityGraphTest.Web;

namespace EntityGraphTest
{
    public static class EntityGraphs
    {
        public static EntityGraphShape SimpleGraphShape1 //MyGraph
        {
            get
            {
                return new EntityGraphShape()
                .Edge<E, F>(E => E.F)
                .Edge<F, E>(F => F.ESet);
            }
        }
        public static EntityGraphShape SimpleGraphShape2
        {
            get
            {
                return new EntityGraphShape()
                .Edge<F, E>(F => F.ESet)
                .Edge<GH, G>(GH => GH.G)
                .Edge<G, GH>(G => G.GHSet);
            }
        }
        public static EntityGraphShape SimpleGraphShape3
        {
            get
            {
                return new EntityGraphShape()
                .Edge<GH, H>(GH => GH.H)
                .Edge<H, GH>(H => H.GHSet);
            }
        }
        public static EntityGraphShape SimpleGraphShapeFull
        {
            get
            {
                return new EntityGraphShape()
                .Edge<E, F>(E => E.F)
                .Edge<F, E>(F => F.ESet)
                .Edge<GH, G>(GH => GH.G)
                .Edge<GH, H>(GH => GH.H)
                .Edge<G, GH>(G => G.GHSet)
                .Edge<H, GH>(H => H.GHSet);
            }
        }
        public static EntityGraphShape CircularGraphShape1 // MyGraph
        {
            get
            {
                return new EntityGraphShape()
                .Edge<A, B>(A => A.B)
                .Edge<B, C>(B => B.C)
                .Edge<C, D>(C => C.D)
                .Edge<D, A>(D => D.A);
            }
        }
        public static EntityGraphShape CircularGraphFull
        {
            get
            {
                return new EntityGraphShape()
                .Edge<A, B>(A => A.B)
                .Edge<A, B>(A => A.BSet)
                .Edge<B, A>(B => B.A)
                .Edge<B, C>(B => B.C)
                .Edge<C, D>(C => C.D)
                .Edge<D, A>(D => D.A);
            }
        }
    }
}
