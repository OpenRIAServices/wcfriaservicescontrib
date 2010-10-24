using System;
using System.Linq;
using System.Reflection;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> :  IDisposable
    {
        public void Dispose() {
            var type = this.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var destructors = type.GetMethods(flags).Where(m => m.IsDefined(typeof(DisposeAttribute), true));

            foreach(var destructor in destructors)
            {
                destructor.Invoke(this, new object[] { });
            }
        }
    }
}
