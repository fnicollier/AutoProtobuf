using AutoProtobuf;
using TagCache.Redis.ProtoBuf;

namespace TagCache.Redis.AutoProtobufAdapter
{
    public class Adapter : ISerializerBuilder
    {
        public void Build<T>()
        {
            SerializerBuilder.Build<T>(); 
        }
    }
}
