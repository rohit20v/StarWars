using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Windows.Storage;

namespace StarWars.Utils
{
    internal class XmlWriter
    {
        public static void WriteXmlFile<T>(List<T> contentList, string fileName)
        {
            var file = ApplicationData.Current.LocalFolder.CreateFileAsync(fileName,
                CreationCollisionOption.ReplaceExisting).GetAwaiter().GetResult();

            using var stream = file.OpenStreamForWriteAsync().GetAwaiter().GetResult();
            var serializer = new XmlSerializer(typeof(List<T>));
            serializer.Serialize(stream, contentList);
        }
    }
}
