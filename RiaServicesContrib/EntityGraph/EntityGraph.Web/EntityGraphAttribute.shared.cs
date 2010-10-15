using System;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Specifies that an association is part of a graph of related entities 
    /// that should be handled as a unit during cloning, detaching, deleting, and so on.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class EntityGraphAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the name of the entity graph
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Constructor. Sets the name of the entity graph to 'name'
        /// </summary>
        /// <param name="name"></param>
        public EntityGraphAttribute(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityGraphAttribute() { Name = null;  }
    }
}
