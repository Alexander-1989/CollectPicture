using System;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace CollectPicture
{
    class Config
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "Config.xml");
        private readonly XmlSerializer serrializer = new XmlSerializer(typeof(Fields));
        public Fields fields = null;

        public void Open()
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    fields = serrializer.Deserialize(fs) as Fields;
                }
            }
            else
            {
                fields = new Fields();
            }
        }

        public void Save()
        {
            if (fields != null)
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    serrializer.Serialize(fs, fields);
                }
            }
        }
    }

    [Serializable]
    public class Fields
    {
        public Point Location { get; set; } = new Point(0, 0);
        public string PicturesFolder { get; set; } = ".\\Pictures";
        public string SoundsFolder { get; set; } = ".\\Sounds";
        public string[] Extensions = new string[] { ".jpg", ".jpeg", ".bmp", ".png" };
    }
}