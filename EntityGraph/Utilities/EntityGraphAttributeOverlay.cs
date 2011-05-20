using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Utilities.graphviz;
using System;

namespace Utilities
{
    public class EntityGraphAttributeOverlay : GraphOverlay
    {
        public EntityGraphAttributeOverlay(Graph graph) : base(graph) { }
        private PropertyInfo[] GetPropertyInfoFromMetadata(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(MetadataTypeAttribute), false).FirstOrDefault();
            var metadata = (MetadataTypeAttribute)attr;
            var metadataClassType = metadata.MetadataClassType;
            return GetPropertyInfoFromType(metadataClassType);
        }
        private PropertyInfo[] GetPropertyInfoFromType(Type type)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var propInfoes = type.GetProperties(bindingFlags).Where(p => p.IsDefined(typeof(EntityGraphAttribute), false));
            return propInfoes.ToArray();
        }
        protected override void MakeOverlay(Graph source, Graph overlay)
        {
            foreach(var node in source.Nodes)
            {
                PropertyInfo[] propInfos;

                if(node.Type.IsDefined(typeof(MetadataTypeAttribute), false))
                {
                    propInfos = GetPropertyInfoFromMetadata(node.Type);
                }
                else
                {
                    propInfos = GetPropertyInfoFromType(node.Type);
                }

                foreach(var propInfo in propInfos)
                {
                    var lhsType = propInfo.DeclaringType;
                    var rhsType = propInfo.PropertyType;
                    if(typeof(IEnumerable).IsAssignableFrom(rhsType) && rhsType.IsGenericType)
                    {
                        rhsType = rhsType.GetGenericArguments()[0];
                    }
                    var edge = source.Edges.SingleOrDefault(e => e.LhsType == node.Type && e.RhsType == rhsType && e.Name == propInfo.Name);
                    var rhsNode = source.Nodes.SingleOrDefault(n => n.Type == rhsType);
                    if(overlay.Edges.Contains(edge) == false)
                        overlay.Edges.Add(new Edge(edge));
                    if(overlay.Nodes.Contains(node) == false)
                        overlay.Nodes.Add(new Node(node));
                    if(overlay.Nodes.Contains(rhsNode) == false)
                        overlay.Nodes.Add(new Node(rhsNode));
                }
            }
        }
    }
}
