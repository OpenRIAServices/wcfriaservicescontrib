using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RIA.EntityValidator
{
    /// <summary>
    /// Class that makes rule dependencies look nicer
    /// </summary>
    /// <typeparam name="TRoot"></typeparam>
    public class ValidationRuleDependencies<TRoot> : List<Expression<Func<TRoot, object>>>
    {
    }
}
