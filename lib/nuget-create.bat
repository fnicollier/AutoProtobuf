nuget pack ../src/AutoProtobuf/AutoProtobuf.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget

nuget pack ../src/TagCache.Redis.AutoProtobufAdapter/TagCache.Redis.AutoProtobufAdapter.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget