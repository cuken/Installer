using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;

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
        bool bLog = false;
        bool blogToConsole = false;


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


            Console.Read();
        }

        void Startup()
        {
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
                    ini.Section("General").Set("ConsoleLogLevel", "False");
                    ini.Section("General").Set("LogDir", Environment.CurrentDirectory + "\\Logs");
                    ini.Section("General").Set("AppDir", Environment.CurrentDirectory + "\\Applications");
                    ini.Section("General").Set("SourceDir", Environment.CurrentDirectory + "\\Source");
                    ini.Save(iniPath);                    
                }


                //Reading INI File and setting variables;
                ini = new INI.IniFile(iniPath);


                Console.WriteLine("[STARTUP] Reading INI values...");                
                bLog = Boolean.Parse(ini.Section("General").Get("LogToFile"));
                wtc.WriteWhite("[STARTUP] LogToFile: ");
                wtc.WriteGreen(bLog.ToString() + "\n");
                logLevel = ini.Section("General").Get("LogLevel");
                wtc.WriteWhite("[STARTUP] LogLevel: ");
                wtc.WriteGreen(logLevel + "\n");
                blogToConsole = Boolean.Parse(ini.Section("General").Get("LogToConsole"));
                wtc.WriteWhite("[STARTUP] LogToConsole: ");
                wtc.WriteGreen(bLog.ToString() + "\n");
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

                if(bLog)
                {
                    wtc.WriteWhiteLine("[STARTUP] Logging Enabled, setting log level");
                    var config = new LoggingConfiguration();
                    var fileTarget = new FileTarget();

                    config.AddTarget("file", fileTarget);

                    fileTarget.Layout = "[${longdate}] - [${level}]: ${message}";
                    fileTarget.FileName = logDir + "\\Main.log";

                    LoggingRule rule_LTF;

                    switch(logLevel.ToUpper())
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

                    if(blogToConsole)
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

                    LogManager.Configuration = config;                    
                    logger = LogManager.GetCurrentClassLogger();
                    logger.Debug("Exporting settings to log");
                    logger.Debug("LogLevel: " + logLevel);
                    logger.Debug("LogDir: " + logDir);
                    logger.Debug("AppDir: " + appDir);
                    logger.Debug("SourceDir: " + sourceDir);                    
                }

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
        }
    }
}
