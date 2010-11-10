﻿using System;
using System.Collections.Generic;

namespace RIA.EntityValidator
{
    public interface IValidationRulesProvider<TEntity, TResult> where TResult : class
    {
        Dictionary<Tuple<object, string>, List<IValidationRule<TResult>>> GetValidationRules(TEntity root);
    }
}
