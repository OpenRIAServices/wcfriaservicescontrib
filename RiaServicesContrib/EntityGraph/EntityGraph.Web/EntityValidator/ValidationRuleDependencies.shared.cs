using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RIA.EntityValidator
{
    /// <summary>
    /// Class that makes rule dependencies look nicer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationRuleDependencies<TEntity> : List<Expression<Func<TEntity, object>>>
    {
    }
}
