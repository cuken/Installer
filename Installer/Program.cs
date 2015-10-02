using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using Installer.Utilities;

namespace Installer
{
    class Program
    {
        #region Variables & Objects
        private static Logger logger;        
        WTC wtc = new WTC();        
        INI.IniFile ini;

        string iniPath = Environment.CurrentDirectory + "\\Config.ini";
        string logLevel = "";
        string consoleLogLevel = "";
        string logDir = "";
        string appDir = "";
        string sourceDir = "";
        string toolDir = "";
        bool bLogToConsole = false;
        bool bClearBeforeMenu = true;
        bool bPsexecMissing = false;


        #endregion


        //Abandoning the Static Requirement
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }

        void Run()
        {
            //Main entry pint
            Console.WriteLine("[STARTUP] Installer is starting up...");

            //Call Startup to initialze our application;
            Startup();
            Application.Application main = new Application.Application("main", iniPath);
            Application.ApplicationCollection.AddApplication(main);
            LoadApplications();
            wtc.WriteGreenLine("Startup Complete, launching into main menu");

            if (bClearBeforeMenu)
                Console.Clear();

            bool running = true;

            wtc.WriteWhiteLine("======================");
            wtc.WriteWhiteLine("= Installer v0.0.0.1 =");
            wtc.WriteWhiteLine("======================");

            do
            {
                wtc.WriteBlue(">");
                wtc.WriteWhite(" ");
                string input = Console.ReadLine().ToLower().Trim();

                //receives user input, switch off of it;
                switch (input)
                {
                    case "help":
                        Menu.Help();
                        break;
                    case "?":
                        Menu.Help();
                        break;
                    case "clear":
                        Console.Clear();
                        break;
                    case "application":
                    case "applications":
                        Menu.Applications();
                        break;
                    case "setting":
                    case "settings":
                        Menu.Settings(main);
                        break;
                    case "quit":
                    case "q":
                    case "exit":
                    case "x":
                        logger.Debug("Exiting due to user input");
                        running = false;
                        break;
                    default:
                        Menu.Help();
                        break;
                }
            }
            while (running);
            {
            }                
        }

