# 概述

基于 ServiceStack, .net core 的脚手架项目。

# 安装

## 安装 .net core sdk
首先到 [.net core](https://www.microsoft.com/net/download/all) 页面下载最新的 2.1 版本的 sdk。

## 下载项目
该脚手架基于 servicestack cli，使用前请先安装 node 及 servicestack/cli 的 npm 包。

```shell
npm install -g @servicestack/cli

dotnet-new https://github.com/jmaxhu/servicestack-template [project name]
```

# 开发

用以下命令开启开发程序。支持代码变更的自动编译。

```shell
ASPNETCORE_ENVIRONMENT=Development dotnet watch run
```

或者通过写好的脚本来执行。

```shell
cd MyApp
./run.sh
```

# 测试

```shell
dotnet test -v=m MyApp.Test/
```

# 数据库

用以下命令创建数据库及用户。 数据名外请自行替换。

```sql
CREATE DATABASE MyApp_DB DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci; 

CREATE USER 'myapp_admin'@'localhost' IDENTIFIED BY 'myapp_admin_123@qwe';
CREATE USER 'myapp_admin'@'%' IDENTIFIED BY 'myapp_admin_123@qwe';

GRANT ALL ON MyApp_DB.* TO 'myapp_admin'@'localhost';
GRANT ALL ON MyApp_DB.* TO 'myapp_admin'@'%';

FLUSH PRIVILEGES;
```

# 关于发布

这里的发布分为测试发布和生产发布。发布工具使用 gitlab ci。

## 测试发布

每次代码提交（master分支）时，gitlab ci 都会自动编译系统，并把结果复制到指定目录，该目录通过 docker 的路径映射功能从 gitlab runner 环境复制到 host 环境。把对应的文件覆盖即可。

## 生产发布

当 realese 分支有提交时，gitlab ci 会执行生产发布的脚本。发布使用 docker build 来创建镜像，再通过 docker push 推送到指定的仓库中。对于私有仓库需要登录，所以要先通过 docker login 登录，注意登录使用的用户名和密码，通过 gitlab ci 提供的 secript veriables 功能来保护和存储，避免把账号信息写在脚本里。**注意：受保护的变量使用时需要把分支设置成保护分支才会传递。**