using System;
using System.IO;

namespace Installer.Utilities
{
    public class WTC
    {
        public void Example(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            
        }
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

        public void WriteBlack(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(message);
        }

        public void WriteBlackLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Black;
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

        public void WriteYellow(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
        }

        public void WriteYellowLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }

        public void WriteBlue(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
        }

        public void WriteBlueLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
        }
    }
}
