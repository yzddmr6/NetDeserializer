using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

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

            //string jsonString = JsonConvert.SerializeObject(testClass);

            var serSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.All
            };
            string jsonString = JsonConvert.SerializeObject(testClass, serSettings);
            Console.WriteLine(jsonString);
            return jsonString;
        }

        static void jsonDser(string jsonString)
        {

            var jsonSet = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
                //TypeNameHandling = TypeNameHandling.All
            };

            Object obj = JsonConvert.DeserializeObject(jsonString, jsonSet);

            Type t = obj.GetType();
            PropertyInfo propertyName = t.GetProperty("Name");
            object objname = propertyName.GetValue(obj, null);

        }
        static void Main(string[] args)
        {
            string jsonString = jsonSer();
            jsonDser(jsonString);
        }
    }
}
