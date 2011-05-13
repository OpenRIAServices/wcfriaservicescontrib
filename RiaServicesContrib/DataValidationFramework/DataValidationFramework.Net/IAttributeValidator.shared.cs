namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Interface for attribute validators. They have a validate method with a fixed signature.
    /// </summary>
    public interface IAttributeValidator
    {
        /// <summary>
        /// Validation method for attribute validators.
        /// </summary>
        /// <param name="value"></param>
        void Validate(object value);
    }
}
