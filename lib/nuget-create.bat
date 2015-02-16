nuget pack ../src/AutoProtobuf/AutoProtobuf.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget

nuget pack ../src/TagCache.Redis.AutoProtobuf/TagCache.Redis.AutoProtobuf.csproj -Build -Symbols -Properties Configuration=Release -OutputDirectory nuget