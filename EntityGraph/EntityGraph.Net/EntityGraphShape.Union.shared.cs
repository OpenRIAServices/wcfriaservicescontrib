using System.Linq;

namespace RiaServicesContrib
{
    public static partial class EntityGraphShapeExtensions
    {
        /// <summary>
        /// Produces the set union of two entity graph shapes by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TEntityGraphShape"></typeparam>
        /// <param name="current"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntityGraphShape Union<TEntityGraphShape>(this TEntityGraphShape current, EntityGraphShape shape)
            where TEntityGraphShape : EntityGraphShape
        {
            foreach (var edge in shape)
            {
                if (current.Contains(edge) == false)
                {
                    current.edges.Add(edge);
                }
            }
            return current;
        }
    }
}
