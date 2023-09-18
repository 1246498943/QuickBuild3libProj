using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPloteQuickBuidProj
{
    public class JsonSerialFileHelper
    {
        public static void Write2File<T>(T obj,string serialFile)
        {
            if (obj==null) return;
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            setting.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            setting.TypeNameHandling = TypeNameHandling.Auto;
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, setting);
            Console.WriteLine(json);
            System.IO.File.WriteAllText(serialFile, json);
        }

        public static T ReadDataFromFile<T>(string FilePath)
        {
            if (!File.Exists(FilePath)) return default(T);
            //反向序列化.
            var content = File.ReadAllText(FilePath);
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            setting.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            setting.TypeNameHandling = TypeNameHandling.Auto;
            T result = JsonConvert.DeserializeObject<T>(content, setting);
            return result;
        }

    }
}
