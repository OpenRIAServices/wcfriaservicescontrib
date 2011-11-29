using System;
using System.Linq;
using RiaServicesContrib.Extensions;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph
    {
        /// <summary>
        /// Method that synchronizes the current graph with the provided graph by copying properties and entity states
        /// from the entities in give graph to the current graph.
        /// </summary>
        /// <param name="srcGraph">The graph to synchronize from</param>
        public void Synchronize(EntityGraph srcGraph)
        {
            if(EntityGraphEqual(srcGraph, EntityTypeComparer) == false)
            {
                throw new Exception("Source graph entity graph must be a copy or a clone of present entity graph.");
            }
            var elements = srcGraph.Zip(this, (src, tgt) => new { src = src, tgt = tgt });
            foreach(var pair in elements)
            {
                var originalState = pair.src.ExtractState(ExtractType.OriginalState);
                var modifiedState = pair.src.ExtractState(ExtractType.ModifiedState);

                pair.tgt.ApplyState(originalState, modifiedState);
            }
        }
        private bool EntityTypeComparer(Entity e1, Entity e2)
        {
            return e1 != e2 && e1.GetType() == e2.GetType();
        }
    }
}
