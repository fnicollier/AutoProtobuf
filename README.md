AutoProtobuf
============

A .NET library to automatically setup protobuf serializations on types without the need to use attributes.


##Usage

Simply call the following once in your process before serializing with ProtoBug:

```c#
SerializerBuilder.Build(complexClass);
```

##Known limitations
Doesn't works with DataTables, please use [protobuf-net-data](http://www.nuget.org/packages/protobuf-net-data) for serializing.

This is also a very early release so use with caution.
