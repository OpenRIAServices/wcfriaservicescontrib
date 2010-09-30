using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RIA.EntityGraph
{
    internal class EntityRelation<NType>
    {
        public EntityRelation()
        {
            SingleEdges = new Dictionary<PropertyInfo, NType>();
            ListEdges = new Dictionary<PropertyInfo, List<NType>>();
        }
        public NType Node { get; set; }
        public Dictionary<PropertyInfo, NType> SingleEdges { get; set; }
        public Dictionary<PropertyInfo, List<NType>> ListEdges { get; set; }
    }

    internal class EntityRelationGraph<NType> : IEnumerable<NType>
    {
        public EntityRelationGraph()
        {
            Nodes = new List<EntityRelation<NType>>();
        }
        public List<EntityRelation<NType>> Nodes { get; private set; }

        public IEnumerator<NType> GetEnumerator()
        {
            foreach (var node in Nodes)
            {
                yield return node.Node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
