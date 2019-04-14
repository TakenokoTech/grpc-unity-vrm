# Google.Protobuf
Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Google.Protobuf/3.7.0" -OutFile "./proto.zip"
Expand-Archive -Path "./proto.zip" -DestinationPath "proto/" -Force
Remove-Item -Path "./proto.zip"

Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Grpc/1.20.0-pre3" -OutFile "./grpc.zip"
Expand-Archive -Path "./grpc.zip" -DestinationPath "grpc/" -Force
Remove-Item -Path "./grpc.zip"

$archive = "https://packages.grpc.io/archive/2019/04/d418c42f1ef2f433df54aaee42c75f8d58742927-d3100d9a-d79a-41d7-9cd5-11d7fe0dd503"
Invoke-WebRequest -Uri "$archive/protoc/grpc-protoc_windows_x64-1.21.0-dev.zip" -OutFile "./grpc-protoc.zip"
Expand-Archive -Path "./grpc-protoc.zip" -DestinationPath "grpc-protoc/" -Force
Remove-Item -Path "./grpc-protoc.zip"

# make proto
protoc.exe sample.proto         `
    --proto_path=.              `
    --csharp_out=.              `
    --grpc_out=.                `
    --plugin=protoc-gen-grpc=./grpc-protoc/grpc_csharp_plugin.exe