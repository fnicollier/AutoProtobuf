using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;

namespace AutoProtobuf
{
    public static class SerializerBuilder
    {
        const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        static readonly Dictionary<Type, List<Type>> subTypes = new Dictionary<Type, List<Type>>();

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
            lock (type)
            {
                if (RuntimeTypeModel.Default.CanSerialize(type))
                {
                    if (type.IsGenericType)
                    {
                        BuildGenerics(type);
                    }

                    return;
                }

                var meta = RuntimeTypeModel.Default.Add(type, false);
                var fields = GetMembers(type);

                meta.Add(fields.Select(m => m.Name).ToArray());
                meta.UseConstructor = false;

                BuildBaseClasses(type, meta, fields.Count);

                BuildGenerics(type);

                foreach (var memberType in fields.Select(f => f.FieldType).Where(t => !t.IsPrimitive))
                {
                    Build(memberType);
                }
            }
        }

        private static List<FieldInfo> GetMembers(Type type)
        {
            return type.GetFields(flags).ToList();
        }

        private static void BuildBaseClasses(Type type, MetaType meta, int fieldCount)
        {
            var baseType = type.BaseType;
            var inheritingType = type;

            while (baseType != null && baseType != typeof(object))
            {
                List<Type> baseTypeEntry;

                if (!subTypes.TryGetValue(baseType, out baseTypeEntry))
                {
                    baseTypeEntry = new List<Type>();
                    subTypes.Add(baseType, baseTypeEntry);
                }

                if (!baseTypeEntry.Contains(inheritingType))
                {
                    Build(baseType);
                    RuntimeTypeModel.Default[baseType].AddSubType(baseTypeEntry.Count + 500, inheritingType);
                    baseTypeEntry.Add(inheritingType);
                }

                inheritingType = baseType;
                baseType = baseType.BaseType;
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