        void Startup()
        {
            #region INISetup
            try
            {
                if (!File.Exists(iniPath))
                {
                    //INI File is not present in application directory
                    //Create one by hand and set default settings;
                    Console.WriteLine("[STARTUP] INI File not found, Generating one with default values");
                    ini = new INI.IniFile();
                    ini.Section("General").Set("LogToFile", "True");
                    ini.Section("General").Set("LogLevel", "Debug");
                    ini.Section("General").Set("LogToConsole", "False");
                    ini.Section("General").Set("ConsoleLogLevel", "Warn");
                    ini.Section("General").Set("LogDir", Environment.CurrentDirectory + "\\Logs");
                    ini.Section("General").Set("AppDir", Environment.CurrentDirectory + "\\Applications");
                    ini.Section("General").Set("SourceDir", Environment.CurrentDirectory + "\\Source");
                    ini.Section("General").Set("ToolDir", Environment.CurrentDirectory + "\\Tools");
                    ini.Section("Optional").Set("ClearBeforeMain", "True", "This will clear the console before displaying main menu");
                    ini.Save(iniPath);
                }


                //Reading INI File and setting variables;
                ini = new INI.IniFile(iniPath);

                Console.WriteLine("[STARTUP] Reading INI values...");
                logLevel = ini.Section("General").Get("LogLevel");
                wtc.WriteWhite("[STARTUP] LogLevel: ");
                wtc.WriteGreen(logLevel + "\n");
                bLogToConsole = Boolean.Parse(ini.Section("General").Get("LogToConsole"));
                wtc.WriteWhite("[STARTUP] LogToConsole: ");
                wtc.WriteGreen(bLogToConsole.ToString() + "\n");
                consoleLogLevel = ini.Section("General").Get("ConsoleLogLevel");
                wtc.WriteWhite("[STARTUP] ConsoleLogLevel: ");
                wtc.WriteGreen(consoleLogLevel + "\n");
                logDir = ini.Section("General").Get("LogDir");
                wtc.WriteWhite("[STARTUP] Log Directory: ");
                wtc.WriteGreen(logDir + "\n");
                appDir = ini.Section("General").Get("AppDir");
                wtc.WriteWhite("[STARTUP] Application Directory: ");
                wtc.WriteGreen(appDir + "\n");
                sourceDir = ini.Section("General").Get("SourceDir");
                wtc.WriteWhite("[STARTUP] Source Directory: ");
                wtc.WriteGreen(sourceDir + "\n");
                toolDir = ini.Section("General").Get("ToolDir");
                wtc.WriteWhite("[STARTUP] Tool Directory: ");
                wtc.WriteGreen(toolDir + "\n");

                //Optional INI settings
                //WE don't care if their missing, just to tailor the experience to each user;
                wtc.WriteWhiteLine("[STARTUP] Checking for optional INI settings");
                try
                {
                    wtc.WriteWhite("[OPTIONAL] Checking for ClearBeforeMain");
                    bClearBeforeMenu = Boolean.Parse(ini.Section("Optional").Get("ClearBeforeMain"));
                }
                catch
                {

                }

                //Checking for Logging directory first so we can log missing folder structure later.
                if (!Directory.Exists(logDir))
                {
                    wtc.WriteWhiteLine("[STARTUP] No logDirectory found, attempting to create");
                    //Try to create the logDir, Fail out and throw error if not
                    try
                    {
                        Directory.CreateDirectory(logDir);
                        wtc.WriteWhite("[STARTUP] Creating LogDir at ");
                        wtc.WriteGreenLine(logDir);
                    }
                    catch (Exception ex)
                    {
                        //Unable to create the directory, throw fatal error and exit
                        wtc.WriteRedLine("Fatal Error: " + ex.Message);
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                }
                #endregion

            #region Logging

                var config = new LoggingConfiguration();
                var fileTarget = new FileTarget();

                config.AddTarget("file", fileTarget);

                fileTarget.Layout = "[${longdate}] - [${level}]: ${message}";
                fileTarget.FileName = logDir + "\\Main.log";

                LoggingRule rule_LTF;

                switch (logLevel.ToUpper())
                {
                    case "TRACE":
                        rule_LTF = new LoggingRule("*", LogLevel.Trace, fileTarget);
                        wtc.WriteWhite("[STARTUP] LogToFileLevel set to: ");
                        wtc.WriteGreen(logLevel + "\n");
                        break;
                    case "DEBUG":
                        rule_LTF = new LoggingRule("*", LogLevel.Debug, fileTarget);
                        wtc.WriteWhite("[STARTUP] LogToFileLevel set to: ");
                        wtc.WriteGreen(logLevel + "\n");
                        break;
                    case "WARN":
                        rule_LTF = new LoggingRule("*", LogLevel.Warn, fileTarget);
                        wtc.WriteWhite("[STARTUP] LogToFileLevel set to: ");
                        wtc.WriteGreen(logLevel + "\n");
                        break;
                    case "INFO":
                        rule_LTF = new LoggingRule("*", LogLevel.Info, fileTarget);
                        wtc.WriteWhite("[STARTUP] LogToFileLevel set to: ");
                        wtc.WriteGreen(logLevel + "\n");
                        break;
                    case "ERROR":
                        rule_LTF = new LoggingRule("*", LogLevel.Error, fileTarget);
                        wtc.WriteWhite("[STARTUP] LogToFileLevel set to: ");
                        wtc.WriteGreen(logLevel + "\n");
                        break;
                    default:
                        wtc.WriteRedLine("[STARTUP] Uknown type " + logLevel + " defaulting to WARN");
                        rule_LTF = new LoggingRule("*", LogLevel.Warn, fileTarget);
                        break;
                }

                config.LoggingRules.Add(rule_LTF);

                if (bLogToConsole)
                {
                    var consoleTarget = new ColoredConsoleTarget();
                    config.AddTarget("console", consoleTarget);

                    consoleTarget.Layout = "[${longdate}] - [${level}]: ${message}";
                    LoggingRule rule_LTC;

                    switch (consoleLogLevel.ToUpper())
                    {
                        case "TRACE":
                            rule_LTC = new LoggingRule("*", LogLevel.Trace, consoleTarget);
                            wtc.WriteWhite("[STARTUP] ConsoleLogLevel set to: ");
                            wtc.WriteGreen(consoleLogLevel + "\n");
                            break;
                        case "DEBUG":
                            rule_LTC = new LoggingRule("*", LogLevel.Debug, consoleTarget);
                            wtc.WriteWhite("[STARTUP] ConsoleLogLevel set to: ");
                            wtc.WriteGreen(consoleLogLevel + "\n");
                            break;
                        case "WARN":
                            rule_LTC = new LoggingRule("*", LogLevel.Warn, consoleTarget);
                            wtc.WriteWhite("[STARTUP] ConsoleLogLevel set to: ");
                            wtc.WriteGreen(consoleLogLevel + "\n");
                            break;
                        case "INFO":
                            rule_LTC = new LoggingRule("*", LogLevel.Info, consoleTarget);
                            wtc.WriteWhite("[STARTUP] ConsoleLogLevel set to: ");
                            wtc.WriteGreen(consoleLogLevel + "\n");
                            break;
                        case "ERROR":
                            rule_LTC = new LoggingRule("*", LogLevel.Error, consoleTarget);
                            wtc.WriteWhite("[STARTUP] ConsoleLogLevel set to: ");
                            wtc.WriteGreen(consoleLogLevel + "\n");
                            break;
                        default:
                            wtc.WriteRedLine("[STARTUP] Uknown type " + consoleLogLevel + " defaulting to WARN");
                            rule_LTC = new LoggingRule("*", LogLevel.Warn, fileTarget);
                            break;
                    }

                    config.LoggingRules.Add(rule_LTC);
                }
                else
                {
                    wtc.WriteWhite("[STARTUP] LogToConsole set to: ");
                    wtc.WriteRed(bLogToConsole.ToString());
                    wtc.WriteWhiteLine(" - Skipping level check");
                }

                LogManager.Configuration = config;
                logger = LogManager.GetCurrentClassLogger();

                logger.Debug("============================");
                logger.Debug("Application Started");
                logger.Debug("============================");


                logger.Debug("Exporting settings to log");
                logger.Debug("LogLevel: " + logLevel);
                logger.Debug("LogToConsole " + bLogToConsole.ToString());
                logger.Debug("ConsoleLogLevel: " + consoleLogLevel);
                logger.Debug("LogDir: " + logDir);
                logger.Debug("AppDir: " + appDir);
                logger.Debug("SourceDir: " + sourceDir);
                logger.Debug("ToolDir: " + toolDir);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Critical  Error: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Application will now close");
                Console.ReadKey();
                Environment.Exit(1605);
            }

            #endregion

            #region DirectoryCheck

            //Broke up the Try Catch so we can get the error and parse it to the log versus the console before logging exists.

            try
            {

                //Check through each directory to make sure they exist
                logger.Debug("Checking through each directory to make sure they exist");

                wtc.WriteWhiteLine("[STARTUP] Checking for AppDirectory");
                logger.Debug("Checking for AppDirectory at " + appDir);

                if (!Directory.Exists(appDir))
                {
                    logger.Debug("No Directory found at " + appDir);

                    //Try to create the appDir, Fail out and throw error if not
                    try
                    {
                        logger.Debug("Attempting to create directory at " + appDir);
                        Directory.CreateDirectory(appDir);
                        wtc.WriteWhite("[STARTUP] Creating AppDir at ");
                        wtc.WriteGreenLine(appDir);
                    }
                    catch (Exception ex)
                    {
                        //Unable to create the directory, throw fatal error and exit
                        logger.Error("Unable to create directory at " + appDir);
                        logger.Error("Failed with error: " + ex.Message);
                        wtc.WriteRedLine("Fatal Error: " + ex.Message);
                        Console.ReadKey();
                        Environment.Exit(6);
                    }
                }
                else
                {
                    logger.Debug("AppDirectory exists at " + appDir);
                    wtc.WriteGreenLine("[STARTUP] Application Directory exists!");
                }

                wtc.WriteWhiteLine("[STARTUP] Checking for SourceDirectory");
                logger.Debug("Checking for SourceDirectory at " + sourceDir);


                if (!Directory.Exists(sourceDir))
                {
                    logger.Debug("No Directory found at " + sourceDir);

                    //Try to create the appDir, Fail out and throw error if not
                    try
                    {
                        logger.Debug("Attempting to create directory at " + sourceDir);
                        Directory.CreateDirectory(sourceDir);
                        wtc.WriteWhite("[STARTUP] Creating SourceDir at ");
                        wtc.WriteGreenLine(sourceDir);

                    }
                    catch (Exception ex)
                    {
                        //Unable to create the directory, throw fatal error and exit
                        logger.Error("Unable to create directory at " + sourceDir);
                        logger.Error("Failed with error: " + ex.Message);
                        wtc.WriteRedLine("Fatal Error: " + ex.Message);
                        Console.ReadKey();
                        Environment.Exit(7);
                    }
                }
                else
                {
                    logger.Debug("SourceDirectory exists at " + sourceDir);
                    wtc.WriteGreenLine("[STARTUP] Source Directory exists!");
                }

                wtc.WriteWhiteLine("[STARTUP] Checking for ToolDirectory");
                logger.Debug("Checking for ToolDirectory at " + sourceDir);


                if (!Directory.Exists(toolDir))
                {
                    logger.Debug("No Directory found at " + toolDir);

                    //Try to create the appDir, Fail out and throw error if not
                    try
                    {
                        logger.Debug("Attempting to create directory at " + sourceDir);
                        Directory.CreateDirectory(toolDir);
                        wtc.WriteWhite("[STARTUP] Creating ToolDir at ");
                        wtc.WriteGreenLine(toolDir);

                    }
                    catch (Exception ex)
                    {
                        //Unable to create the directory, throw fatal error and exit
                        logger.Error("Unable to create directory at " + sourceDir);
                        logger.Error("Failed with error: " + ex.Message);
                        wtc.WriteRedLine("Fatal Error: " + ex.Message);
                        Console.ReadKey();
                        Environment.Exit(7);
                    }
                }
                else
                {
                    logger.Debug("ToolDirectory exists at " + toolDir);
                    wtc.WriteGreenLine("[STARTUP] Tool Directory exists!");
                }

                //Check for Write/Read/Delete Permissions in directories;

                logger.Debug("Checking for Write/Read/Delete Permissions in directories");
                try
                {
                    //APPDIR
                    logger.Debug("Creating file TEST in " + appDir);
                    File.WriteAllText(appDir + "\\test.test", "");
                    logger.Debug(appDir + "\\test.test" + " - File Created!");
                    logger.Debug("Deleting File " + appDir + "\\test.test");
                    File.Delete(appDir + "\\test.test");
                    logger.Debug(appDir + "\\test.test" + " - File Deleted!");
                    //SOURCEDIR
                    logger.Debug("Creating file TEST in " + sourceDir);
                    File.WriteAllText(sourceDir + "\\test.test", "");
                    logger.Debug(sourceDir + "\\test.test" + " - File Created!");
                    logger.Debug("Deleting File " + sourceDir + "\\test.test");
                    File.Delete(sourceDir + "\\test.test");
                    logger.Debug(sourceDir + "\\test.test" + " - File Deleted!");
                    //TOOLDIR
                    logger.Debug("Creating file TEST in " + toolDir);
                    File.WriteAllText(toolDir + "\\test.test", "");
                    logger.Debug(toolDir + "\\test.test" + " - File Created!");
                    logger.Debug("Deleting File " + toolDir + "\\test.test");
                    File.Delete(toolDir + "\\test.test");
                    logger.Debug(toolDir + "\\test.test" + " - File Deleted!");
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex.Message);
                    wtc.WriteRedLine("[FATAL ERROR] " + ex.Message);
                    Console.ReadKey();
                    Environment.Exit(8);
                }
                #endregion

                #region ToolSupport
                //This seciton needs to get improved as we add more tool support;
                //Currently going to check for PSEXEC, will need to validate more as we use more.

                //IDEA MIRROR SUPPORT;
                //DOWNLOAD TOOLS NEEDED VIA A MIRROR AND VERIFY MD5

                //DEFINE TOOLS IN INI?
                if (!File.Exists(toolDir + "\\psexec.exe"))
                {
                    //PSEXEC is missing;
                    logger.Warn("Unable to find psexec in the following location [" + toolDir + "\\psexec.exe]");
                    logger.Warn("Any applications that use PSEXEC will not function!");
                    bPsexecMissing = true;
                    wtc.WriteYellowLine("[STARTUP] PSEXEC is missing from the Tools directory. Please make sure the exe is in the given path, or change the \"ToolDir\" path in your ini to where PSEXEC exists");
                    wtc.WriteYellowLine("[WARNING] Program will continue, any application that uses PSEXEC as the install driver will not function till this is resolved");
                }
            }
            catch (Exception ex)
            {
                wtc.WriteRedLine("[FATAL ERROR] " + ex.Message);
                logger.Fatal(ex.Message);
                Console.ReadKey();
                Environment.Exit(2);
            }
            #endregion
        }

