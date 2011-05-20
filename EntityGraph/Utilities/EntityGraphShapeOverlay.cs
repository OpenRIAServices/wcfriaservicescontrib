using System;
using System.Collections;
using System.Linq;
using RiaServicesContrib;
using Utilities.graphviz;

namespace Utilities
{
    public class EntityGraphShapeOverlay : GraphOverlay
    {
        public EntityGraphShape EntityGraphShape { get; private set; }

        public EntityGraphShapeOverlay(Graph source, EntityGraphShape shape) :
            base(source)
        {
            EntityGraphShape = shape;
        }

        protected override void MakeOverlay(Graph source, Graph target)
        {
            foreach(var edgeExpr in EntityGraphShape)
            {
                var lhsType = edgeExpr.FromType;
                var lhsDeclaringType = edgeExpr.EdgeInfo.DeclaringType;
                var rhsType = edgeExpr.EdgeInfo.PropertyType;
                var edgeName = edgeExpr.EdgeInfo.Name;
                if(typeof(IEnumerable).IsAssignableFrom(rhsType) && rhsType.IsGenericType)
                {
                    rhsType = rhsType.GetGenericArguments()[0];
                }

                var edge = source.Edges.SingleOrDefault(e => e.LhsType == lhsDeclaringType && e.RhsType == rhsType && e.Name == edgeName);
                var lhsNode = source.Nodes.SingleOrDefault(node => node.Type == edge.LhsType);
                var rhsNode = source.Nodes.SingleOrDefault(node => node.Type == edge.RhsType);

                if(target.Nodes.Contains(lhsNode) == false)
                    target.Nodes.Add(new Node(lhsNode));
                if(target.Edges.Contains(edge) == false)
                    target.Edges.Add(new Edge(edge));

                AddSubTypes(source, target, rhsType);
                AddBaseTypes(source, target, rhsNode);
            }
            GraphOperations.AddInheritance(target);
        }
        private void AddBaseTypes(Graph source, Graph target, Node node)
        {
            while(node != null)
            {
                if(target.Nodes.Contains(node) == false)
                {
                    target.Nodes.Add(new Node(node));
                }

                node = source.Nodes.SingleOrDefault(n => n.Type == node.Type.BaseType);
            }
        }
        private void AddSubTypes(Graph source, Graph target, Type type)
        {
            foreach(var node in source.Nodes.Where(n => type.IsAssignableFrom(n.Type)))
            {
                if(target.Nodes.Contains(node) == false)
                {
                    target.Nodes.Add(node);
                }
            }
        }
    }
}
