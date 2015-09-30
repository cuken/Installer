using System;
using System.IO;

namespace Installer
{
    public class WTC
    {
        public void WriteWhite(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);  
                 
        }

        public void WriteWhiteLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public void WriteGreen(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
        }

        public void WriteGreenLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public void WriteRed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
        }

        public void WriteRedLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public void WriteBlue(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
        }

        public void WriteBlueLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
        }
    }
}
