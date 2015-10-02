using System;
using Installer.Application;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Installer.Utilities
{
    public static class Menu
    {
        static WTC wtc = new WTC();
        static Application.Application main;

        static public void Help()
        {
            Console.BackgroundColor = ConsoleColor.White;            
            wtc.WriteBlackLine("[ Help ]");
            Console.BackgroundColor = ConsoleColor.Black;
            wtc.WriteBlue("Help | ?");
            wtc.WriteWhiteLine(" - Displays this screen");
            wtc.WriteBlue("Clear");
            wtc.WriteWhiteLine(" - Clears the screen");
            wtc.WriteBlue("Application");
            wtc.WriteWhiteLine(" - Opens up the Application menu to manage your Applications");
            wtc.WriteBlue("Settings");
            wtc.WriteWhiteLine(" - Opens up the Settings Menu to modify Installer Settings");
            wtc.WriteBlue("Quit | Exit");
            wtc.WriteWhiteLine(" - Closes Installer");
        }

        static public void Quit()
        {
            Environment.Exit(0);
        }

        #region Application

        static public void Applications()
        {
            wtc.WriteMagenta("applications");
            wtc.WriteBlue(">");
            wtc.WriteWhite(" ");
            string[] input = Console.ReadLine().ToLower().Trim().Split(' ');
            while (input[0] != "back" && input[0] != "return")
            {
                switch (input[0])
                {
                    case "?":
                    case "help":
                        if (input.Length > 1)
                            ApplicationHelp(input);
                        else
                            ApplicationHelp();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "list":
                        SettingsList(input);
                        break;
                    case "settings":
                        ApplicationSendToSettings(input);
                        break;
                    case "create":
                        ApplicationAdd(input);
                        break;
                    //case "copy":
                    //    ApplicationCopy(input);
                    //    break;
                    case "delete":
                        ApplicationDelete(input);
                        break;
                    case "return":
                    case "back":          
                        break;
                    default:
                        wtc.WriteRedLine("[ERROR] Unrecognized command");
                        break;
                }

                wtc.WriteMagenta("applications");
                wtc.WriteBlue(">");
                wtc.WriteWhite(" ");
                input = Console.ReadLine().ToLower().Trim().Split(' ');

            }
        }

        private static void ApplicationSendToSettings(string[] input)
        {
            bool found = false;
            string appName = "";
            for (int i = 1; i < input.Length; i++)
            {
                appName += input[i] + " ";
            }
            //Remove last space we added with reconstruction method
            appName = appName.Remove(appName.Length - 1, 1);            
            foreach (Application.Application app in ApplicationCollection.GetApplications())
            {
                if (app.Name.ToLower().Trim() == appName)
                {
                    Settings(app);
                    found = true;                                     
                }
            }
            if(!found)
            {
                wtc.WriteRed("Unable to find ");
                wtc.WriteWhite(appName);
                wtc.WriteRedLine(" in Application Store!");
            }
        }

        private static void ApplicationDelete(string[] input)
        {
            bool found = false;
            string appName = "";
            for (int i = 1; i < input.Length; i++)
            {
                appName += input[i] + " ";
            }
            //Remove last space we added with reconstruction method
            appName = appName.Remove(appName.Length - 1, 1);
            if (appName == "main")
            {
                wtc.WriteRedLine("Unable to delete main application. . .");
            }
            else
            {
                List<Application.Application> apps = (List<Application.Application>)ApplicationCollection.GetApplications();
                for (int i = 0; i < apps.Count; i++)
                {
                    if (appName == apps[i].Name.ToLower().Trim())
                    {
                        //Found the file
                        if (File.Exists(apps[i].Directory + "\\running.lock"))
                        {
                            wtc.WriteRed("Unable to delete: ");
                            wtc.WriteWhiteLine(appName);
                            wtc.WriteRedLine("A job is currently using the program");
                            found = true;
                        }
                        else
                        {
                            try
                            {
                                Directory.Delete(apps[i].Directory, true);
                                wtc.WriteRedLine(apps[i].Name + " Deleted");
                                ApplicationCollection.RemoveApplication(apps[i]);
                                found = true;
                            }
                            catch (Exception ex)
                            {
                                wtc.WriteRedLine(ex.Message);
                            }
                        }

                    }
                }
                if (!found)
                {
                    wtc.WriteRed("Unable to find ");
                    wtc.WriteWhite(appName);
                    wtc.WriteRedLine(" in Application Store!");
                }
            }
        }

        //private static void ApplicationCopy(string[] input)
        //{
        //    if(input.Length < 3)
        //    {
        //        wtc.WriteRed("[ERROR] Bad Syntax for Copy Command; Type ");
        //        wtc.WriteWhite("Help Copy ");
        //        wtc.WriteRedLine("For examples");
        //    }
        //    else
        //    {
        //        string appName = "";
        //        for (int i = 1; i < input.Length; i++)
        //        {
        //            appName += input[i] + " ";
        //        }
        //        //Remove last space we added with reconstruction method
        //        appName = appName.Remove(appName.Length - 1, 1);
        //        ApplicationAdd(appName);
        //    }
        //}

        private static void ApplicationAdd(string[] input)
        {
            if(input.Length < 2)
            {
                wtc.WriteRed("[ERROR] Bad Syntax for Create Command; Type ");
                wtc.WriteWhite("Help Copy ");
                wtc.WriteRedLine("For examples");
            }
            else
            {
                string appName = "";
                for (int i = 1; i < input.Length; i++)
                {
                    appName += input[i] + " ";
                }
                //Remove last space we added with reconstruction method
                appName = appName.Remove(appName.Length - 1, 1);
                //Get the Main application settings;
                Application.Application gen = ApplicationCollection.GetSpecificApplication("main");
                INI.IniFile ini = new INI.IniFile(gen.IniPath);
                string createDir = ini.Section("General").Get("AppDir");
                string sourceDir = ini.Section("General").Get("SourceDir");
                string appDir = createDir + "\\" + appName;
                try
                {
                    Directory.CreateDirectory(appDir);
                    INI.IniFile appINI = new INI.IniFile();
                    appINI.Section("AppInfo").Set("Name", appName);
                    appINI.Section("AppInfo").Set("SourceFiles", sourceDir + "\\" + appName);
                    appINI.Save(appDir + "\\settings.ini");
                    //BIG TODO HERE

                    //Create the App and add it to our collection;
                    Application.Application app = new Application.Application(appName, appDir + "\\settings.ini");
                    ApplicationCollection.AddApplication(app);

                    Settings(app);

                }
                catch(Exception ex)
                {
                    wtc.WriteRed("[ERROR]: " + ex.Message);
                }

            }
        }

        private static void ApplicationAdd(string appName)
        {
            //Get the Main application settings;
            Application.Application gen = ApplicationCollection.GetSpecificApplication("main");
            INI.IniFile ini = new INI.IniFile(gen.IniPath);
            string createDir = ini.Section("General").Get("AppDir");
            string sourceDir = ini.Section("General").Get("SourceDir");
            string appDir = createDir + "\\" + appName;
            try
            {
                if(Directory.Exists(appDir))
                {
                    wtc.WriteRed("[ERROR]: " + appDir + " already exists!");
                }
                else
                {
                    Directory.CreateDirectory(appDir);
                    INI.IniFile appINI = new INI.IniFile();
                    appINI.Section("AppInfo").Set("Name", appName);
                    appINI.Section("AppInfo").Set("SourceFiles", sourceDir + "\\" + appName);
                    appINI.Save(appDir + "\\settings.ini");

                    //Create the App and add it to our collection;
                    Application.Application app = new Application.Application(appName, appDir + "\\settings.ini");
                    ApplicationCollection.AddApplication(app);

                    Settings(app);
                }
                
            }
            catch (Exception ex)
            { 
                wtc.WriteRed("[ERROR]: " + ex.Message);
            }

        }    

        private static void ApplicationHelp()
        {
            Console.BackgroundColor = ConsoleColor.White;
            wtc.WriteBlackLine("[ Application > Help ]");
            Console.BackgroundColor = ConsoleColor.Black;
            wtc.WriteBlueLine("(Help | ?) [Command]");
            wtc.WriteBlueLine("Clear");
            wtc.WriteBlueLine("List {String}");
            wtc.WriteBlueLine("Settings [Application]");
            //wtc.WriteBlueLine("Copy [SourceApp] [NewApp]");
            wtc.WriteBlueLine("Create [Application]");
            wtc.WriteBlueLine("Delete [Application]");            
            wtc.WriteBlueLine("(Back | Return | Main)");
            wtc.WriteBlueLine("(Quit | Exit)");
        }

        private static void ApplicationHelp(string[] input)
        {
            Console.BackgroundColor = ConsoleColor.White;
            wtc.WriteBlackLine("[ Application > Help > " + input[1] + " ]");
            Console.BackgroundColor = ConsoleColor.Black;

            switch (input[1])
            {
                case "help":                    
                    wtc.WriteBlueLine("(Help | ?) [Command]");
                    wtc.WriteWhiteLine("Description: Displays this screen");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Help\t\t");
                    wtc.Example("Displays the Simple Help Menu");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Help Help\t");
                    wtc.Example("Displays a detailed help menu about \"Help\"");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("? Delete\t");
                    wtc.Example("Displays a detailed help menu about \"Delete\"");
                    break;
                case "clear":
                    wtc.WriteBlueLine("Clear");
                    wtc.WriteWhiteLine("Description: Clears the screen");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Logging is enabled by default, if you need to review an entry please visit the \"main.log\" in your LogDir");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Clear\t");
                    wtc.Example("Clears the Screen");
                    break;
                case "list":
                    wtc.WriteBlueLine("List {String}");
                    wtc.WriteWhiteLine("Description: Lists all applications loaded into Installer");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("{String} Will limit the results to applications that start with the supplied argument");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("List\t");
                    wtc.Example("Displays all Applications loaded into Installer");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("List Note\t");
                    wtc.Example("Displays all Applications that *START WITH* Note");
                    break;
                case "settings":
                    wtc.WriteBlueLine("Settings [Application]");
                    wtc.WriteWhiteLine("Description: Brings you to the setting menu for the Application");                    
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Settings main\t");
                    wtc.Example("Switches to the settings for the Installer Application");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Switch Notepad\t");
                    wtc.Example("Switches to the settings for the Notepad Application");
                    break;
                //case "copy":
                //    wtc.WriteBlueLine("Copy [SourceApp] [NewApp]");
                //    wtc.WriteWhiteLine("Description: Copies the settings from one app to another changing needed values");
                //    wtc.WriteGreen("NOTE: ");
                //    wtc.WriteWhiteLine("Most options will copy over but not all. Please go to the settings of the application after copy to verify its setup correctly");
                //    wtc.WriteRedLine("Examples:");
                //    wtc.WriteBlue(">");
                //    wtc.WriteWhite("Copy Notepad Notepadv2\t");
                //    wtc.Example("Copies relevant settings from Notepad and creates the Notepad2 application");
                //    break;
                case "create":
                    wtc.WriteBlueLine("Create [Application]");
                    wtc.WriteWhiteLine("Description: Creates an applicaiton and adds it to your application store");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Default settings are copied into the Application, please modify the ini after creation via the settings menu");
                    wtc.WriteRedLine("Example:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Create Notepad\t");
                    wtc.Example("Creates the Notepad Application");
                    break;
                case "delete":
                    wtc.WriteBlueLine("Delete [Application]");
                    wtc.WriteWhiteLine("Description: Deletes an applicaiton from your application store");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("An application cannot be deleted if a job is currently running on the application");
                    wtc.WriteRedLine("Example:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Delete Notepad\t");
                    wtc.Example("Deletes the Notepad Application");                    
                    break;
                case "back":
                    wtc.WriteBlueLine("Back | Return | Main");
                    wtc.WriteWhiteLine("Description: Returns to the Main Menu");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Back\t");
                    wtc.Example("Returns you to the Main Menu");
                    break;
                default:
                    wtc.WriteRedLine("[ERROR] " + input[1] + " not recognized");
                    break;
            }
        }  

        #endregion

        #region Settings        

        private static void SettingsHelp()
        {
            Console.BackgroundColor = ConsoleColor.White;
            wtc.WriteBlackLine("[ Settings > Help ]");
            Console.BackgroundColor = ConsoleColor.Black;
            wtc.WriteBlueLine("(Help | ?) [Command]");
            wtc.WriteBlueLine("Clear");
            wtc.WriteBlueLine("List {String}");
            wtc.WriteBlueLine("Switch [Application]");            
            wtc.WriteBlueLine("Display");
            wtc.WriteBlueLine("Edit");
            wtc.WriteBlueLine("Modify [Section] [Property] [Value]");
            wtc.WriteBlueLine("(Back | Return | Main)");
            wtc.WriteBlueLine("(Quit | Exit)");
        }

        private static void SettingsHelp(string[] input)
        {
            Console.BackgroundColor = ConsoleColor.White;
            wtc.WriteBlackLine("[ Settings > Help > " + input[1] + " ]");
            Console.BackgroundColor = ConsoleColor.Black;

            switch(input[1])
            {
                case "help":
                    wtc.WriteBlueLine("(Help | ?) [Command]");
                    wtc.WriteWhiteLine("Description: Displays this screen");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Help\t\t");
                    wtc.Example("Displays the Simple Help Menu");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Help Help\t");
                    wtc.Example("Displays a detailed help menu about \"Help\"");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("? Display\t");
                    wtc.Example("Displays a detailed help menu about \"Display\"");
                    break;
                case "clear":
                    wtc.WriteBlueLine("Clear");
                    wtc.WriteWhiteLine("Description: Clears the screen");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Logging is enabled by default, if you need to review an entry please visit the \"main.log\" in your LogDir");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Clear\t");
                    wtc.Example("Clears the Screen");
                    break;
                case "list":
                    wtc.WriteBlueLine("List {String}");
                    wtc.WriteWhiteLine("Description: Lists all applications loaded into Installer");                    
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("{String} Will limit the results to applications that start with the supplied argument");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("List\t");
                    wtc.Example("Displays all Applications loaded into Installer");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("List Note\t");
                    wtc.Example("Displays all Applications that *START WITH* Note");
                    break;
                case "switch":
                    wtc.WriteBlueLine("Switch [Application]");
                    wtc.WriteWhiteLine("Description: Switches the application you are modifying");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Pay attention to the application you have selected in settings. Use switch to change the settings of a different Application");
                    wtc.WriteWhiteLine("You can always return back to the Installer instance by using Switch Main");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Switch Notepad++\t");
                    wtc.Example("Switches to the application Notepad++");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Switch Main\t");
                    wtc.Example("Switches to the Installer Instance");
                    break;
                case "display":
                    wtc.WriteBlueLine("Display");
                    wtc.WriteWhiteLine("Description: Displays the INI file for the Application");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Pay attention to the application you have selected in settings. By default you are looking at the \"main\" application which represents Installer");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Display\t");
                    wtc.Example("Outputs the INI file to the Screen");
                    break;
                case "edit":
                    wtc.WriteBlueLine("Edit");
                    wtc.WriteWhiteLine("Description: Opens the ini file in your default ini editorett for manual editing");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Edit\t");
                    wtc.Example("Opens the current application's ini file in notepad");
                    break;
                case "modify":
                    wtc.WriteBlueLine("Modify [Section] [Property] [Value]");
                    wtc.WriteWhiteLine("Description: Modifies the Applications INI file with a new value");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Modify General LogDir C:\\Example\\Example\t");
                    wtc.Example("Changes the LogDir entry to \"C:\\Example\\Example\"");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Modify General LogDir C:\\Example\\Example with space\t");
                    wtc.Example("Changes the LogDir entry to \"C:\\Example\\Example with space\"");
                    break;
                case "back":
                    wtc.WriteBlueLine("Back | Return | Main");
                    wtc.WriteWhiteLine("Description: Returns to the Main Menu");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue("> ");
                    wtc.WriteWhite("Back\t");
                    wtc.Example("Returns you to the Main Menu");
                    break;
                default:
                    wtc.WriteRedLine("[ERROR] " + input[1] + " not recognized"); 
                    break;
            }            
        }

        static public void Settings(Application.Application application)
        {
            Application.Application app = application;
            main = application;
            wtc.WriteYellow("settings");
            wtc.WriteWhite("[");
            wtc.WriteGreen(app.Name);
            wtc.WriteWhite("]");
            wtc.WriteBlue(">");
            wtc.WriteWhite(" ");
            string[] input = Console.ReadLine().ToLower().Trim().Split(' ');
            while (input[0] != "back" && input[0] != "return")
            {
                switch (input[0])
                {
                    case "help":
                        if (input.Length > 1)
                            SettingsHelp(input);
                        else
                            SettingsHelp();
                        break;
                    case "?":
                        if (input.Length > 1)
                            SettingsHelp(input);
                        else
                            SettingsHelp();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "list":
                        SettingsList(input);
                        break;
                    case "switch":
                        app = SettingsSwitch(app, input);
                        break;
                    case "display":
                        SettingsDisplay(app);
                        break;
                    case "edit":
                        SettingsEdit(app);
                        break;
                    case "modify":
                        SettingsModify(app, input);
                        break;
                    case "back":
                        break;
                    case "return":
                        break;
                    default:
                        wtc.WriteRedLine("[ERROR] Unrecognized command");
                        break;
                }

                wtc.WriteYellow("settings");
                wtc.WriteWhite("[");
                wtc.WriteGreen(app.Name);
                wtc.WriteWhite("]");
                wtc.WriteBlue(">");
                wtc.WriteWhite(" ");
                input = Console.ReadLine().ToLower().Trim().Split(' ');
            }
        }

        private static void SettingsEdit(Application.Application app)
        {
            Process.Start(app.IniPath);
        }

        private static void SettingsList(string[] input)
        {
            if (input.Length > 1)
            {
                string appName = "";
                for (int i = 1; i < input.Length; i++)
                {
                    appName += input[i] + " ";
                }
                //Remove last space we added with reconstruction method
                appName = appName.Remove(appName.Length - 1, 1);

                foreach (Application.Application app in ApplicationCollection.GetApplications())
                {
                    if (app.Name.ToLower().Trim().StartsWith(appName))
                    {
                        wtc.WriteGreen(app.Name);
                        wtc.WriteWhite(" - ");
                        wtc.WriteBlueLine(app.IniPath);
                    }
                }
            }
            else
            {
                foreach (Application.Application app in ApplicationCollection.GetApplications())
                {
                    wtc.WriteGreen(app.Name);
                    wtc.WriteWhite(" - ");
                    wtc.WriteBlueLine(app.IniPath);
                }
            }
        }

        private static Application.Application SettingsSwitch(Application.Application app, string[] input)
        {
            //reconstruct the input array
            string appName = "";
            for (int i = 1; i < input.Length; i++)
            {
                appName += input[i] + " ";
            }
            //Remove last space we added with reconstruction method
            appName = appName.Remove(appName.Length - 1, 1);            
            try
            {
                foreach (Application.Application p in ApplicationCollection.GetApplications())
                {

                    if (p.Name.ToLower().Trim() == appName)
                    {
                        return p;
                    }
                }

                return app;
            }
            catch (Exception ex)
            {
                wtc.WriteRed("[ERROR] " + ex.Message);
                return app;
            }
        }

        private static void SettingsDisplay(Application.Application application)
        {
            application.ReadINI();
        }

        private static void SettingsModify(Application.Application application, string[] input)
        {
            if(input.Length < 4)
            {
                wtc.WriteRed("[ERROR] Bad Syntax for Modify Command; Type ");
                wtc.WriteWhite("Help Modify ");
                wtc.WriteRedLine("For examples");
            }
            else
            {
                application.ChangeINI(input);
            }
        }

        #endregion
    }
}
