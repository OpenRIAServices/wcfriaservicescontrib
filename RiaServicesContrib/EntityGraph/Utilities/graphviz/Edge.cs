using System;

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
        }
        public string Lhs { get { return LhsType.Name; } }
        public string Rhs { get { return RhsType.Name; } }
        public Type LhsType { get; set; }
        public Type RhsType { get; set; }
        public override string ToString()
        {
            return "\"" + Lhs + "\"" + "->" + "\"" + Rhs + "\"" + Attributes2String();// +String.Format(@"[label=""{0}""]", Name);
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
