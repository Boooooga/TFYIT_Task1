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
            string path = @"C:\Users\123\Documents\Универ\Теория формальных языков и трансляций\NKA_2.txt";
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
                Automaton newAuto = myAuto.knaToKda();
                if (newAuto != null)
                {
                    newAuto.ShowInfo();
                    newAuto.ShowTable();
                }
            }

            if (myAuto.type == 3)
            {
                Automaton newAuto = myAuto.knaEpsToKna();
                newAuto.ShowInfo();
                newAuto.ShowTable();
            }
        }
    }
}