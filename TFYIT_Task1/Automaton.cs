using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TFYIT_Task1
{
    internal class Automaton
    {
        public uint type;
        private string[]? states;
        private string[]? inputs;
        private string[]? finalStates;
        private string? initState;
        private Dictionary<string, List<string>>? transitions;
        private Dictionary<string, int>? inpIndexes;
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
            isInitiatedCorrectly = true;
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
                            if (j == 0) Console.Write($" {{0, {-maxLength - 2}}}:\t", "");
                            else Console.Write($"|{{0, {-maxLength - 1}}}|", inputs[j - 1]);
                        }
                    }
                    else // для остальных строк
                    {
                        for (int j = 0; j < inputs.Length + 1; j++)
                        {
                            if (j == 0)
                            {
                                if (states[i - 1] == initState && finalStates.Contains(states[i - 1]))
                                {
                                    Console.Write($"->*{{0, {-maxLength}}}:\t", states[i - 1]);
                                }
                                else if (states[i - 1] == initState)
                                {
                                    Console.Write($"->{{0, {-maxLength - 1}}}:\t", states[i - 1]);
                                }
                                else if (finalStates.Contains(states[i - 1]))
                                {
                                    Console.Write($" *{{0, {-maxLength - 1}}}:\t", states[i - 1]);
                                }
                                else Console.Write($"  {{0, {-maxLength - 1}}}:\t", states[i - 1]);
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
            else if (type == 2) return ProcessKNA(word);
            else return ProcessKNA_E(word);
        }
        private bool ProcessKDA(string word)
        {
            bool isOk = false;
            if (isInitiatedCorrectly)
            {
                bool EmergencyBreak = false;
                string currentState = initState;
                List<string> inputsList = inputs.ToList();

                Console.WriteLine($"\nТекущее состояние: {currentState}");

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
        private bool ProcessKNA_E(string word)
        {
            bool isOk = false;
            bool deadEnd = false;
            bool eps_cycle = false;
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
                List<string> eps_shortcut = new List<string>();
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

                        // обновляем список текущих состояний
                        if (reachableStates.Count != 1)
                            reachableStates.Remove("~");
                        currentStates = new List<string>(reachableStates);
                        reachableStates.Clear();  

                        // если текущее состояние - пустое множество
                        if (currentStates.Count == 1 && currentStates[0] == "~")
                        {
                            deadEnd = true;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Из текущего состояния недостижимы другие. Тупик.");
                            Console.ResetColor();
                            break;
                        }

                        // обработка эпсилон-переходов, поиск замыканий
                        int i = 0;
                        while (i < currentStates.Count)
                        {
                            string toAdd = "";
                            string item = currentStates[i];
                            if (transitions[item][inputs.Length - 1] != "~")
                            {
                                // добавляем текущее состояние в замыкание
                                eps_shortcut.Add(item);
                                // и смежные с ним
                                toAdd = transitions[item][inputs.Length - 1];
                                if (!eps_shortcut.Contains(toAdd))
                                    eps_shortcut.Add(toAdd);
                                else
                                {
                                    eps_cycle = true; // если обнаружен цикл по эпсилон-переходам
                                    break;
                                }
                            }
                            if (toAdd != "") // если добавляемое состояние не пустое
                            {
                                if (!toAdd.Contains('{')) // если состояние однозначное
                                {
                                    if (!currentStates.Contains(toAdd))
                                        currentStates.Add(toAdd);
                                }
                                else // если состояние неоднозначное
                                {
                                    string tempState = toAdd.Trim(['{', '}']);
                                    foreach (string state in tempState.Split(','))
                                    {
                                        if (!currentStates.Contains(state))
                                            currentStates.Add(state);
                                    }
                                }
                            }
                            i++;
                        }

                        if (eps_cycle)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Обнаружен цикл по эпсилон-переходам. Программа будет остановлена.");
                            eps_shortcut = eps_shortcut.Distinct().ToList();
                            for (int j = 0; j < eps_shortcut.Count; j++)
                            {
                                Console.Write($"{eps_shortcut[j]} -> ");
                            }
                            Console.WriteLine("...");
                            Console.ResetColor();
                            break;
                        }

                        bool epsAdded = false;
                        // к текущим состояниям добавляем состояния из эпсилон-замыкания
                        foreach (string item in eps_shortcut)
                        {
                            if (!item.Contains('{')) // если состояние однозначное
                            {
                                if (!currentStates.Contains(item))
                                    currentStates.Add(item);
                                epsAdded = true;
                            }
                            else // если состояние неоднозначное
                            {
                                string tempState = item.Trim(['{', '}']);
                                foreach (string state in tempState.Split(',')) 
                                {
                                    if (!currentStates.Contains(state))
                                        currentStates.Add(state);
                                }
                                epsAdded = true;
                            }
                        }

                        if (epsAdded)
                            Console.WriteLine("Был осуществлён эпсилон-переход");

                        Console.Write(" - Текущее(ие) состояние(ия): ");
                        currentStates.Sort();
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
                if (!EmergencyBreak && !deadEnd && !eps_cycle) // если чтение завершилось без ошибок и мы не в тупике
                {
                    // если хотя бы одно из текущих состояний входит в число конечных, сообщаем об этом
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
                else if (!EmergencyBreak && deadEnd) // если попали в тупик
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Чтение строки невозможно продолжить");
                    Console.ResetColor();
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
        public Automaton KNAtoKDA()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nВыполняется преобразование НКА к ДКА:\n");
            Console.ResetColor();

            List<string> newStates = new List<string>(); // список новых состояний
            List<string> newFinalStates = new List<string>(); // список новых финальных состояний
            Dictionary<string, List<string>> newTransitions = new Dictionary<string, List<string>>();
            bool run = true;

            newStates.Add(initState);
            string currentState = initState;

            int i = 0;
            while (run)
            {
                if (!newStates[i].Contains("{")) // если текущее состояние однозначно
                {
                    foreach (string item in transitions[currentState]) // рассматриваем все смежные состояния
                    {
                        if (!newStates.Contains(item)) // и если следующего состояния нет в новом списке
                        {
                            newStates.Add(item); // то добавляем его

                            // проверяем, входит ли полученное состояние в число финальных
                            string[] tempStates = item.Trim(['{', '}']).Split(',');
                            bool isFinal = false;
                            foreach (string state in tempStates)
                            {
                                if (finalStates.Contains(state))
                                {
                                    isFinal = true;
                                    break;
                                }
                            }
                            if (isFinal)
                                newFinalStates.Add(item);
                        }
                    }
                    newTransitions.Add(currentState, transitions[currentState]);
                }
                else // если текущее состояние не однозначно
                {
                    List<string> allReachableStates = new List<string>(); // временный список для всех состояний,
                                                                         // достижимых из текущго неоднозначного

                    string tempStates = currentState.Trim(['{', '}']); // разделяем текущее состояние на однозначные
                    List<HashSet<string>> reachableFromInputs = new List<HashSet<string>>(inputs.Length);
                    for (int j = 0; j < inputs.Length; j++) // инициализация хешсетов
                    {
                        reachableFromInputs.Add(new HashSet<string>());
                    }


                    foreach (string state in tempStates.Split(','))   // и рассматриваем по одному
                    {
                        for (int j = 0; j < inputs.Length; j++)
                        {
                            string[] splitted = transitions[state][j].Trim(['{', '}']).Split(",");
                            foreach (string symbol in splitted)
                            {
                                reachableFromInputs[j].Add(symbol);
                            }
                        }
                    }


                    List<string> results = new List<string>(); // вспомогат. список, в котором будут все
                                                              // результирующие состояния для этого шага
                    // форматируем достижимые состояния для их добавления в список новых
                    foreach (var item in reachableFromInputs)
                    {
                        string[] statesArray = item.ToArray();
                        Array.Sort(statesArray);
                        bool isFinal = false;

                        if (statesArray.Length > 1) // если получили более 1 состояния (т.е. {...})
                        {
                            string result = "{";
                            foreach (string elem in statesArray)
                            {
                                result += elem + ",";
                                if (finalStates.Contains(elem))
                                    isFinal = true;
                            }

                            result = result.TrimEnd(',') + "}";

                            if (isFinal)
                                newFinalStates.Add(result);
                            results.Add(result);

                            if (!newStates.Contains(result)) // если такое состояние ещё не встречалось
                            {
                                newStates.Add(result);
                            }
                        }
                        else // если получили одно состояние
                        {
                            if (finalStates.Contains(statesArray[0]))
                                newFinalStates.Add(statesArray[0]);
                            results.Add(statesArray[0]);

                            if (!newStates.Contains(statesArray[0]))
                            {
                                newStates.Add(statesArray[0]);
                            }
                        }
                    }
                    newTransitions.Add("{" + tempStates + "}", new List<string>(results));
                    results.Clear();
                }


                i++;
                if (i >= newStates.Count)
                {
                    run = false;
                    break;
                }
                currentState = newStates[i];
            }

            //foreach (var item in newTransitions)
            //{
            //    Console.Write($"{item.Key}: ");
            //    foreach (string elem in item.Value)
            //    {
            //        Console.Write($"{elem} ");
            //    }
            //    Console.WriteLine();
            //}

            return new Automaton(1, newStates.ToArray(), inputs, newFinalStates.ToArray(), initState, newTransitions);
        }
    }
}
