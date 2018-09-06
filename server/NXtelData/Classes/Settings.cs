﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NXtelData
{
    public class Settings
    {
        private string appDir;

        public string ConnectionString { get; set; }

        public Settings()
        {
        }

        public Settings(string AppDir)
        {
            appDir = AppDir;
        }

        public Settings Load()
        {
            Settings settings;
            try
            {
                string xml = File.ReadAllText(FileName);
                StringReader reader = new StringReader(xml);
                using (reader)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(reader);
                    reader.Close();
                }
                settings.appDir = appDir;
            }
            catch
            {
                settings = new Settings(appDir);
            }
            return settings;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            TextWriter writer = new StreamWriter(FileName);
            using (writer)
            {
                serializer.Serialize(writer, this);
                writer.Close();
            }
        }

        private string FileName
        {
            get
            {
                return Path.Combine(appDir, "Settings.xml");
            }
        }
    }
}