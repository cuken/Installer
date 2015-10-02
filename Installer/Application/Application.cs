using Installer.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Installer.Application
{
    public class Application
    {
        public string Name;
        public string IniPath;
        public string Directory;
        WTC wtc = new WTC();
        INI.IniFile iniFile;

        public Application(string name, string iniPath)
        {
            Name = name;            
            IniPath = iniPath;
            Directory = iniPath.Substring(0, iniPath.LastIndexOf('\\'));
            iniFile = new INI.IniFile(iniPath);
        }

        public void ReadINI()
        {            
            foreach(INI.IniSection iniSection in iniFile.Sections)
            {
                Console.BackgroundColor = ConsoleColor.White;
                wtc.WriteBlackLine(iniSection.Name);
                Console.BackgroundColor = ConsoleColor.Black;
                foreach(INI.IniProperty properties in iniSection.Properties)
                {
                    wtc.WriteBlue(properties.Name + " = ");
                    wtc.WriteWhiteLine(properties.Value);                    
                }                
            }
        }

        public void ChangeINI(string[] input)
        {
            bool sectionFound = false;
            bool propertyFound = false;
            //"Modify [Section] [Property] [Value]"
            //[0]       [1]         [2]      [3],[4]...
            foreach (INI.IniSection Section in iniFile.Sections)
            {                
                if(string.Equals(Section.Name.Trim(), input[1], StringComparison.CurrentCultureIgnoreCase))
                {
                    sectionFound = true;     
                    foreach (INI.IniProperty property in Section.Properties)
                    {
                        if (string.Equals(property.Name.Trim(), input[2], StringComparison.CurrentCultureIgnoreCase))
                        {
                            propertyFound = true;
                            string value = "";
                            for (int i = 3; i < input.Length; i++)
                            {
                                //Reconstructing the input array;
                                value += input[i] + " ";                                                                
                            }
                            value.TrimEnd();
                            iniFile.Section(Section.Name).Set(property.Name, value, "Changed by Installer");
                            wtc.WriteGreenLine("Modified INI -> OK");
                            iniFile.Save(IniPath);
                        }
                    }
                    if(!propertyFound)
                    {
                        wtc.WriteRedLine("Unable to find " + input[2] + " in INI file");
                    }
                }                
            }
            if(!sectionFound)
            {
                wtc.WriteRedLine("Unable to find " + input[1] + " in INI file");
            }
        }
    }

    public static class ApplicationCollection
    {
        static ICollection<Application> applications = new List<Application>();

        public static void AddApplication(Application app)
        {
            applications.Add(app);
        }

        public static void RemoveApplication(Application app)
        {
            applications.Remove(app);
        }

        public static Application GetSpecificApplication(string app)
        {
            foreach(Application p in applications)
            {
                if(app.ToLower().Trim() == p.Name.ToLower().Trim())
                {
                    return p;
                }
            }
            return null;
        }

        public static ICollection<Application> GetApplications()
        {
            return applications;
        }

    }
}
