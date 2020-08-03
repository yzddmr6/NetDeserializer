using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using fastJSON;

namespace Fastjson_TestClass
{
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
        static string SerTestClass()
        {
            TestClass testClass = new TestClass();
            testClass.Classname = "fff";
            testClass.Name = "aaa";
            testClass.Age = 18;

            JSONParameters jsonPara = new JSONParameters
            {
                UseExtensions = true,
            };

            string instance = JSON.ToJSON(testClass, jsonPara);
            Console.WriteLine(instance);
            return instance;
        }

        static void DeserTestClass(string jsonObj)
        {
            JSONParameters jsonPara = new JSONParameters
            {
                UseExtensions = true,
            };

            //var instance = JSON.ToObject(jsonObj, jsonPara);
            var instance = JSON.ToObject(jsonObj);
            Type t = instance.GetType();
            PropertyInfo propertyName = t.GetProperty("Name");
            object objName = propertyName.GetValue(instance, null);
            Console.WriteLine(objName);
        }

        static void Main(string[] args)
        {
            string jsonObj = SerTestClass();
            DeserTestClass(jsonObj);

        }
    }
}