        void LoadApplications()
        {
            wtc.WriteWhiteLine("[Apps] Scanning for Applications");
            logger.Debug("Scanning for Applications in " + appDir);

            ICollection<string> directories = Directory.GetDirectories(appDir);

            if(directories.Count < 1)
            {
                wtc.WriteYellowLine("[WARNING] 0 applications discovered in " + appDir);
                wtc.WriteRedLine("[WARNING] Installer needs applications in order to function");
                wtc.WriteWhiteLine("Please check INI file for correct Application Directory. To create a new Application go to the Application Menu from Main");
                logger.Warn("0 Applications found in " + appDir);
            }
            else
            {
                foreach(string d in directories)
                {
                    logger.Debug("Trying to load " + d + " into Applications");
                    if(!File.Exists(d + "\\settings.ini"))
                    {
                        logger.Warn("Unable to find settings.ini in folder " + d);
                        logger.Error("Application will not be loaded into active set");
                    }
                    else
                    {
                        INI.IniFile ini = new INI.IniFile(d + "\\settings.ini");
                        try
                        {
                            Application.Application app = new Application.Application(ini.Section("AppInfo").Get("Name"), d + "\\settings.ini");
                            Application.ApplicationCollection.AddApplication(app);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                            throw;
                        }
                    }
                    
                }
            }
            
        }

    }
}

