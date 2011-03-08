using System;

namespace EntityGraph.Validation
{
	public class ValidationResultChangedEventArgs<TValidationResult> : EventArgs
    {
        public TValidationResult OldValidationResult { get; private set; }
        public TValidationResult ValidationResult { get; private set; }

        public ValidationResultChangedEventArgs(TValidationResult OldValidationResult, TValidationResult ValidationResult)
        {
            this.OldValidationResult = OldValidationResult;
            this.ValidationResult = ValidationResult;
        }
	}
}
