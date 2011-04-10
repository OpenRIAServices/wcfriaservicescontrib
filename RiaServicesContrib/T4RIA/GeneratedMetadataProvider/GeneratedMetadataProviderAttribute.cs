using System;
using System.ServiceModel.DomainServices.Server;

namespace GeneratedMetadataProvider
{
    /// <summary>
    /// When applied to a <see cref="DomainService"/> this attribute indicates that
    /// generated metadata should be applied to the Types exposed by the <see cref="DomainService"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class GeneratedMetadataProviderAttribute : DomainServiceDescriptionProviderAttribute
    {
        public GeneratedMetadataProviderAttribute()
            : base(typeof(GeneratedMetadataProvider))
        {
        }
    }
}
