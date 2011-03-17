using System;

namespace RiaServicesContrib.Validation
{
    /// <summary>
    /// Class that represents a binding between a dependency parameter and an object.
    /// 
    /// For a validation rule dependency
    ///    A => A.B.c (where type of A equals A)
    ///    
    /// And an object
    ///    var a = new A { B = new B() };
    /// 
    /// ParameterObject equals a.
    /// ParamterObjectType equals A (i.e., the type of parameter A)
    /// ParameterName equals 'A'
    /// </summary>
    internal class ParameterObjectBinding : IEquatable<ParameterObjectBinding>
    {
        /// <summary>
        /// Represents the object that is bound to the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public object ParameterObject { get; set; }
        /// <summary>
        /// Represents the type of the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public Type ParameterObjectType { get; set; }
        /// <summary>
        /// Represents the name of the parameter 'A' of a valudation rule dependency 'A => A.some.path.p'.
        /// </summary>
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
