# 概述

基于 ServiceStack, .net core 的脚手架项目。

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

用以下命令创建数据库及用户。 数据库外请自行替换。

```sql
CREATE DATABASE [dbname] DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci; 

CREATE USER 'user'@'localhost' IDENTIFIED BY 'password';
CREATE USER 'user'@'%' IDENTIFIED BY 'password';

GRANT ALL ON [dbname].* TO 'user'@'localhost';
GRANT ALL ON [dbname].* TO 'user'@'%';

FLUSH PRIVILEGES;
```