using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace StarWars.Utils
{
    internal class JsonWriter
    {
        public static void WriteJsonFile<T>(List<T> contentList, string fileName)
        {
            var file = ApplicationData.Current.LocalFolder.CreateFileAsync(fileName,
                CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();
            FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(contentList)).GetAwaiter().GetResult();
        }
    }
}
