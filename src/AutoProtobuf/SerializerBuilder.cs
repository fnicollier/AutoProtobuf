using System;
using System.Data;
using System.Linq;
using System.Reflection;
using ProtoBuf.Meta;

namespace AutoProtobuf
{
    public static class SerializerBuilder
    {
        /// <summary>
        /// Build the ProtoBuf serializer from the generic <see cref="Type">type</see>.
        /// </summary>
        /// <typeparam name="T">The type of build the serializer for.</typeparam>
        public static void Build<T>()
        {
            var type = typeof(T);
            Build(type);
        }

        /// <summary>
        /// Build the ProtoBuf serializer from the data's <see cref="Type">type</see>.
        /// </summary>
        /// <typeparam name="T">The type of build the serializer for.</typeparam>
        /// <param name="data">The data who's type a serializer will be made.</param>
        // ReSharper disable once UnusedParameter.Global
        public static void Build<T>(T data)
        {
            Build<T>();
        }

        /// <summary>
        /// Build the ProtoBuf serializer for the <see cref="Type">type</see>.
        /// </summary>
        /// <param name="type">The type of build the serializer for.</param>
        public static void Build(Type type)
        {
            const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            if (RuntimeTypeModel.Default.CanSerialize(type))
            {
                return;
            }

            if (type == typeof(DataTable))
            {
                return;
            }

            var meta = RuntimeTypeModel.Default.Add(type, false);
            var properties = type.GetProperties(flags).Where(p => p.GetSetMethod() != null).ToList();
            var fields = type.GetFields(flags).ToList();
            
            meta.Add((properties.Select(p => p.Name).Distinct().Concat(fields.Select(m => m.Name).Distinct()).ToArray()));
            meta.UseConstructor = false;

            BuildGenerics(type);

            foreach (var memberType in (properties.Select(p => p.PropertyType).Concat(fields.Select(f => f.FieldType)).Where(t => !t.IsPrimitive)))
            {
                Build(memberType);
            }
        }
        
        private static void BuildGenerics(Type type)
        {
            if (type.IsGenericType)
            {
                var generics = type.GetGenericArguments();

                foreach (var generic in generics)
                {
                    Build(generic);
                }
            }
        }
    }
}
