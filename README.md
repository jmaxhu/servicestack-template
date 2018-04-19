# 概述

基于 ServiceStack, .net core 的脚手架项目。

# 安装

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

# 测试

```shell
dotnet test MyApp.Test/
```

# 数据库

用以下命令创建数据库及用户。 数据名外请自行替换。

```sql
CREATE DATABASE [dbname] DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci; 

CREATE USER 'user'@'localhost' IDENTIFIED BY 'password';
CREATE USER 'user'@'%' IDENTIFIED BY 'password';

GRANT ALL ON [dbname].* TO 'user'@'localhost';
GRANT ALL ON [dbname].* TO 'user'@'%';

FLUSH PRIVILEGES;
```