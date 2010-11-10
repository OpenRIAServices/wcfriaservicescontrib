using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.graphviz;

namespace Utilities
{
    public static class GraphOperations
    {
        #region Overlays
        public static Graph Overlay(this Graph source, GraphOverlay overlay, Action<Node> markNode, Action<Edge> markEdge)
        {
            Graph target = new Graph();

            foreach(var node in source.Nodes)
            {
                var newNode = new Node(node);
                target.Nodes.Add(newNode);
                if(overlay.Graph.Nodes.Contains(newNode))
                    markNode(newNode);
            }
            foreach(var edge in source.Edges)
            {
                var newEdge = new Edge(edge);
                target.Edges.Add(newEdge);
                if(overlay.Graph.Edges.Contains(newEdge))
                    markEdge(newEdge);
            }
            return target;
        }
        #endregion
        #region Shrink
        /// <summary>
        /// Returns a graph that is obtained by thrinking the given graph to only include nodes (and corresponding) edges for 
        /// which nodeMatch return true
        /// </summary>
        /// <param name="source"></param>
        /// <param name="nodeMatch"></param>
        /// <returns></returns>
        public static Graph Shrink(this Graph source, Func<Node, bool> nodeMatch)
        {
            return Shrink(source, nodeMatch, 0);
        }
        /// <summary>
        /// Returns a graph that is obtained by first thrinking the given graph to only include nodes (and corresponding) edges for 
        /// which nodeMatch return true, and then expanding it to include nodes of given distance
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matches"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Graph Shrink(this Graph source, Func<Node, bool> matches, int distance)
        {
            var target = new Graph { Name = source.Name };
            Seed(source, target, matches);
            for(int i = 0; i < distance; i++)
            {
                Expand(source, target);
            }
            return target;
        }
        private static void Expand(Graph source, Graph target)
        {
            var targetNodes = target.Nodes.ToList();
            foreach(var edge in source.Edges.Where(edge => target.Edges.Contains(edge) == false))
            {
                var lhsType = edge.LhsType;
                var rhsType = edge.RhsType;
                if(targetNodes.Any(node => node.Type == lhsType && node.Name == edge.Lhs))
                {
                    if(targetNodes.Any(node => node.Type == rhsType && node.Name == edge.Rhs) == false)
                    {
                        var sourceNode = source.Nodes.Single(node => node.Type == rhsType && node.Name == edge.Rhs);
                        target.Nodes.Add(new Node(sourceNode));
                        target.Edges.Add(new Edge(edge));
                    }
                }
            }
        }
        private static void Seed(Graph source, Graph target, Func<Node, bool> matches)
        {
            foreach(var node in source.Nodes.Where(matches))
            {
                target.Nodes.Add(new Node(node));
            }
            foreach(var edge in source.Edges)
            {
                var lhsType = edge.LhsType;
                var rhsType = edge.RhsType;
                if(lhsType.Name == "Diagnosis")
                {
                }
                var lhs = target.Nodes.SingleOrDefault(n => n.Type == lhsType);
                var rhs = target.Nodes.SingleOrDefault(n => n.Type == rhsType);
                if(lhs != null && rhs != null && lhs.Type == edge.LhsType && rhs.Type == edge.RhsType)
                {
                    target.Edges.Add(new Edge(edge));
                }
            }
        }
        #endregion
        #region Entity types 2 dot graph
        public static Graph EntityTypes2DotGraph(Func<IEnumerable<Type>> Entities, Func<PropertyInfo, bool> IsAssociation)
        {
            var graph = new Graph();
            var xs = Entities().ToList();
            var elements = Entities().SelectMany(entityType => EntityType2DotElements(entityType, IsAssociation));

            foreach(var node in elements.OfType<Node>())
            {
                graph.Nodes.Add(node);
            }
            foreach(var edge in elements.OfType<Edge>())
            {
                graph.Edges.Add(edge);
            }
            AddInheritance(graph);
            return graph;
        }
        public static void AddInheritance(Graph graph)
        {
            foreach(var node in graph.Nodes)
            {
                var baseType = graph.Nodes.SingleOrDefault(n => n.Type == node.Type.BaseType);
                if(baseType != null)
                {
                    var edge = new Edge
                    {
                        LhsType = node.Type,
                        RhsType = baseType.Type
                    };
                    edge.Attributes.Add("style", "dashed");
                    graph.Edges.Add(edge);
                }
            }
        }

        private static IEnumerable<DotElement> EntityType2DotElements(Type type, Func<PropertyInfo, bool> IsAssociation)
        {
            yield return new Node { Name = type.Name, Type = type };
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            foreach(var propInfo in type.GetProperties(bindingFlags).Where(IsAssociation))
            {
                if(typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType) && propInfo.PropertyType.IsGenericType)
                {
                    var ptype = propInfo.PropertyType.GetGenericArguments()[0];
                    var edge = new Edge
                    {
                        Name = propInfo.Name,
                        LhsType = type,
                        RhsType = ptype
                    };
                    yield return edge;
                }
                else
                {
                    var edge = new Edge
                    {
                        Name = propInfo.Name,
                        LhsType = type,
                        RhsType = propInfo.PropertyType
                    };
                    yield return edge;
                }
            }
        }
        #endregion
    }
}
