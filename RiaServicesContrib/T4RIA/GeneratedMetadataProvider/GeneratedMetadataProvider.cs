using System;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Server;

namespace GeneratedMetadataProvider
{
    /// <summary>
    /// DomainServiceDescriptionProvider used to extend Type description
    /// by applying additional generated metadata.
    /// </summary>
    internal class GeneratedMetadataProvider : DomainServiceDescriptionProvider
    {
        public GeneratedMetadataProvider(Type domainServiceType, DomainServiceDescriptionProvider parent)
            : base(domainServiceType, parent)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type type, ICustomTypeDescriptor parent)
        {
            return new GeneratedMetadataTypeDescriptor(base.GetTypeDescriptor(type, parent), type);
        }
    }
}
