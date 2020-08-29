using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace JavaScriptSerializer_TestClass
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
            testClass.Name = "aaa";
            testClass.Age = 18;
            testClass.Classname = "bbb";

            JavaScriptSerializer jss = new JavaScriptSerializer(new SimpleTypeResolver());
            var json_req = jss.Serialize(testClass);
            Console.WriteLine(json_req);
            return json_req;
        }

        static void DeserTestClass(string json_req)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer(new SimpleTypeResolver());
            var test = jss.Deserialize<TestClass>(json_req);
            Console.WriteLine(test.Name);
        }

        static void Main(string[] args)
        {
            string json_req = SerTestClass();
            DeserTestClass(json_req);
        }
    }
}
