using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RIA.EntityValidator;

namespace EntityGraph
{
    public partial class EntityGraph<TEntity, TBase, TValidationResult>
    {
        private ValidationEngine<EntityGraph<TEntity, TBase, TValidationResult>, TValidationResult> Validator;

        [Initialize]
        internal void InitGraphValidation() {
            var rulesProvider = new MEFValidationRulesProvider<EntityGraph<TEntity, TBase, TValidationResult>, TValidationResult>();
            Validator = new ValidationEngine<EntityGraph<TEntity, TBase, TValidationResult>, TValidationResult>(rulesProvider, this);
            Validator.ValidationResultChanged += Validator_ValidationResultChanged;
            this.PropertyChanged += Validate;
            this.CollectionChanged += Validator_CollectionChanged;
            this.ValidateAll();
        }
        
        [Dispose]
        internal void CleanGraphValidation()
        {
            this.PropertyChanged -= Validate;
            this.CollectionChanged -= Validator_CollectionChanged;
        }

        private void Validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TValidationResult> e) {
            var rule = (GraphValidationRule<TEntity, TBase, TValidationResult>)sender;
            foreach(var entity in Validator.ObjectsInvolved(rule).Cast<TBase>())
            {
                if(HasValidationResult(entity, e.OldResult))
                    ClearValidationResult(entity, e.OldResult);
                if(HasValidationResult(entity, e.Result) == false)
                    SetValidationResult(entity, e.Result);
            }
        }

        protected abstract bool HasValidationResult(TBase entity, TValidationResult validationResult);
        protected abstract void ClearValidationResult(TBase entity, TValidationResult validationResult);
        protected abstract void SetValidationResult(TBase entity, TValidationResult validationResult);

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
        private void ValidateAll()
        {
            Validator.ValidateAll();
        }
    }
}