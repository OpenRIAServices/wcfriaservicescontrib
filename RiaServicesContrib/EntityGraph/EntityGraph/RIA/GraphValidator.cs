using System.ComponentModel.DataAnnotations;
using RiaServicesContrib.DataValidation;
using RiaServicesContrib.DomainServices.Client.DataValidation;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph
    {
        [Initialize]
        internal void InitializeGraphValidation()
        {
            this.Validator = new ValidationEngine
            {
                RulesProvider = new MEFValidationRulesProvider<ValidationResult>()
            };
        }
    }
}
