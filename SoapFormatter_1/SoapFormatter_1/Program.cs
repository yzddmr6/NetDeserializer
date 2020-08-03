using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Soap;

namespace SoapFormatter_1
{

    [Serializable]
    public class TestClass
    {
        private string classname;
        private string name;
        private int age;
        public string Classname { get => classname; set => classname = value; }
        public string Name { get => name; set => name = value; }
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
            testClass.Age = 18;
            testClass.Name = "test";
            testClass.Classname = "333";
            FileStream stream = new FileStream("./soap.xml", FileMode.Create);
            SoapFormatter bFormat = new SoapFormatter();
            bFormat.Serialize(stream, testClass);
            stream.Close();
        }

        static void DeserTestClass()
        {
            FileStream stream = new FileStream("./soap.xml", FileMode.Open);
            SoapFormatter bFormat = new SoapFormatter();
            var person = bFormat.Deserialize(stream);
            Console.WriteLine(((TestClass)person).Name);
            stream.Close();
        }

        static void Main(string[] args)
        {
            //SerTestClass();
            DeserTestClass();
        }
    }
}
