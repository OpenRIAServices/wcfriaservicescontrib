using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represent the dependency signature of a validation rule
    /// </summary>
    public class Signature : List<ValidationRuleDependency>
    {
        public Signature Depedency<TSource, TTarget>(Expression<Func<TSource, TTarget>> dependency)
        {
            var validationRuleDependency = new ValidationRuleDependency
            {
                Expression = dependency,
            };
            base.Add(validationRuleDependency);
            return this;
        }
    }
}
