# SummerRest - HTTP Endpoints Visualization Plugin for Unity
A plugin works as Postman, which supports to visualize the structure of your HTTP endpoints and request to them inside Unity

This package also generates boilerplate codes based on your structure to simplify the process of calling HTTP endpoints in Runtime mode


## Installation
***Please do not move the plugin folder to a different position because the plugin embeds UXML files inside custom property drawers (change the path will break the path constants in source)***

### Asset store

### Git

## Definitions

There are a few important definitions in the plugin you should know to easily get acquainted with it

First and foremost, we must know the structure of an endpoint tree
- `Endpoint`: every components below are treated as `Endpoint` (actually they inherit it :))
- `Domain`: This is the root component of a single backend, you may have multiple domains in your project. For example, you have a master service (my-master-service.com) and a storage service (my-storage-service), it possibly comes up with 2 different domains
  - The main reason why I made this component is `API Versioning`: A domain has more than one origin, and you have to select the active origin.

- `Service`: A service is nothing but an Endpoint container, it's only used to build API structure
  - A `Service` is able to have child Services and child Requests
- `Request`: The terminal component of an Endpoint tree (It does not have any children)
  - A `Request` is a HTTP request, that means a Request has `method` and `body`

- Inheriting resource path from parents stands out as the most crucial reason of this plugin. This is a real life example of my working project     


Additionally, you may see these things everywhere in the plugin

-  None, Inherit, Custom, AppendToParent: a field marked with this attribute is able to leverage value from its **closest** parent, you may set it `None` to leave it default
- 

## Getting started

- After installing the plugin, click on the `Tools/SummerRest` to open the plugin window
- The plugin works on an asset named "SummerRestConfiguration", an initializing panel will be shown if the plugin does not detect the asset
- Initially, you need to define at least 1 domain, click on `Add` to create a new domain
- A domain must have at least 1 origin, please note that origins must an absolute URL eg. https://sample.com/api/v1 
- Click on the domain to view its structure, right click on an item of the domain tree view to create/delete its children
- Domain and Service are not callable, only Request offers that feature
  - Name: name of generated class associated with this endpoint [Source Generation]()
  - Path: relative path from its parent
  - Url: absolute url formed from the parents' path and its path
- Click on `Do Request` to call your endpoint in the editor

## Auth

### Configure
- The plugin supports to append auth information to your request
- Click on `Advanced settings` to open the auth settings section
- You will see a list of auth containers, each of them contains a record of key, appender type and auth data
  - Key: used by endpoints to refer the auth container
  - Auth data: the value will be only used in editor requests, this value will be resolved by an [IAuthDataRepository](Runtime/Authenticate/TokenRepositories/IAuthDataRepository.cs) in runtime
  - Appender type: how the auth data will be appended into a request (typically modify the request's header), currently we support BearerToken, Basic(Username/password),... You can make your own appender by
    - Modify params or headers of an endpoint (not reusable)
    - Or implement [IAuthAppender](Runtime/Authenticate/Appenders/IAuthAppender.cs), then the class will be shown in the type dropdown
- For example: if you use BearerToken with data "my-data", every requests refer to this container will be added a header "Authorization":"Bearer my-data"  

### Auth data repository
- Storing your secrets on RAM maybe a bad idea for several reasons: 
  - Can not remember logged in sessions
  - Easy to be exploited by attackers
  - ... I dont know
- The plugin provides a single place resolving your auth data; So a request only keeps auth key and appender type, it needs to query a repository about auth data  
- The default repository bases on PlayerPrefs. But you can implement your own
  - Inherit [IAuthDataRepository](Runtime/Authenticate/TokenRepositories/IAuthDataRepository.cs)
  - Select default repository in the plugin window
  - Or change it in runtime by modifying `IAuthDataRepository.Current`

## Runtime support

### Source generation
- The plugin helps to leverage your structure to automatically generate corresponding source code called in runtime
- The generated source will be structured as what you have designed in Editor. The name of each class reflect on the name of the associated endpoint
```c#
public static class MyDomain {
    public static class MyService1 { ...
    }
    public class MyRequest1 { ...
    }
```
- **Because of C# limitations, we can not have an embedded class having the same name as its parent, so you must manage to avoid the collisions of endpoint names (use distinct names to easily address this problem)**

### Use generated classes
- A class generated from `Request` comes up with some utility methods for calling the endpoint
- First, create a request object by invoking static `Create()` method
- Originally, the request's information is alighted with what you assigned in Editor
- But you can modify them through the object's properties. Please note that, a request object is reusable, you can keep it as a field in your classes
- The plugin supports 3 types of request: data, texture, audio clip. Each of them has 2 versions: Simple (only returns response data) and Detailed
- Normally, generated classes have only coroutine methods. You can enable async methods by add "SUMMER_REST_TASK" compilation symbol and import UniTask package

### Advanced settings

The plugin provides a most common way to deal with HTTP requests. But, you are able to embed your customizations easily 

- Data serializer: the default serializer bases on NewtonSoft package, you can adapt it through the plugin window (Advanced settings section) or `IDataSerializer.Current` 
- IAuthDataRepository: please refer to []
- There are some more considerations like IContentParser, IUrlBuilder... but I do feel there is no need to change their logic