using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntityGraph.Validation;

namespace EntityGraph
{
    public partial class EntityGraph<TEntity, TBase, TValidationResult>
    {
        private ValidationEngine<TBase, TValidationResult> Validator;

        [Initialize]
        internal void InitGraphValidation()
        {
            var rulesProvider = new MEFValidationRulesProvider<TValidationResult>();
            Validator = new ValidationEngine<TBase, TValidationResult>(rulesProvider);
            Validator.ValidationResultChanged += Validator_ValidationResultChanged;

            this.EntityRelationGraphResetted += ValidatorRefresh;

            this.CollectionChanged += Validator_CollectionChanged;
            this.PropertyChanged += Validate;

            Validator.ValidateAll(this);
        }

        [Dispose]
        internal void CleanGraphValidation()
        {
            this.Validator.ValidationResultChanged -= Validator_ValidationResultChanged;
            this.EntityRelationGraphResetted -= ValidatorRefresh;
            Validator.Dispose();
            this.CollectionChanged -= Validator_CollectionChanged;
            this.PropertyChanged -= Validate;
        }

        private void ValidatorRefresh(object sender, EventArgs args)
        {
            ValidateAll();
        }
        private void Validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TValidationResult> e)
        {
            var rule = (ValidationRule<TValidationResult>)sender;
            foreach(var entity in Validator.ObjectsInvolved(rule).OfType<TBase>())
            {
                if(HasValidationResult(entity, e.OldValidationResult))
                    ClearValidationResult(entity, e.OldValidationResult);
                if(HasValidationResult(entity, e.ValidationResult) == false)
                    SetValidationResult(entity, e.ValidationResult);
            }
        }

        protected abstract bool HasValidationResult(TBase entity, TValidationResult validationResult);
        protected abstract void ClearValidationResult(TBase entity, TValidationResult validationResult);
        protected abstract void SetValidationResult(TBase entity, TValidationResult validationResult);

        private void Validator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var senderType = sender.GetType();
            var collections = (from n in EntityRelationGraph.Nodes
                               from edge in n.ListEdges
                               where
                                 edge.Key.PropertyType == senderType &&
                                 edge.Key.GetValue(n.Node, null) == sender
                               select new { owner = n.Node, propInfo = edge.Key });
            var collection = collections.SingleOrDefault();
            Validator.Validate(collection.owner, collection.propInfo.Name, this);
        }
        private void Validate(object sender, PropertyChangedEventArgs args)
        {
            Validator.Validate(sender, args.PropertyName, this);
        }
        private void ValidateAll()
        {
            Validator.ValidateAll(this);
        }
    }
}