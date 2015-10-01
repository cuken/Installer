using System;
using Installer.Application;

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
            wtc.WriteBlue("Settings");
            wtc.WriteWhiteLine(" - Opens up the Settings Menu to modify Installer Settings");
            wtc.WriteBlue("Quit | Exit");
            wtc.WriteWhiteLine(" - Closes Installer");
        }

        static public void Quit()
        {
            Environment.Exit(0);
        }

        #region Settings

        static public void Settings(Application.Application application)
        {
            Application.Application app = application;
            main = application;
            wtc.WriteYellow("settings");
            wtc.WriteWhite("[");
            wtc.WriteGreen(app.name);
            wtc.WriteWhite("]");
            wtc.WriteBlue(">");
            wtc.WriteWhite(" ");
            string[] input = Console.ReadLine().ToLower().Trim().Split(' ');
            while(input[0] != "back" && input[0] != "return")
            {
                switch(input[0])
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
                wtc.WriteGreen(app.name);
                wtc.WriteWhite("]");
                wtc.WriteBlue(">");
                wtc.WriteWhite(" ");
                input = Console.ReadLine().ToLower().Trim().Split(' ');
            }
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
                    if(app.name.ToLower().Trim().StartsWith(appName))
                    {
                        wtc.WriteGreen(app.name);
                        wtc.WriteWhite(" - ");
                        wtc.WriteBlueLine(app.iniPath);
                    }
                }
            }
            else
            {
                foreach (Application.Application app in ApplicationCollection.GetApplications())
                {
                    wtc.WriteGreen(app.name);
                    wtc.WriteWhite(" - ");
                    wtc.WriteBlueLine(app.iniPath);
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
            appName = appName.Remove(appName.Length -1, 1);
            if(appName == "main")
            {
                return main;
            }
            try
            {
                foreach(Application.Application p in ApplicationCollection.GetApplications())
                {
                    
                    if(p.name.ToLower().Trim() == appName)
                    {
                        return p;
                    }
                }

                return app;
            }
            catch(Exception ex)
            {
                wtc.WriteRed("[ERROR] " + ex.Message);
                return app;
            }
        }

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
                    Console.Clear();
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
                    Console.Clear();
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
                    Console.Clear();
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
                    Console.Clear();
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
                    Console.Clear();
                    wtc.WriteBlueLine("Display");
                    wtc.WriteWhiteLine("Description: Displays the INI file for the Application");
                    wtc.WriteGreen("NOTE: ");
                    wtc.WriteWhiteLine("Pay attention to the application you have selected in settings. By default you are looking at the \"main\" application which represents Installer");
                    wtc.WriteRedLine("Examples:");
                    wtc.WriteBlue(">");
                    wtc.WriteWhite("Display\t");
                    wtc.Example("Outputs the INI file to the Screen");
                    break;
                case "modify":
                    Console.Clear();
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
                    Console.Clear();
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
