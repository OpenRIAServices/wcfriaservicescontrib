using System;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    using RiaServicesContrib.DomainServices.Client.contrib;

    public partial class EntityGraph
    {
        /// <summary>
        /// Method that synchronizes the current graph with the provided graph by copying properties and entity states
        /// from the entities in give graph to the current graph. All property values in the current graph will be overwritten with
        /// the property values form the entities in srcGraph (i.e., this method has load behavior LoadBehavior.RefreshCurrent).
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
                RefreshCurrent(pair.src, pair.tgt);
            }
        }
        /// <summary>
        /// Method that synchronizes the current graph with the provided graph by copying properties and entity states
        /// from the entities in give graph to the current graph. The way that property values are merged is specified by loadBehavior. 
        /// </summary>
        /// <param name="srcGraph">The graph to synchronize from</param>
        /// <param name="loadBehavior">The LoadBehavior to apply. When specifying LoadBehavior.KeepCurrent, Synchronize will have no effect.</param>
        public void Synchronize(EntityGraph srcGraph, LoadBehavior loadBehavior)
        {
            if(loadBehavior == LoadBehavior.KeepCurrent)
            {
                return;
            }
            if(loadBehavior == LoadBehavior.RefreshCurrent)
            {
                Synchronize(srcGraph);
            }
            if(EntityGraphEqual(srcGraph, EntityTypeComparer) == false)
            {
                throw new Exception("Source graph entity graph must be a copy or a clone of present entity graph.");
            }
            var elements = srcGraph.Zip(this, (src, tgt) => new { src = src, tgt = tgt });
            foreach(var pair in elements)
            {
                MergeIntoCurrent(pair.src, pair.tgt);
            }
        }
        private bool EntityTypeComparer(Entity e1, Entity e2)
        {
            return e1 != e2 && e1.GetType() == e2.GetType();
        }
        private void RefreshCurrent(Entity src, Entity tgt)
        {
            var originalState = src.ExtractState(ExtractType.OriginalState);
            var modifiedState = src.ExtractState(ExtractType.ModifiedState);

            tgt.ApplyState(originalState, modifiedState);
        }
        private void MergeIntoCurrent(Entity src, Entity tgt)
        {
            var originalStateSrc = src.ExtractState(ExtractType.OriginalState);
            var modifiedStateSrc = src.ExtractState(ExtractType.ModifiedState);
            var originalStateTgt = tgt.ExtractState(ExtractType.OriginalState);
            var modifiedStateTgt = tgt.ExtractState(ExtractType.ModifiedState);

            foreach(var propName in originalStateSrc.Keys.ToList())
            {
                var originalValueTgt = originalStateTgt[propName];
                var modifiedValueTgt = modifiedStateTgt[propName];
                var modifiedValueSrc = modifiedStateSrc[propName];

                if(object.Equals(modifiedValueSrc, modifiedValueTgt) == false)
                {
                    if(object.Equals(modifiedValueTgt, originalValueTgt) == false)
                    {
                        originalStateSrc[propName] = originalValueTgt;
                        modifiedStateSrc[propName] = modifiedValueTgt;
                    }
                }
            }
            tgt.ApplyState(originalStateSrc, modifiedStateSrc);
        }
    }
}
