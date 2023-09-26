# QuickBuild3libProj
痛点:使用c++开发过程中;尽管CMAKE很强大,方便帮助我们编译好第三方库,但是对第三方库的管理,以及快速搭建开发环境,以及第三方库互相搭配使用并进行demo验证上个人觉得,还是繁琐! 所以写了个小工具,来省略这个过程;
这个工具长这个样子:

他能做什么:
1.只需要勾勾选选,就可以将用visutual studio创建的c++ 项目或者库; 将库环境一键写入;
2.快速创建包组织目录:



3.可以快速保存自己的环境配置文件,比如下面这个: 如果下次需要快速配置环境,就使用这个配置文件,直接写入;




目前的几个案例:

1.opencv简单的环境搭配:




2.osgEarthu快速环境搭建:



后话:这个工具只是减少环境搭配的繁琐步骤;比如debug,release ,x64,x86等等;

git地址: https://gitee.com/hong_go_4/XPloteQuickBuidProj.git

![image](https://github.com/1246498943/QuickBuild3libProj/assets/37721652/850ac064-1315-4ed8-b57f-156824eda694)

