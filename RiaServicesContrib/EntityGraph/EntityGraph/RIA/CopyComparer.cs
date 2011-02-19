using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib;
using RiaServicesContrib.Extensions;
using System.Collections.Generic;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Determines whether two entity graphs are copies of eachother by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool IsCopyOf<T>(EntityGraph<T> graph) where T : Entity
        {
            return EntityGraphEqual(graph, (e1, e2) => e1 != e2 && MemberwiseCompare(e1, e2, false));
        }

        private bool MemberwiseCompare(Entity e1, Entity e2, bool checkKeys) {
            if(CheckState(e1, e2, ExtractType.ModifiedState, checkKeys) == false)
                return false;
            if(CheckState(e1, e2, ExtractType.OriginalState, checkKeys) == false)
                return false;
            return true;
        }

        private bool CheckState(Entity e1, Entity e2, ExtractType state, bool checkKeys) {
            List<string> keys = null;
            List<string> foreignKeys = null;
            var stateE1 = e1.ExtractState(state);
            var stateE2 = e2.ExtractState(state);
            if(stateE1.Count != stateE2.Count)
            {
                return false;
            }
            if(checkKeys == false)
            {
                keys = GetKey(e1);
                foreignKeys = GetForeignKeys(e1);
            }
            var zip = stateE1.Zip(stateE2, (a, b) => new { name = a.Key, a = a.Value, b = b.Value });
            foreach(var v in zip)
            {
                if(v.a != v.b && v.a.Equals(v.b) == false)
                {
                    if(checkKeys == false)
                    {
                        if(keys.Contains(v.name) || foreignKeys.Contains(v.name))
                        {
                            continue;
                        }
                    }
                    return false;
                }
            }
            return true;
        }
        private List<string> GetForeignKeys(Entity e)
        {
            var type = e.GetType();
            var assocs = from prop in type.GetProperties()
                         where prop.IsDefined(typeof(AssociationAttribute), true)
                         from assoc in prop.GetCustomAttributes(typeof(AssociationAttribute), true).Cast<AssociationAttribute>()
                         where
                         assoc.IsForeignKey
                         select assoc.ThisKey;
            return assocs.ToList();
        }
        private List<string> GetKey(Entity e)
        {
            var type = e.GetType();
            var keys = from prop in type.GetProperties()
                       where prop.IsDefined(typeof(KeyAttribute), true)
                       select prop.Name;
            return keys.ToList();
        }
    }
}
