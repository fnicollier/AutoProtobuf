using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using ProtoBuf.Data;
using Newtonsoft.Json;
using ProtoBuf.Meta;
using DeepEqual.Syntax;

namespace AutoProtobuf.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private static List<SimpleClass> GenerateSimpleClassList()
        {
            var list = new List<SimpleClass>();

            for (int i = 0; i < 200; i++)
            {
                list.Add(new SimpleClass
                {
                    Id = i,
                    Name = String.Format("Test class {0}", i)
                });
            }
            return list;
        }

        [TestMethod]
        public void SerializeSimpleClass()
        {
            var simpleClass = new SimpleClass
            {
                Id = 1,
                Name = "Test1"
            };
            
            SerializerBuilder.Build(simpleClass);

            var result = Serializer.DeepClone(simpleClass);

            Assert.AreEqual(simpleClass.Id, result.Id);
            Assert.AreEqual(simpleClass.Name, result.Name);
        }

        [TestMethod]
        public void SerializeSimpleClassList()
        {
            var list = GenerateSimpleClassList();

            SerializerBuilder.Build(list);

            var result = Serializer.DeepClone(list);

            Assert.AreEqual(list[100].Id, result[100].Id);
            Assert.AreEqual(list[100].Name, result[100].Name);
        }

        [TestMethod]
        public void SerializeComplexClass()
        {
            var complexClass = new ComplexClass<SimpleClass>(Guid.NewGuid());
            complexClass.Values = GenerateSimpleClassList();

            SerializerBuilder.Build(complexClass);

            var result = Serializer.DeepClone(complexClass);

            Assert.AreEqual(complexClass.Key, result.Key);
            Assert.AreEqual(complexClass.Values[100].Id, result.Values[100].Id);
            Assert.AreEqual(complexClass.Values[100].Name, result.Values[100].Name);
        }

        [TestMethod]
        public void SerializeDataTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("A", typeof (int));
            dataTable.Columns.Add("B", typeof(string));
            dataTable.Rows.Add(1, "Hello");

            using (var stream = new MemoryStream())
            {
                DataSerializer.Serialize(stream, dataTable);
                stream.Seek(0, SeekOrigin.Begin);
                var result = DataSerializer.DeserializeDataTable(stream);

                Assert.AreEqual(dataTable.Rows[0]["A"], result.Rows[0]["A"]);
                Assert.AreEqual(dataTable.Rows[0]["B"], result.Rows[0]["B"]);
            }
        }

        [TestMethod]
        public void Given_a_hiererchical_structure_should_serialize_and_deserialize_correctly()    
        {
            var bobby = new Person { Name = "Bobby" };
            var johnny = new Person { Name = "Johnny", Child = bobby };
            bobby.Parent = johnny;
            var marty = new Person { Name = "Marty", Parent = bobby };
            bobby.Child = marty;

            var people = new List<Person> { bobby, johnny, marty };

            var refTypes = new [] {typeof(Person)};

            SerializerBuilder.Build<List<Person>>(x => refTypes.Contains(x));

            var result = Serializer.DeepClone(people);

            Assert.AreEqual(people.Count, result.Count);
            for (int i = 0; i < people.Count; i++)
            {
                Assert.AreEqual(people[i].Name, result[i].Name);          
            }
        }

        public class Person
        {
            public Person Parent { get; set; }
            public Person Child { get; set; }
            public string Name { get; set; }
        }
    }
}
