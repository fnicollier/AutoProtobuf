using TagCache.Redis.ProtoBuf;

namespace TagCache.Redis.AutoProtobuf
{
    public class SerializerBuilder : ISerializerBuilder
    {
        public void Build<T>()
        {
            global::AutoProtobuf.SerializerBuilder.Build<T>(); 
        }
    }
}
