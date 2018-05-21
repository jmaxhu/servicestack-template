rm -rf publish

dotnet publish MyApp/MyApp.csproj -c Release -f netcoreapp2.0 -o ../publish

 docker build -t registry.cn-hangzhou.aliyuncs.com/jmax/my_app:v0.1.0 .