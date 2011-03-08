using System.Collections.Generic;
using System.Reflection;

namespace EntityGraph
{
    public interface IEntityGraphShape
    {
        IEnumerable<PropertyInfo> OutEdges(object entity);
        bool IsElementOf(PropertyInfo edge);
    }
}
