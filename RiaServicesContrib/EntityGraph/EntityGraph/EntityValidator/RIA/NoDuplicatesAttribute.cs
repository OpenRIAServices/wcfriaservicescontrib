using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RIA.EntityValidator.RIA
{
    /// <summary>
    /// This validation attribute verifies that a collection does not contain duplicate elements
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NoDuplicatesAttribute : ValidationAttribute
    {
        public void Validate(IEnumerable collection)
        {
            List<object> list = new List<object>();
            foreach(var element in collection)
            {
                if(list.Contains(element))
                {
                    string msg = String.Format(ErrorMessage, PropertyInfo.Name);
                    Result = new ValidationResult(msg, new string[] { PropertyInfo.Name });
                    return;
                }
                list.Add(element);
            }
            Result = ValidationResult.Success;
        }
    }
}
