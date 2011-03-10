using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EntityGraph.Validation;
using System.Diagnostics;

namespace EntityGraph
{
    /// <summary>
    /// Class that implements cross-entity validation for entity graphs.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    /// <typeparam name="TValidationResult"></typeparam>
    public partial class EntityGraph<TEntity, TBase, TValidationResult>
    {
        private ValidationEngine<TBase, TValidationResult> Validator;

        /// <summary>
        /// Initialization method.
        /// </summary>
        [Initialize]
        internal void InitGraphValidation()
        {
            var rulesProvider = new MEFValidationRulesProvider<TValidationResult>();
            Validator = new ValidationEngine<TBase, TValidationResult>(rulesProvider);
            Validator.ValidationResultChanged += Validator_ValidationResultChanged;

            this.EntityRelationGraphResetted += ValidatorRefresh;

            this.CollectionChanged += Validator_CollectionChanged;
            this.PropertyChanged += Validator_PropertyChanged;

            Validator.Validate(this);
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        [Dispose]
        internal void CleanGraphValidation()
        {
            this.Validator.ValidationResultChanged -= Validator_ValidationResultChanged;
            this.EntityRelationGraphResetted -= ValidatorRefresh;
            Validator.Dispose();
            this.CollectionChanged -= Validator_CollectionChanged;
            this.PropertyChanged -= Validator_PropertyChanged;
        }

        /// <summary>
        /// Method that checks if the entity has a validation error for the given set of members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected abstract bool HasValidationResult(TBase entity, string[] membersInError, TValidationResult validationResult);
        /// <summary>
        /// Method that clears the validation result of the given entity, for the given members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void ClearValidationResult(TBase entity, string[] membersInError, TValidationResult validationResult);
        /// <summary>
        /// Method that sets a validation error for the given memebrs of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void SetValidationResult(TBase entity, string[] membersInError, TValidationResult validationResult);

        /// <summary>
        /// callback method that is called when the entity graph has been reset. In that case 
        /// all validation rules have to be evaluated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ValidatorRefresh(object sender, EventArgs args)
        {
            Validator.Validate(this);
        }
        /// <summary>
        /// Callback method that is called when a ValidationResultChanged event is received from the
        /// validation rule engine. In this method we distribute the validation results over
        /// the involved entities. These are the target owner objects of InputOutput rule depdenencies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TValidationResult> e)
        {
            var rule = (ValidationRule<TValidationResult>)sender;

            var bindingGroups = from binding in rule.RuleBinding.DependencyBindings
                                where
                                binding.ValidationRuleDependency.InputOnly == false &&
                                binding.TargetOwnerObject is TBase
                                group binding by binding.TargetOwnerObject;

            foreach(var bindingGroup in bindingGroups)
            {
                var entity = bindingGroup.Key as TBase;
                var membersInError = bindingGroup.Select(binding => binding.ValidationRuleDependency.TargetProperty.Name).ToArray();

                if(HasValidationResult(entity, membersInError, e.OldValidationResult))
                    ClearValidationResult(entity, membersInError, e.OldValidationResult);
                if(HasValidationResult(entity, membersInError, e.ValidationResult) == false)
                    SetValidationResult(entity, membersInError, e.ValidationResult);
            }
        }
        /// <summary>
        /// Callback method that is called when a CollectionChanged event is received from the entity graph.
        /// We obtain the node and edge in the entity graph for this collection and then call the Validate
        /// method of the validation engine for them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Validator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var senderType = sender.GetType();
            var nodeEdge = (from n in EntityRelationGraph.Nodes
                               from edge in n.ListEdges
                               where
                                 edge.Key.PropertyType == senderType &&
                                 edge.Key.GetValue(n.Node, null) == sender
                               select new { node = n.Node, edge = edge.Key.Name }).Single();
            Validator.Validate(nodeEdge.node, nodeEdge.edge, this);
        }
        /// <summary>
        /// Callback method that is called when a PropertyChanged event is received from the entity graph.
        /// We call the Validate method of the validation engine for the object and property name of the 
        /// changed property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Validator_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            Validator.Validate(sender, args.PropertyName, this);
        }
    }
}