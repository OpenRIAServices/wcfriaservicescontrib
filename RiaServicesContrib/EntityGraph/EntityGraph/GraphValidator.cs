using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Specialized;
using RIA.EntityValidator;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity>
    {
        private ValidationEngine<EntityGraph<TEntity>, ValidationResult> Validator { get; set; }

        private void InitGraphValidation() {
            Validator = new ValidationEngine<EntityGraph<TEntity>, ValidationResult>(this);

            Validator.ValidationResultChanged += Validator_ValidationResultChanged;
            this.PropertyChanged += Validate;
            this.CollectionChanged += Validator_CollectionChanged;
        }

        private void Validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<ValidationResult> e) {
            var rule = (GraphValidationRule<TEntity>)sender;
            foreach(var entity in Validator.ObjectsInvolved(rule).Cast<Entity>())
            {
                if(e.OldResult != ValidationResult.Success)
                    entity.ValidationErrors.Remove(e.OldResult);
                if(e.Result != ValidationResult.Success && entity.ValidationErrors.Contains(e.Result) == false)
                    entity.ValidationErrors.Add(e.Result);
            }
        }
        private void Validator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args) {
            Validator.Refresh();
            var collection = (from n in EntityRelationGraph.Nodes
                              from edge in n.ListEdges
                              where edge.Key.PropertyType == sender.GetType()
                              select new { onwer = n.Node, propInfo = edge.Key }).SingleOrDefault();
            Validator.Validate(collection.onwer, collection.propInfo.Name);
        }
        private void Validate(object sender, PropertyChangedEventArgs args) {
            Validator.Validate(sender, args.PropertyName);           
        }
    }
}