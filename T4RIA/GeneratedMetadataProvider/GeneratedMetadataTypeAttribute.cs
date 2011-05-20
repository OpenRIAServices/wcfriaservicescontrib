using System;

namespace GeneratedMetadataProvider
{
    /// <summary>
    /// Generated metadata attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GeneratedMetadataTypeAttribute : Attribute
    {
        public GeneratedMetadataTypeAttribute(Type generatedMetadataClassType)
        {
            if (generatedMetadataClassType == null)
            {
                throw new ArgumentNullException();
            }
            GeneratedMetadataClassType = generatedMetadataClassType;
        }

        public Type GeneratedMetadataClassType { get; private set; }
    }
}
