using System;
using System.Collections.Generic;
namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represents a binding between an dependency parameter and an object
    /// </summary>
    internal  class ParameterObjectBinding :IEquatable<ParameterObjectBinding>
    {
        public object ParameterObject { get; set; }
        public Type ParameterObjectType { get; set; }
        public string ParameterName { get; set; }

        public bool Equals(ParameterObjectBinding other)
        {
            if(Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if(Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return ParameterObject == other.ParameterObject
                && ParameterObjectType == other.ParameterObjectType
                && ParameterName == other.ParameterName;
        }
        public override int GetHashCode()
        {
            int hashParameterObject = ParameterObject == null ? 0 : ParameterObject.GetHashCode();
            int hashParameterObjectType = ParameterObjectType == null ? 0 : ParameterObjectType.GetHashCode();
            int hashParameterName = ParameterName == null ? 0 : ParameterName.GetHashCode();

            return hashParameterObject ^ hashParameterObjectType ^ hashParameterName;
        }
    }
}
