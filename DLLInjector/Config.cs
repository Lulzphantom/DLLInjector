using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace DLLInjector
{
    public class Config
    {
        string Cfg = "InjectorCfg.ini";
        public string DLLPath = "";
        public string PrcName = "";
        public bool Console = false;
        public bool Close = false;

        public void CheckConfig()
        {
            if (File.Exists(Cfg))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Cfg);
                DLLPath = data["Configuration"]["Dll"];
                PrcName = data["Configuration"]["Process"];
                Console = Convert.ToBoolean(data["Configuration"]["Console"]);
                Close = Convert.ToBoolean(data["Configuration"]["Close"]);                
            }
            else
            {
                CreateConfig();
            }            
        }
        public void SaveConfig()
        {
            var parser = new FileIniDataParser();
            IniData data = new IniData();
            data["Configuration"]["Dll"] = DLLPath;
            data["Configuration"]["Process"] = PrcName;
            data["Configuration"]["Console"] = Console.ToString();
            data["Configuration"]["Close"] = Close.ToString();

            parser.WriteFile(Cfg, data);
        }
        void CreateConfig()
        {
            File.Create(Cfg).Dispose();

            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Cfg);            
            data.Sections.AddSection("Configuration");
            data["Configuration"].AddKey("Dll", "");
            data["Configuration"].AddKey("Process", "");
            data["Configuration"].AddKey("Console", Console.ToString());
            data["Configuration"].AddKey("Close", Close.ToString());

            parser.WriteFile(Cfg, data);
        }
    }

    
}
