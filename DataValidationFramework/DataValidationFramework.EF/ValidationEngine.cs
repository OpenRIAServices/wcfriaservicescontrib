using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using RiaServicesContrib.DataValidation;

namespace DataValidationFramework.EF
{
    public class ValidationEngine : ValidationEngine<object, DbValidationError>
    {
        private Dictionary<object, List<DbValidationError>> _entityErrors;
        /// <summary>
        /// Returns a dictionary with entities as keys and lists of validation errors as values.
        /// </summary>
        public Dictionary<object, List<DbValidationError>> EntityErrors
        {
            get
            {
                if(_entityErrors == null)
                {
                    _entityErrors = new Dictionary<object, List<DbValidationError>>();
                }
                return _entityErrors;
            }
        }
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given
        /// the collection of entities managed by the change tracker of the provided DbContext and an 
        /// object that must be present in any selcted signature.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objects"></param>
        public void Validate(object entity, DbContext context)
        {
            Validate(entity, context.ChangeTracker.Entries().Select(x => x.Entity));
        }
        /// <summary>
        /// Method that clears the validation result of the given entity, for the given members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected override void ClearValidationResult(object entity, string[] membersInError, DbValidationError validationResult)
        {
            if(validationResult != null && EntityErrors.ContainsKey(entity))
            {
                var errors = EntityErrors[entity];
                var validationErrors = errors
                    .Where(ve => ve.ErrorMessage == validationResult.ErrorMessage)
                    .Where(ve => membersInError.Contains(ve.PropertyName));
                if(validationErrors.Any())
                {
                    foreach(var ve in validationErrors)
                    {
                        errors.Remove(ve);
                    }
                }
            }
        }
        /// <summary>
        /// Method that checks if the entity has a validation error for the given set of members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected override bool HasValidationResult(object entity, string[] membersInError, DbValidationError validationResult)
        {
            if(validationResult == null || EntityErrors.ContainsKey(entity) == false)
            {
                return false;
            }
            var errors = EntityErrors[entity];
            var validationErrors = errors
                    .Where(ve => ve.ErrorMessage == validationResult.ErrorMessage)
                    .Where(ve => membersInError.Contains(ve.PropertyName));
            if(validationErrors.Any())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method that checks if the given validation result indicates a successful validation.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected override bool IsValidationSuccess(DbValidationError validationResult)
        {
            return validationResult == null;
        }
        /// <summary>
        /// Method that sets a validation error for the given memebrs of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected override void SetValidationResult(object entity, string[] membersInError, DbValidationError validationResult)
        {
            if(validationResult == null)
            {
                return;
            }

            List<DbValidationError> result = null;
            if(EntityErrors.ContainsKey(entity))
            {
                result = EntityErrors[entity];
            }
            else
            {
                result = new List<DbValidationError>();
                EntityErrors.Add(entity, result);
            }
            foreach(var member in membersInError)
            {
                result.Add(new DbValidationError(member, validationResult.ErrorMessage));
            }
        }
    }
}
