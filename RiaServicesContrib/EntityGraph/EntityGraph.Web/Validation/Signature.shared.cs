﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that represent the signature of a validation rule which consists
    /// of InputOutput and InputOnly dependency parameters.
    /// 
    /// Dependency parameters are expressed as Linq Expressions and have the form of
    ///    A => A.some.path.i
    /// 
    /// InputOnly parameters are only used as input to a validation rule. The target property 'i', will
    /// never be invalidated during validation.
    /// 
    /// InputOutput parameters are like InputOnly parameters but they can be invalidated. This means that 
    /// the target property 'i' can be invalidated during validation.
    /// </summary>
    public class Signature : IEnumerable<ValidationRuleDependency>
    {
        /// <summary>
        /// Creates a new instance of a validation rule signature.
        /// </summary>
        private List<ValidationRuleDependency> RuleDependendencies = new List<ValidationRuleDependency>();
        /// <summary>
        /// Adds an InputOutput dependency parameter 'A => A.some.path.i' to this Signature. The target property
        /// 'i' will be invalidated in case of validation errors.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public Signature InputOutput<TSource, TTarget>(Expression<Func<TSource, TTarget>> dependency)
        {
            var validationRuleDependency = new ValidationRuleDependency
            {
                Expression = dependency
            };
            RuleDependendencies.Add(validationRuleDependency);
            return this;
        }
        /// <summary>
        /// Adds an InputOnly dependency parameter 'A => A.some.path.i' to this Signature. The target
        /// property 'i' will not be invalidaed in case of validation errors.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public Signature InputOnly<TSource, TTarget>(Expression<Func<TSource, TTarget>> dependency)
        {
            var validationRuleDependency = new ValidationRuleDependency
            {
                Expression = dependency,
                InputOnly = true
            };
            RuleDependendencies.Add(validationRuleDependency);
            return this;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection of input parameters. 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ValidationRuleDependency> GetEnumerator()
        {
            return RuleDependendencies.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection of input parameters.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Gets the number of input parameters of this Signature.
        /// </summary>
        public int Count
        {
            get
            {
                return RuleDependendencies.Count;
            }
        }
    }
}