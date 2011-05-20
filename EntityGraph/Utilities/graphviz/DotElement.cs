using System;
using System.Collections.Generic;

namespace Utilities.graphviz
{
    public abstract class DotElement : IEquatable<DotElement>
    {
        public DotElement(DotElement de)
        {
            this.Name = de.Name;
            foreach(var attr in de.Attributes)
            {
                this.Attributes.Add(attr.Key, attr.Value);
            }
        }
        public DotElement() { }

        public string Name { get; set; }
        private Dictionary<string, string> _attributes = new Dictionary<string, string>();
        public Dictionary<string, string> Attributes { get { return _attributes; } }
        protected string Attributes2String()
        {
            string attributeString = "";
            foreach(var key in Attributes)
            {
                if(attributeString != "")
                    attributeString += ",";
                attributeString += String.Format("{0}={1}", key.Key, key.Value);
            }
            return "[" + attributeString + "]";
        }

        public virtual bool Equals(DotElement other)
        {
            if(this.Name != other.Name)
                return false;
            return true;
        }
    }
}
