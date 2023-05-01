using System.Diagnostics;

namespace lab2;

public class Lexer
{
    List<string> VariableTypesList = new()
    {
        "int", "double", "long", "string", "wchar_t",
        "char", "float", "bool", "short", "void",
        "int*", "double*", "long*", "string*", "wchar_t*",
        "char*", "float*", "bool*", "short*", "void*"
    };
    
    List<string> KeywordsList = new()
    {
        "and", "auto", "break", "case", "catch",
        "class", "const", "const_cast", "continue",
        "default", "delete", "do", "dynamic_cast",
        "else", "enum", "explicit", "export", "if",
        "false", "true", "for", "friend", "goto",
        "inline", "mutable", "namespace", "new",
        "nullptr", "operator", "or", "private",
        "public", "reinterpret_cast", "return",
        "sizeof", "static", "static_cast", "try",
        "switch", "template", "this", "throw",
        "struct", "unsigned", "using", "virtual",
        "while", "xor", "return", "include", "endl",
        "namespace", "std", "cin", "cout", "<iostream>",
        "length", "extern", "not", "protected", "signed",

    };
    
    List<string> OperatorsList = new ()
    {
        "++", "--", "<<", ">>", "+=", "-=", "*=", "/=", "%=",
        "<", ">", "<=", ">=", "==", "!=", "&&", "||", "!", "|",
        "&", "^", "~", "?:", "->", "+", "-", "*", "/", "%", "="
    };

    List<string> SpecialSymbolsList = new()
    {
        "{", "}", "(", ")", "[", "]", ",", "*", "~", ".", ";", "#"
    };
    
    public HashSet<string> FoundOperators = new();
    public HashSet<string> FoundKeyWords = new();
    public HashSet<string> FoundVariableTypes = new();
    public HashSet<string> FoundSpecialSymbols = new();
    public List<string> FoundErrors = new();
    public HashSet<string> FoundVariables = new();
    public HashSet<string> FoundConstants = new();
    public List<string> FoundStrings = new();

