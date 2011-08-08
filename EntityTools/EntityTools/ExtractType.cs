using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RiaServicesContrib
{
    /// <summary>
    /// The type of Extract
    /// </summary>
    public enum ExtractType
    {
        /// <summary>
        /// Extracts the original state of the entity, if the entity is unmodified then this is also the current state.
        /// </summary>
        OriginalState, 
        /// <summary>
        /// Extracts the current state of the entity
        /// </summary>
        ModifiedState, 
        /// <summary>
        /// Extracts only the properties with modified properties
        /// </summary>
        ChangesOnlyState
    };
}
