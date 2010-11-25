using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityValidator.RIA
{
    public abstract class ValidationAttribute : Attribute, IValidationAttribute<Entity,ValidationResult>
    {
        public event EventHandler<ValidationResultChangedEventArgs<ValidationResult>> ValidationResultChanged;

        protected PropertyInfo PropertyInfo { get; private set; }
        
        public string ErrorMessage { get; set; }

        protected ValidationAttribute()
        {
            this.ErrorMessage = "Required property {0} is missing";
        }

        private ValidationResult _result;
        public ValidationResult Result
        {
            get { return _result; }
            protected set
            {
                if(_result != value)
                {
                    ValidationResult old = _result;
                    _result = value;
                    if(ValidationResultChanged != null)
                    {
                        ValidationResultChanged(this, new ValidationResultChangedEventArgs<ValidationResult>(old, value));
                    }
                }
            }
        }

        public void InvokeValidate(Entity entity)
        {
            var minfo = ValidationRuleInvokeHelper<Entity, ValidationResult>.FindValidateMethod(this, new Type[] { PropertyInfo.PropertyType });
            minfo.Invoke(this, new object[] { PropertyInfo.GetValue(entity, null) });
        }

        public void SetPropertyInfo(PropertyInfo propInfo)
        {
            PropertyInfo = propInfo;
        }
    }
}
