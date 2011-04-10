using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GeneratedMetadataProvider
{
    /// <summary>
    /// Custom type descriptor for generated metadata.
    /// </summary>
    internal class GeneratedMetadataTypeDescriptor : CustomTypeDescriptor
    {
        private Type generatedMetadataType { get; set; }

        public GeneratedMetadataTypeDescriptor(ICustomTypeDescriptor parent, Type type)
            : base(parent)
        {
            this.generatedMetadataType = TypeDescriptorCache.GetGeneratedMetadataType(type);
            if (this.generatedMetadataType != null)
            {
                TypeDescriptorCache.ValidateMetadataType(type, this.generatedMetadataType);
            }
        }

        /// <summary>
        /// Returns the attributes for this type.
        /// </summary>
        /// <returns>The attributes for this type.</returns>
        public override AttributeCollection GetAttributes()
        {
            AttributeCollection existing = base.GetAttributes();
            if (this.generatedMetadataType != null)
            {
                Attribute[] newAttributes = TypeDescriptor.GetAttributes(this.generatedMetadataType).OfType<Attribute>().ToArray<Attribute>();
                existing = AttributeCollection.FromExisting(existing, newAttributes);
            }
            return existing;
        }

        /// <summary>
        /// Returns the list of properties for this type.
        /// </summary>
        /// <returns>The list of properties for this type.</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            return this.GetPropertiesWithMetadata(base.GetProperties());
        }

        /// <summary>
        /// Returns the list of properties for given attribues.
        /// </summary>
        /// <param name="attributes">The attributes to provide properties for.</param>
        /// <returns>The list of properties for this type.</returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return this.GetPropertiesWithMetadata(base.GetProperties(attributes));
        }

        private PropertyDescriptorCollection GetPropertiesWithMetadata(PropertyDescriptorCollection originalCollection)
        {
            if (this.generatedMetadataType != null)
            {
                bool flag = false;
                List<PropertyDescriptor> list = new List<PropertyDescriptor>();
                foreach (PropertyDescriptor descriptor in originalCollection)
                {
                    Attribute[] generatedMetadata = TypeDescriptorCache.GetGeneratedMetadata(this.generatedMetadataType, descriptor.Name);
                    PropertyDescriptor item = descriptor;
                    if (generatedMetadata.Length > 0)
                    {
                        item = new GeneratedMetadataPropertyDescriptor(descriptor, generatedMetadata);
                        flag = true;
                    }
                    list.Add(item);
                }
                if (flag)
                {
                    return new PropertyDescriptorCollection(list.ToArray(), true);
                }
            }
            return originalCollection;
        }        

        private static class TypeDescriptorCache
        {
            private static readonly ConcurrentDictionary<Type, Type> metadataTypeCache = new ConcurrentDictionary<Type, Type>();
            private static readonly ConcurrentDictionary<Tuple<Type, string>, Attribute[]> typeMemberCache = new ConcurrentDictionary<Tuple<Type, string>, Attribute[]>();
            private static readonly ConcurrentDictionary<Tuple<Type, Type>, bool> validatedMetadataTypeCache = new ConcurrentDictionary<Tuple<Type, Type>, bool>();
            private static readonly Attribute[] emptyAttributes = new Attribute[0];

            public static Attribute[] GetGeneratedMetadata(Type type, string memberName)
            {
                Attribute[] customAttributes;
                Tuple<Type, string> key = new Tuple<Type, string>(type, memberName);
                if (!typeMemberCache.TryGetValue(key, out customAttributes))
                {
                    MemberTypes types = MemberTypes.Property | MemberTypes.Field;
                    BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                    MemberInfo element = type.GetMember(memberName, types, bindingAttr).FirstOrDefault<MemberInfo>();
                    if (element != null)
                    {
                        customAttributes = Attribute.GetCustomAttributes(element, true);
                    }
                    else
                    {
                        customAttributes = emptyAttributes;
                    }
                    typeMemberCache.TryAdd(key, customAttributes);
                }
                return customAttributes;
            }

            public static Type GetGeneratedMetadataType(Type type)
            {
                Type metadataClassType = null;
                if (!metadataTypeCache.TryGetValue(type, out metadataClassType))
                {
                    GeneratedMetadataTypeAttribute customAttribute = (GeneratedMetadataTypeAttribute)Attribute.GetCustomAttribute(type, typeof(GeneratedMetadataTypeAttribute));
                    if (customAttribute != null)
                    {
                        metadataClassType = customAttribute.GeneratedMetadataClassType;
                    }
                    metadataTypeCache.TryAdd(type, metadataClassType);
                }
                return metadataClassType;
            }

            private static void CheckGeneratedMetadataType(Type mainType, Type generatedMetadataType)
            {
                HashSet<string> other = new HashSet<string>(from p in mainType.GetProperties() select p.Name);
                IEnumerable<string> first = from f in generatedMetadataType.GetFields() select f.Name;
                IEnumerable<string> second = from p in generatedMetadataType.GetProperties() select p.Name;
                HashSet<string> source = new HashSet<string>(first.Concat<string>(second), StringComparer.Ordinal);
                if (!source.IsSubsetOf(other))
                {
                    source.ExceptWith(other);
                    throw new InvalidOperationException();
                }
            }

            public static void ValidateMetadataType(Type type, Type associatedType)
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(type, associatedType);
                if (!validatedMetadataTypeCache.ContainsKey(key))
                {
                    CheckGeneratedMetadataType(type, associatedType);
                    validatedMetadataTypeCache.TryAdd(key, true);
                }
            }
        }

        /// <summary>
        /// This class combines generated and custom attributes provided by developer.
        /// </summary>
        private class GeneratedMetadataPropertyDescriptor : PropertyDescriptor
        {
            private PropertyDescriptor descriptor;
            private bool isReadOnly;

            public GeneratedMetadataPropertyDescriptor(PropertyDescriptor descriptor, Attribute[] newAttributes)
                : base(descriptor, newAttributes)
            {
                this.descriptor = descriptor;
                ReadOnlyAttribute attribute = newAttributes.OfType<ReadOnlyAttribute>().FirstOrDefault<ReadOnlyAttribute>();
                this.isReadOnly = (attribute != null) ? attribute.IsReadOnly : false;
            }

            public override void AddValueChanged(object component, EventHandler handler)
            {
                this.descriptor.AddValueChanged(component, handler);
            }

            public override bool CanResetValue(object component)
            {
                return this.descriptor.CanResetValue(component);
            }

            public override object GetValue(object component)
            {
                return this.descriptor.GetValue(component);
            }

            public override void RemoveValueChanged(object component, EventHandler handler)
            {
                this.descriptor.RemoveValueChanged(component, handler);
            }

            public override void ResetValue(object component)
            {
                this.descriptor.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                this.descriptor.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return this.descriptor.ShouldSerializeValue(component);
            }

            public override Type ComponentType
            {
                get
                {
                    return this.descriptor.ComponentType;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    if (!this.isReadOnly)
                    {
                        return this.descriptor.IsReadOnly;
                    }
                    return true;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return this.descriptor.PropertyType;
                }
            }

            public override bool SupportsChangeEvents
            {
                get
                {
                    return this.descriptor.SupportsChangeEvents;
                }
            }
        }
    }
}
