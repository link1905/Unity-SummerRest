This project show simple usages of the plugin [SummerRest](https://github.com/risethesummer/Unity-SummerRest), because jumping into this project, please make sure that you've had a look at [README.md](../../README.md)

First and foremost, click on `Tools/SummerRest` to view the predefined endpoints of the sample

To use the generated requests, there are 2 notable folders under the Asset folder: 
- `Model`: Request and response data containers. Because of using [Unity Serialization](https://docs.unity3d.com/Manual/script-Serialization.html), please remember to use `System.Serializable` attribute (and [SerializeField](https://docs.unity3d.com/ScriptReference/SerializeField.html) with private fields)
- `Auth`: [DummyJsonApiAuthAppender](Scripts/Auth/DummyJsonApiAuthAppender.cs) shows how to implement a custom auth appender
- `Managers`
  1. [SampleCoroutineManager](Scripts/Managers/SampleCoroutineManager.cs) shows how to call HTTP endpoints by leveraging `Coroutine` system
  2. [SampleTaskManager](Scripts/Managers/SampleTaskManager.cs) shows how to call HTTP endpoints asynchronously by using [UniTask](https://github.com/Cysharp/UniTask). To enable the code belonging to this section, you need to
     1. Import [UniTask](https://github.com/Cysharp/UniTask) package
     2. Add **SUMMER_REST_TASK** [Scripting Define Symbol](https://docs.unity3d.com/Manual/CustomScriptingSymbols.html) (briefly, you may find it at `Player settings` > `Other settings` > `Script compilation`)
     ![](../../Screenshots/3_source_2_symbol.png)
  3. In the end part of the manager classes, you will figure out to call independent requests which are not configured in the window   

[summer-rest-generated.RestSourceGenerator](Configures/summer-rest-generated.RestSourceGenerator.additionalfile) is an additional file for generating code, please click on `Generate source to` button to refresh the file and its associated source everytime you want to sync the plugin configures with your project 
