using System;
using System.Collections;

namespace Utilities.graphviz
{
    public class Edge : DotElement, IEquatable<Edge>
    {
        public Edge() { }
        public Edge(Edge edge)
            : base(edge)
        {
            this.LhsType = edge.LhsType;
            this.RhsType = edge.RhsType;
            this.IsCollection = edge.IsCollection;
        }
        public string Lhs { get { return LhsType.Name; } }
        public string Rhs { get { return RhsType.Name; } }
        public bool IsCollection { get; set; }
        public Type LhsType { get; set; }
        public Type RhsType { get; set; }
        public override string ToString()
        {
            string edgeString = String.Format(@"""{0}"" -> ""{1}"" {2}", Lhs, Rhs, Attributes2String());
            if(IsCollection)
                edgeString += @"[label=""*""]";
            return edgeString;
        }
        public bool Equals(Edge other)
        {
            if(base.Equals(other) == false)
                return false;

            if(other == null)
                return false;

            return this.LhsType == other.LhsType && this.RhsType == other.RhsType;
        }
    }
}
