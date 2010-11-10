using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.graphviz
{
    public class Graph : DotElement
    {
        public Graph() { }
        public Graph(Graph graph)
            : base(graph)
        {
            foreach(var node in graph.Nodes)
            {
                Nodes.Add(new Node(node));
            }
            foreach(var edge in graph.Edges)
            {
                Edges.Add(new Edge(edge));
            }
        }

        private List<Node> _nodes = new List<Node>();
        public List<Node> Nodes { get { return _nodes; } }

        private List<Edge> _edges = new List<Edge>();
        public List<Edge> Edges { get { return _edges; } }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendFormat("digraph {0} {{", Name);
            b.AppendLine();
            foreach(var node in Nodes)
            {
                b.AppendLine(node.ToString());
            }
            foreach(var edge in Edges)
            {
                b.AppendLine(edge.ToString());
            }
            b.AppendLine("}");
            return b.ToString();
        }
    }
}
