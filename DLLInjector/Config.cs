using System;
using System.IO;
using Microsoft.Win32;
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
        DateTime localDate = DateTime.Now;
        string Cfg = "InjectorCfg.ini";
        public string DLLPath = "";
        public string PrcName = "";
        public bool console = false;
        public bool Close = false;

        public void CheckConfig()
        {
            if (File.Exists(Cfg))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Cfg);
                DLLPath = data["Configuration"]["Dll"];
                PrcName = data["Configuration"]["Process"];
                console = Convert.ToBoolean(data["Configuration"]["Console"]);
                Close = Convert.ToBoolean(data["Configuration"]["Close"]);                
            }
            else
            {
                CreateConfig();
            }            
        }
        public void SaveConfig()
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = new IniData();
                data["Configuration"]["Dll"] = DLLPath;
                data["Configuration"]["Process"] = PrcName;
                data["Configuration"]["Console"] = console.ToString();
                data["Configuration"]["Close"] = Close.ToString();
                parser.WriteFile(Cfg, data);
                Console.WriteLine(localDate.ToShortTimeString() + " - Config saved");
            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(localDate.ToShortTimeString() + " - Exception: " + ex.ToString());
                Console.ResetColor();
            }
           
        }
        void CreateConfig()
        {
            try
            {
                File.Create(Cfg).Dispose();

                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(Cfg);
                data.Sections.AddSection("Configuration");
                data["Configuration"].AddKey("Dll", "");
                data["Configuration"].AddKey("Process", "");
                data["Configuration"].AddKey("Console", console.ToString());
                data["Configuration"].AddKey("Close", Close.ToString());
                parser.WriteFile(Cfg, data);
                Console.WriteLine(localDate.ToShortTimeString() + " - Config created");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(localDate.ToShortTimeString() + " - Exception: " + ex.ToString());
                Console.ResetColor();
            }
            
        }
    }

    
}
