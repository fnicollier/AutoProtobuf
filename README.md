AutoProtobuf
============

A .NET library to automatically setup protobuf serializations on types without the need to use attributes.


##Usage

Simply call the following once in your process before serializing with ProtoBug:

```c#
SerializerBuilder.Build(complexClass);
```

If the class already has a serializer generated, the call does nothing.

##What can it do?

Generates serializers for any type protobuf-net can serialize. It does this by calling protobuf-net's RuntimeTypeModel and adding all fields to the serializaton model for each class. 

It will navigate an object's inheritance tree so that inherited fields are also serialized properly.

##Known limitations
Doesn't works with DataTables, please use [protobuf-net-data](http://www.nuget.org/packages/protobuf-net-data) for serializing those.

This is also a very early release so use with caution.