    public void Parse(string text)
    {
        text = text.Replace(";", " ; ")
            .Replace("\"", " \" ")
            .Replace("\'", " \' ")
            .Replace("(", " ( ")
            .Replace(")", " ) ")
            .Replace("[", " [ ")
            .Replace("]", " ] ")
            .Replace("{", " { ")
            .Replace("}", " } ")
            .Replace("#", " # ")
            .Replace(",", " , ") + " ";
        
        string currWord = "";
        bool isString = false;
        bool isVariable = false;
        var number = 0.0;
        int line = 1, pos = 1;

        foreach (var character in text)
        {
            if (character == '\n')
            {
                line++;
                pos = 1;
            }

            if (!Char.IsWhiteSpace(character))
            {
                currWord += character.ToString();
            }
            else
            {
                pos++;
                if (!currWord.Equals(String.Empty))
                {
                    if (currWord is "\"" or "\'")
                    {
                        isString = !isString;
                    }

                    if (isString || currWord is "\"" or "\'")
                    {
                        if (isString)
                            if (currWord is "\'" or "\"")
                                FoundStrings.Add(currWord);
                            else
                                FoundStrings[^1] += currWord + " ";
                        else
                        {
                            if (FoundStrings[^1] is "\"" or "\'")
                                FoundStrings[^1] += currWord;
                            else
                                FoundStrings[^1] = FoundStrings[^1].Remove(FoundStrings[^1].Length - 1, 1) + currWord;
                        }
                    }
                    else
                    {
                        // increment decrement
                        if (isVariable)
                        {
                            if (KeywordsList.Any(currWord.Equals))
                            {
                                FoundErrors.Add($"Token \"{currWord}\" at {line}:{pos - 1 - 1} is a reserved keyword");
                            }
                            else if (currWord.Contains('='))
                            {
                                var words = currWord.Split('=');
                                FoundVariables.Add(words[0]);
                                FoundOperators.Add("=");

                                if (double.TryParse(words[1], out number))
                                {
                                    FoundConstants.Add(words[1]);
                                }
                                else
                                {
                                    if (!words[1].Equals("") && !FoundVariables.Any(words[1].Contains))
                                        FoundErrors.Add($"Undefined token \"{words[1]}\" at {line}:{pos}");
                                }
                            }
                            else
                            {
                                FoundVariables.Add(currWord);
                            }

                            isVariable = false;
                        }
                        else if (FoundVariables.Contains(currWord) ||
                                 FoundVariables.Contains(currWord.Split('[', '.', ']')[0]) ||
                                 FoundVariables.Contains(currWord.Split('[', '.', ']')[0].Split("++")[0]
                                     .Split("--")[0]) ||
                                 FoundVariables.Contains(
                                     currWord.Split('[', '.', ']')[^1].Split("++")[^1].Split("--")[^1]))
                        {
                            if (currWord.Contains('.'))
                            {
                                if (KeywordsList.Contains(currWord.Split('.')[1]))
                                {
                                    FoundVariables.Add(currWord.Split('.')[0]);
                                    FoundKeyWords.Add(currWord.Split('.')[1]);
                                }
                                else
                                {
                                    FoundErrors.Add($"Undefined token \"{currWord}\" at {line}:{pos - 1 - 1}");
                                }
                            }
                            else if (currWord.Length >= 3 && (currWord.Contains('+') || currWord.Contains('-')))
                            {
                                if (currWord[^2..] is "++" or "--" &&
                                    !(currWord[..^2].Contains('+') || currWord[..^2].Contains('-')))
                                {
                                    FoundVariables.Add(currWord[..^2]);
                                    FoundOperators.Add(currWord[^2..]);
                                }
                                else if (currWord[..2] is "++" or "--" &&
                                         !(currWord[2..].Contains('+') || currWord[2..].Contains('-')))
                                {
                                    FoundVariables.Add(currWord[2..]);
                                    FoundOperators.Add(currWord[..2]);
                                }
                                else if (currWord[^2..] is not "++" or "--" &&
                                         !(currWord[..^2].Contains('+') || currWord[..^2].Contains('-')))
                                {
                                    FoundVariables.Add(currWord);
                                }
                                else if (currWord[..2] is not "++" or "--" &&
                                         !(currWord[2..].Contains('+') || currWord[2..].Contains('-')))
                                {
                                    FoundVariables.Add(currWord);
                                }
                                else
                                {
                                    FoundErrors.Add($"Undefined token \"{currWord}\" at {line}:{pos - 1 - 1}");
                                }
                            }
                            else if (OperatorsList.Any(currWord.Contains) || SpecialSymbolsList.Any(currWord.Contains))
                            {
                                FoundErrors.Add($"Undefined token \"{currWord}\" at {line}:{pos - 1 - 1}");
                            }
                            else
                            {
                                FoundVariables.Add(currWord);
                            }

                            isVariable = false;
                        }
                        else
                        {
                            if (KeywordsList.Contains(currWord))
                                FoundKeyWords.Add(currWord);
                            else if (OperatorsList.Contains(currWord))
                                FoundOperators.Add(currWord);
                            else if (VariableTypesList.Contains(currWord))
                            {
                                FoundVariableTypes.Add(currWord);
                                isVariable = true;
                            }
                            else if (SpecialSymbolsList.Contains(currWord))
                            {
                                FoundSpecialSymbols.Add(currWord);
                                pos -= 2;
                            }
                            else if (double.TryParse(currWord, out number) ||
                                     double.TryParse(currWord.Replace('.', ','), out number))
                            {
                                if (currWord[0].Equals('+'))
                                {
                                    FoundOperators.Add("+");
                                    FoundConstants.Add(currWord[1..]);
                                }
                                else if (currWord[0].Equals('-'))
                                {
                                    FoundOperators.Add("-");
                                    FoundConstants.Add(currWord[1..]);
                                }
                                else
                                {
                                    FoundConstants.Add(currWord);
                                }
                            }
                            else if (OperatorsList.Any(currWord.Contains))
                            {
                                /*Console.WriteLine(currWord);*/

                                foreach (var oper in OperatorsList)
                                {
                                    if (currWord.Contains(oper))
                                    {
                                        currWord = currWord.Replace($"{oper}", " ");
                                        FoundOperators.Add(oper);
                                    }
                                }

                                /*Console.WriteLine(currWord);*/
                                Parse(currWord);
                            }
                            else
                                FoundErrors.Add($"Undefined token \"{currWord}\" at {line}:{pos - 1 - 1}"); //-1 for cursor before error and -1 for remove whitespace pos++ 
                        }
                    }
                }

                pos += currWord.Length;
                currWord = "";
            }
        }
    }

    public void Print(string text)
    {
        Parse(text);
        /*if (FoundErrors.Count != 0)
        {
            Console.WriteLine("\nFound error:");
            Console.WriteLine(FoundErrors[0]);
            Environment.Exit(1);
        }*/

        Console.WriteLine("\nFound keywords:");
        foreach (var variable in FoundKeyWords)
        {
            Console.WriteLine(variable);
        }
        
        foreach (var variable in FoundVariableTypes)
        {
            Console.WriteLine(variable);
        }

        Console.WriteLine("\nFound identificators:");
        foreach (var variable in FoundVariables)
        {
            Console.WriteLine(variable);
        }
        
        Console.WriteLine("\nFound special symbols:");
        foreach (var variable in FoundSpecialSymbols)
        {
            Console.WriteLine(variable);
        }
        
        Console.WriteLine("\nFound strings:");
        foreach (var variable in FoundStrings)
        {
            Console.WriteLine(variable);
        }

        Console.WriteLine("\nFound constants:");
        foreach (var variable in FoundConstants)
        {
            Console.WriteLine(variable);
        }

        Console.WriteLine("\nFound operators:");
        foreach (var variable in FoundOperators)
        {
            Console.WriteLine(variable);
        }

        Console.WriteLine("\nFound error:");
        foreach (var variable in FoundErrors)
        {
            Console.WriteLine(variable);
        }
    }
}