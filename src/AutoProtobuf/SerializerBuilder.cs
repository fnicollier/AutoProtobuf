using System;
using System.Collections.Generic;
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

            lock (type)
            {
                if (RuntimeTypeModel.Default.CanSerialize(type))
                {
                    //Dictionaries report they can serialize when their generic parameters aren't serializable yet
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        BuildGenerics(type);
                    }

                    return;
                }

                var meta = RuntimeTypeModel.Default.Add(type, false);
                var fields = type.GetFields(flags).ToList();

                meta.Add(fields.Select(m => m.Name).ToArray());
                meta.UseConstructor = false;

                BuildGenerics(type);

                foreach (var memberType in fields.Select(f => f.FieldType).Where(t => !t.IsPrimitive))
                {
                    Build(memberType);
                }
            }
        }

        private static void BuildGenerics(Type type)
        {
            if (type.IsGenericType || (type.BaseType != null && type.BaseType.IsGenericType))
            {
                var generics = type.IsGenericType ? type.GetGenericArguments() : type.BaseType.GetGenericArguments();

                foreach (var generic in generics)
                {
                    Build(generic);
                }
            }
        }
    }
}
