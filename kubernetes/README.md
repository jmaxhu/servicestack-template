# 关于在 kubernetes 上的发布

kubernetes 部署的时候， mysq， redis是分开部署的，系统中配置了连接 redis 和 mysql 的信息。系统的部署文件中通过环境变量的形式进行替换。

另外数据为听密码因为涉及到敏感信息，所以以 secret 的形式添加到 kubernetes 集群中，再导入到系统部署文件中。

```shell
kubectl create secret generic MyApp-mysql-pass --from-literal=password=MyApp_admin_123@qwe

```

由于系统使用的镜像是在私有仓库中，所以还要设置 docker 验证相关的 secret。名称必须和下面的命名一下，因为系统部署的时候需要用到。

```shell
kubectl create secret docker-registry MyApp-docker-regcred --docker-server=registry.cn-hangzhou.aliyuncs.com --docker-username=maxwell_hu --docker-password=<your-pword> --docker-email=maxwell_hu@163.com
```

然后依次部署 redis， mysql 和 后端api程序。

```shell
kubectl create -f redis-deployment.yml
kubectl create -f mysql-deployment.yml
kubectl create -f api-deployment.yml
kubectl create -f ui-deployment.yml

# 检查运行情况
kubectl get svc
kubectl get po
kubectl get deploy
```

api的部署端口默认为 8082，如果需要调整，可以修改 api-deployment.yml 中的 ports 值。

# 清理

```shell
kubectl delete secret MyApp-mysql-pass

kubectl delete deploy -l app=MyApp
kubectl delete svc -l app=MyApp
kubectl delete pvc -l app=MyApp
```