DevOps C# Task
==============
Приложение включает в себя три модуля:
--------------------------------------

1. **Config** - содержит модели конфигов, с возможностью их сериализации/десериализации из/в json, а так же GitClient для их добавление в git репозиторий.
2. **TpRest** - аутентификация через _TargetProcess REST API_. Настороена по умолчанию на тестовый акаунт sldemo.tpondemand.com с юзером admin/admin
3. **Web** - веб-интерфейс к конфигам (_AspNet.Mvc 4_)

Для управления зависимостями используется **nuget**
.

Описание конфигов:
------------------
Пример git репозитория конфигов располагается: **Web\App_Data\base**, если его клонировать, то можно увидеть две паки: 
+ **servers** содержит конфигурации серверов. Главный аттрибут **roles** определяет роль сервера, бывают два типа **app** и **db**. Имя файла (до .json.cfg) является идентификатором сервера. 
+ **accounts** содержит конфигурации акаунтов. Каждый акаунт связан с одним **app** и одним **db** сервером, которые прописываются в соответствующие аттрибуты. Имя файла (до .json.cfg) является идентификатором акаунта.

В **Web\web.config** важными являются пути к репозиториям
```
<add key="GitFolder" value="d:/TargetProcess/Control/Web/App_Data/rep" />
<add key="GitUrl" value="d:/TargetProcess/Control/Web/App_Data/base" />
```

**GitUrl** - отсюда приложение будет делать clone/fetch git репозитория с конфигами в папку заданную в **GitFolder**

В **текущей реализации** приложение показывает список акаунтов, конфигурации которых есть в репозитории.

###Необходимо реализовать:
+ возможность в web интерфейсе изменить db сервер для аккаунта
+ выпадающий список для выбора сервера
+ фильтр (включать и выключать возможность переноса с ams в dal. по умолчанию - отключено)

+ puppet модуль, который проверит наличие iis, .net и всех необходимых зависимостей на севере для этого приложения. Модуль также должен проверять, работает ли приложение.
+ написать powershell скрипт, который установит этот приложение на iis (создать application pool, site)