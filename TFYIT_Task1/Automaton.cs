using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFYIT_Task1
{
    internal class Automaton
    {
        uint type;
        private string[] states;
        private string[] inputs;
        private string[] finalStates;
        private string initState;
        private Dictionary<string, List<string>> transitions;
        private Dictionary<string, int> inpIndexes;
        bool isInitiatedCorrectly;

        public Automaton(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                uint type = 0;
                string parameter = file.ReadLine();
                string line;

                string[] states = null;
                bool gotStates = false;
                string[] inputs = null;
                bool gotInputs = false;
                string initState = "";
                bool gotInitState = false;
                string[] finalStates = null;
                bool gotFinalStates = false;
                Dictionary<string, List<string>> transitions = new Dictionary<string, List<string>>();
                bool gotTransitions = false;

                bool emergencyStop = false;

                // определение типа автомата по первой строке файла
                if (parameter == "DKA") type = 1;
                else if (parameter == "NKA") type = 2;
                else if (parameter == "NKA-E") type = 3;
                else
                {
                    type = 0;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Введён некорректный тип автомата: '{parameter}'");
                    Console.ResetColor();
                }

                if (type != 0)
                {
                    line = file.ReadLine();
                    while (line != null)
                    {
                        if (line.StartsWith("Q:")) // ввод состояний
                        {
                            line = line.Replace("Q:", "");
                            line = line.Replace(" ", "");
                            states = line.Split(',');
                            gotStates = true;

                        }
                        else if (line.StartsWith("S:")) // ввод алфавита
                        {
                            line = line.Replace("S:", "");
                            line = line.Replace(" ", "");
                            inputs = line.Split(',');
                            gotInputs = true;
                        }
                        else if (line.StartsWith("Q0:"))
                        {
                            line = line.Replace("Q0:", "");
                            line = line.Replace(" ", "");
                            initState = line;
                            gotInitState = true;
                        }
                        else if (line.StartsWith("F:"))
                        {
                            line = line.Replace("F:", "");
                            line = line.Replace(" ", "");
                            finalStates = line.Split(",");
                            gotFinalStates = true;
                        }
                        else if (line.StartsWith("Table") && gotStates && gotInputs)
                        {
                            for (int i = 0; i < states.Length; i++)
                            {
                                line = file.ReadLine();
                                string[] tempStates = line.Split(' ');
                                if (type == 0)
                                {
                                    foreach (string state in tempStates)
                                    {
                                        if (!states.ToList().Contains(state))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine($"Ошибка! Состояние '{state}', указанное в таблице переходов, не определено!");
                                            Console.ResetColor();
                                            emergencyStop = true;
                                            break;
                                        }
                                    }
                                }
                                transitions.Add(states[i], tempStates.ToList());
                            }
                            gotTransitions = true;
                        }
                        line = file.ReadLine();
                    }
                    if (emergencyStop)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Автомат не может быть инициализирован из-за критической ошибки!");
                        Console.ResetColor();
                        isInitiatedCorrectly = false;
                    }
                    else if (gotStates && gotInputs && gotInitState && gotFinalStates && gotTransitions)
                    {
                        this.type = type;
                        this.states = states;
                        this.inputs = inputs;
                        this.finalStates = finalStates;
                        this.initState = initState;
                        this.transitions = transitions;

                        isInitiatedCorrectly = true;
                    }
                }
            }
            Console.WriteLine($"Автомат считан из файла {path}");
        }
        public Automaton(uint type, string[] states, string[] inputs, string[] finalStates, string initState, Dictionary<string, List<string>> transitions)
        {
            this.type = type;
            this.states = states;
            this.inputs = inputs;
            this.finalStates = finalStates;
            this.initState = initState;
            this.transitions = transitions;
        }
        public void ShowInfo()
        {
            if (isInitiatedCorrectly)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (type == 1) Console.WriteLine("Детерминированный КА");
                else if (type == 2) Console.WriteLine("Недетерминированный КА");
                else Console.WriteLine("Недетерминированный КА с е-переходами");
                Console.ResetColor();


                Console.Write("Состояния: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string word in states)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();


                Console.Write("Алфавит: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string word in inputs)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();

                Console.Write("Начальное состояние: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(initState);
                Console.ResetColor();


                Console.Write("Финальное(ые) состояние(я): ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string word in finalStates)
                {
                    Console.Write(word + " ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Операция 'Show' не может быть выполнена: автомат не проинициализирован.");
                Console.ResetColor();
            }
        }
        public void ShowTable()
        {
            if (isInitiatedCorrectly)
            {
                int maxLength = MaxLengthForTable();

                Console.WriteLine("Таблица переходов автомата:");

                for (int i = 0; i < states.Length + 1; i++)
                {
                    if (i == 0) // для первой строки
                    {
                        for (int j = 0; j < inputs.Length + 1; j++)
                        {
                            if (j == 0) Console.Write("\t");
                            else Console.Write($"|{{0, {-maxLength - 1}}}|", inputs[j - 1]);
                        }
                    }
                    else // для остальных строк
                    {
                        for (int j = 0; j < inputs.Length + 1; j++)
                        {
                            if (j == 0)
                            {
                                if (states[i - 1] == initState)
                                {
                                    Console.Write($"->{states[i - 1]}:\t");
                                }
                                else if (finalStates.Contains(states[i - 1]))
                                {
                                    Console.Write($" *{states[i - 1]}:\t");
                                }
                                else Console.Write($"  {states[i - 1]}:\t");
                            }
                            else Console.Write($"|{{0, {-maxLength - 1}}}|", transitions[states[i - 1]][j - 1]);
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Операция 'ShowTable' не может быть выполнена: автомат не проинициализирован.");
            }
        }
        private int MaxLengthForTable()
        {
            int maxLength = 0;
            foreach (var item in transitions.Values)
            {
                foreach (string state in item)
                {
                    if (maxLength < state.Length) maxLength = state.Length;
                }
            }
            return maxLength;
        }
        public bool ProcessInputLine(string word)
        {
            if (type == 1) return ProcessKDA(word);
            else return ProcessKNA(word);
        }
        private bool ProcessKDA(string word)
        {
            bool isOk = false;
            if (isInitiatedCorrectly)
            {
                bool EmergencyBreak = false;
                string currentState = initState;
                List<string> inputsList = inputs.ToList();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Состояния имеют следующий порядок: ");
                for (int i = 1; i <= states.Length; i++)
                {
                    if (i != states.Length)
                        Console.Write($"{i}-'{states[i - 1]}', ");
                    else
                        Console.Write($"{i}-'{states[i - 1]}'.\n");
                }
                Console.Write("Входные символы имеют следующий порядок: ");
                for (int i = 1; i <= inputs.Length; i++)
                {
                    if (i != inputs.Length)
                        Console.Write($"{i}-'{inputs[i - 1]}', ");
                    else
                        Console.Write($"{i}-'{inputs[i - 1]}'.\n");
                }
                Console.WriteLine($"\nТекущее состояние: {currentState}");
                Console.ResetColor();

                foreach (char symbol in word)
                {
                    if (inputs.Contains(symbol.ToString()))
                    {
                        Console.WriteLine($"Считан символ '{symbol}'");
                        currentState = transitions[currentState][inputsList.IndexOf(symbol.ToString())];
                        Console.WriteLine($" - Текущее состояние теперь {currentState}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка! Считанный символ '{symbol}' не входит в алфавит!");
                        Console.ResetColor();
                        EmergencyBreak = true;
                        break;
                    }
                }

                if (!EmergencyBreak)
                {
                    Console.WriteLine($"\nВходное слово успешно прочитано. Автомат пришёл в состояние {currentState}");
                    if (finalStates.ToList().Contains(currentState))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Состояние {currentState} входит в число финальных состояний.");
                        Console.ResetColor();
                        isOk = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Состояние {currentState} не входит в число финальных состояний.");
                        Console.ResetColor();
                    }
                }
                return isOk;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Операция 'ProcessInputLine' не может быть выполнена: автомат не проинициализирован.");
                Console.ResetColor();
                return false;
            }
        }
        private bool ProcessKNA(string word)
        {
            bool isOk = false;
            if (isInitiatedCorrectly)
            {
                bool EmergencyBreak = false;
                List<string> inputsList = inputs.ToList();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Состояния имеют следующий порядок: ");
                for (int i = 1; i <= states.Length; i++)
                {
                    if (i != states.Length)
                        Console.Write($"{i}-'{states[i - 1]}', ");
                    else
                        Console.Write($"{i}-'{states[i - 1]}'.\n");
                }
                Console.Write("Входные символы имеют следующий порядок: ");
                for (int i = 1; i <= inputs.Length; i++)
                {
                    if (i != inputs.Length)
                        Console.Write($"{i}-'{inputs[i - 1]}', ");
                    else
                        Console.Write($"{i}-'{inputs[i - 1]}'.\n");
                }
                Console.ResetColor();

                List<string> reachableStates = new List<string>();
                List<string> currentStates = [initState];

                Console.WriteLine($"\nТекущее состояние: {initState}");

                foreach (char symbol in word)
                {
                    if (inputs.Contains(symbol.ToString())) // если символ входит в алфавит
                    {
                        Console.WriteLine($"Считан символ '{symbol}'");
                        //рассматриваем все состояния, в которых мы находимся на текущем шаге
                        foreach (string item in currentStates)
                        {
                            string tempState = transitions[item][inputsList.IndexOf(symbol.ToString())];
                            if (tempState.Contains("{")) // если мы переходим не в одно состояние
                            {
                                tempState = tempState.Trim(['{', '}']);
                                foreach (string state in tempState.Split(',')) // сохраняем все достижимые состояния
                                {
                                    if (!reachableStates.Contains(state))
                                        reachableStates.Add(state);
                                }
                            }
                            else // если мы переходим в одно состояние
                            {
                                if (!reachableStates.Contains(tempState))
                                    reachableStates.Add(tempState); // сохраняем это достижимое состояние
                            }
                        }

                        
                        currentStates = new List<string>(reachableStates);
                        reachableStates.Clear();
                        Console.Write(" - Текущее(ие) состояние(ия): ");
                        foreach (string item in currentStates)
                        {
                            Console.Write($"'{item}', ");
                        }
                        Console.WriteLine();
                    }
                    // если символа нет в алфавите
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка! Считанный символ '{symbol}' не входит в алфавит!");
                        Console.ResetColor();
                        EmergencyBreak = true;
                        break;
                    }
                }
                if (!EmergencyBreak)
                {
                    // если хотя бы одно из текущих состояний входит в число конечных
                    if (finalStates.ToList().Any(x => currentStates.Any(y => y == x)))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Одно из состояний ");
                        foreach (string item in currentStates)
                        {
                            Console.Write($"'{item}', ");
                        }
                        Console.WriteLine("входит в число финальных состояний.");
                        Console.ResetColor();
                        isOk = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Ни одно из состояний ");
                        foreach (string item in currentStates)
                        {
                            Console.Write($"'{item}', ");
                        }
                        Console.WriteLine("не входит в число финальных состояний.");
                        Console.ResetColor();
                    }
                }
                return isOk;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Операция 'ProcessInputLine' не может быть выполнена: автомат не проинициализирован.");
                Console.ResetColor();
                return false;
            }
        }
    }
}
