using System;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace CollectPicture
{
    class Config
    {
        public Fields fields = null;
        private readonly XmlSerializer serrializer = new XmlSerializer(typeof(Fields));
        private readonly string fileName = Path.Combine(Environment.CurrentDirectory, "Config.xml");

        public void Read()
        {
            if (!File.Exists(fileName))
            {
                fields = new Fields();
            }
            else
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fields = serrializer.Deserialize(fs) as Fields;
                }
            }
        }

        public void Write()
        {
            if (fields != null)
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    serrializer.Serialize(fs, fields);
                }
            }
        }
    }

    [Serializable]
    public class Fields
    {
        public Point Location { get; set; } = new Point();
        public string PicturesFolder { get; set; } = ".\\Pictures";
        public string SoundFile { get; set; } = ".\\Sounds\\Click.wav";
        public string[] Extensions = new string[] { ".jpg", ".jpeg", ".bmp", ".png" };
    }
}