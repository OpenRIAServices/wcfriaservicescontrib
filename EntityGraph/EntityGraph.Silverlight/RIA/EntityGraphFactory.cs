using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    /// <summary>
    /// Class that implements a cache for instantiated entity graphs using weak references.
    /// This is a WCF DomainServices.Client services specific instantiation of the 
    /// RiaServicesContrib.EntityGraphFactory class.
    /// </summary>
    public class EntityGraphFactory : EntityGraphFactory<Entity>
    {
        /// <summary>
        /// Private constructor to prevent instantiation
        /// </summary>
        private EntityGraphFactory() { }

        protected override EntityGraph<Entity> Create(Entity entity, IEntityGraphShape shape)
        {
            return new EntityGraph(entity, shape);
        }
        /// <summary>
        /// Creates a new entity graph for the given entity and shape and stores it in the
        /// cache, or returns the graph from the cache if it is already present there.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static EntityGraph Get(Entity entity, IEntityGraphShape shape)
        {
            return EntityGraphFactory._factory.GetOrCreate(entity, shape) as EntityGraph;
        }
        private static EntityGraphFactory _factory = new EntityGraphFactory();
    }
}
