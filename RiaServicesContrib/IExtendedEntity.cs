﻿
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib
{
    public interface IExtendedEntity
    {
        EntitySet EntitySet {get; set;}
    }
   public interface IExtendedEntity<T> where T : Entity, new()
    {
         EntitySet<T>  EntitySet { get; set; }
    }
}