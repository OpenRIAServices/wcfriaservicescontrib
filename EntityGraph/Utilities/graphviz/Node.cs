using System;

namespace Utilities.graphviz
{
    public class Node : DotElement, IEquatable<Node>
    {
        public Node()
        {
            this.Attributes.Add("style", "filled");
        }
        public Node(Node node)
            : base(node)
        {
            this.Type = node.Type;
        }
        public Type Type { get; set; }
        public override string ToString()
        {
            return "\"" + Name + "\"" + Attributes2String(); ;
        }
        public bool Equals(Node other)
        {
            if(base.Equals(other) == false)
                return false;
            if(other == null)
                return false;
            return this.Type == other.Type;
        }
    }
}
