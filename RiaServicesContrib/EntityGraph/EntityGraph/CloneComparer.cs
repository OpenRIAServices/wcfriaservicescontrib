using System.Linq;
using System.ServiceModel.DomainServices.Client;

using RiaServicesContrib;
using RiaServicesContrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Determines whether two entity graphs are clones by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool IsCloneOf<T>(EntityGraph<T> graph) where T : Entity{
            return EntityGraphEqual(graph, (e1, e2) => e1 != e2 && MemberwiseCompare(e1, e2));
        }

        private bool MemberwiseCompare(Entity e1, Entity e2) {
            if(CheckState(e1, e2, ExtractType.ModifiedState) == false)
                return false;
            if(CheckState(e1, e2, ExtractType.OriginalState) == false)
                return false;
            return true;
        }

        private bool CheckState(Entity e1, Entity e2, ExtractType state) {
            var stateE1 = e1.ExtractState(ExtractType.ModifiedState);
            var stateE2 = e2.ExtractState(ExtractType.ModifiedState);
            var zip = stateE1.Zip(stateE2, (a, b) => new { name = a.Key, a = a.Value, b = b.Value });
            foreach(var v in zip)
            {
                if(v.a != v.b)
                {
                    var propInfo = e1.GetType().GetProperty(v.name);
                    if(propInfo.IsDefined(typeof(RoundtripOriginalAttribute), true))
                        continue;
                    return false;
                }
            }
            return true;
        }

    }
}
