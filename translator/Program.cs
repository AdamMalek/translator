﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace translator
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Console.WriteLine("usage: tr.exe [source_file_path]");
            //    Console.ReadLine();
            //    Environment.Exit(0);
            //}
            //string path = args[0];

            string path = "source.src";

            string src = File.ReadAllText(path);
            src = src.Replace("\r", "");
            Analyzer analizer = new Analyzer();
            string[] symbols = new string[] { };
            Console.WriteLine("Result:");
            Console.WriteLine("-----------------------");
            try
            {
                symbols = analizer.AnalizeSource(src);
            }
            catch (InvalidSyntaxException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                GetFaultFragment(e.Position, src);
            }

            foreach (var symbol in symbols)
            {
                Console.WriteLine(symbol);
            }
            Console.ReadLine();
        }

        static void GetFaultFragment(int pos, string code)
        {
            int lastenter;
            try
            {
                lastenter = code.Select((x, i) => new { item = x, index = i }).Last(xx => xx.item == '\n' && xx.index < pos).index;
            }
            catch (Exception)
            {
                lastenter = -1;
            }
            int firstenter;
            try
            {
                firstenter = code.Select((x, i) => new { item = x, index = i }).First(xx => xx.item == '\n' && xx.index > pos).index;
            }
            catch (Exception)
            {
                firstenter = code.Length-1;
            }
            Console.Write(code.Substring(lastenter+1, pos - lastenter-1));
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            var character = code.ElementAt(pos);
            if (string.IsNullOrWhiteSpace(character.ToString()))
            {
                Console.Write("_");
            }
            else
            {
                Console.Write(character);
            }
            Console.ForegroundColor = consoleColor;
            if (pos < code.Length -1)
            Console.WriteLine(code.Substring(pos + 1, firstenter - pos-1));
        }
    }
}
