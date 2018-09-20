using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace EnhancedDisastersMod
{
    public class DisastersContainer
    {
        private static string optionsFileName = "EnhancedDisastersModOptions.xml";

        public EnhancedForestFire ForestFire;
        public EnhancedThunderstorm Thunderstorm;
        public EnhancedSinkhole Sinkhole;
        //public EnhancedTornado Tornado;
        //public EnhancedEarthquake Earthquake;
        //public EnhancedTsunami Tsunami;
        //public EnhancedMeteorStrike MeteorStrike;

        [XmlIgnore]
        public List<EnhancedDisaster> AllDisasters = new List<EnhancedDisaster>();

        public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(DisastersContainer));
            TextWriter writer = new StreamWriter(getOptionsFilePath());
            ser.Serialize(writer, this);
            writer.Close();
        }

        public void CheckObjects()
        {
            if (ForestFire == null) ForestFire = new EnhancedForestFire();
            if (Thunderstorm == null) Thunderstorm = new EnhancedThunderstorm();
            if (Sinkhole == null) Sinkhole = new EnhancedSinkhole();

            AllDisasters.Clear();
            AllDisasters.Add(ForestFire);
            AllDisasters.Add(Thunderstorm);
            AllDisasters.Add(Sinkhole);
            //AllDisasters.Add(Tornado);
            //AllDisasters.Add(Earthquake);
            //AllDisasters.Add(Tsunami);
            //AllDisasters.Add(MeteorStrike);
        }

        public static DisastersContainer CreateFromFile()
        {
            string path = getOptionsFilePath();

            if (!File.Exists(path)) return null;

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(DisastersContainer));
                TextReader reader = new StreamReader(path);
                DisastersContainer instance = (DisastersContainer)ser.Deserialize(reader);
                reader.Close();

                instance.CheckObjects();

                return instance;
            }
            catch
            {
                return null;
            }
        }

        private static string getOptionsFilePath()
        {
            //return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Colossal Order", "Cities_Skylines", optionsFileName);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "Colossal Order");
            path = Path.Combine(path, "Cities_Skylines");
            path = Path.Combine(path, optionsFileName);
            return path;
        }
    }
}
