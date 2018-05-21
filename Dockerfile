# 对应 microsoft/aspnetcore:2，因为网络原因使用aliyun的容器服务
FROM registry.cn-hangzhou.aliyuncs.com/jmax/aspnetcore:2

WORKDIR /app

ADD ./publish .

EXPOSE 80

CMD ["dotnet", "MyApp.dll"]