using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;

namespace DataContractSerializer_TestClass
{
    [DataContract]
    public class TestClass
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

        public static void ClassMethod(string value)
        {
            Process.Start(value);
        }
    }

    class Program
    {
        static void SerTestClass()
        {
            TestClass testClass = new TestClass();
            testClass.Name = "aaa";
            testClass.Age = 18;
            testClass.Classname = "bbb";

            using (FileStream stream = new FileStream("./test.xml", FileMode.Create))
            {
                DataContractSerializer jsonSer = new DataContractSerializer(testClass.GetType());
                jsonSer.WriteObject(stream, testClass);
            }
        }

        static void DeserTestClass()
        {
            using (FileStream stream = new FileStream("./test.xml", FileMode.Open))
            {
                DataContractSerializer jsonSer = new DataContractSerializer(typeof(TestClass));
                TestClass obj = (TestClass)jsonSer.ReadObject(stream);
                Console.WriteLine(obj.Name);
            }
        }

        static void Main(string[] args)
        {
            //SerTestClass();
            DeserTestClass();
        }
    }
}
