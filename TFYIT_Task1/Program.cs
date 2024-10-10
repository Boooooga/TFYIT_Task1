using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;

namespace TFYIT_Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\123\Documents\Универ\Теория формальных языков и трансляций\DKA.txt";
            Automaton myAuto = new Automaton(path);
            myAuto.ShowInfo();
            Console.WriteLine();
            myAuto.ShowTable();
            string inLine;
            Console.Write("\nВведите входное слово: ");
            inLine = Console.ReadLine();
            myAuto.ProcessInputLine(inLine);

            if (myAuto.type == 2) // для случая недетерминированного автомата
            {
                Automaton newAuto = myAuto.KNAtoKDA();
                newAuto.ShowInfo();
                newAuto.ShowTable();
            }
        }
    }
}