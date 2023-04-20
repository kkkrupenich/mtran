using System.Text.RegularExpressions;

namespace lab2;

public static class Program
{
    public static void Main()
    {
        var VariableTypesList = new List<string>()
        {
            "int", "double", "long", "string", "wchar_t",
            "char", "float", "bool", "short", "void"
        };

        var KeywordsList = new List<string>()
        {
            "and", "auto", "break", "case", "catch",
            "class", "const", "const_cast", "continue",
            "default", "delete", "do", "dynamic_cast",
            "else", "enum", "explicit", "export", "extern",
            "false", "true", "for", "friend", "goto", "if", 
            "inline", "mutable", "namespace", "new", "not", 
            "nullptr", "operator", "or", "private", "protected", 
            "public", "reinterpret_cast", "return", "signed", 
            "sizeof", "static", "static_cast", "struct", "switch", 
            "template", "this", "throw", "try", "unsigned", "using", 
            "virtual", "while", "xor", "return", "#include", 
            "namespace", "std", "cin", "cout", "endl"
        };

        var OperatorsList = new List<string>()
        {
            "+", "-", "*", "/", "%", "=", "&"
        };

        var text = File.ReadAllText("../../../test.cpp")
            .Replace(";", " ;")
            .Replace("\"", " \" ")
            .Replace("(", " ( ")
            .Replace(")", " ) ");
        var FoundOperators = new HashSet<string>();
        var FoundKeyWords = new HashSet<string>();
        var FoundVariables = new HashSet<string>();

        string currWord = "";
        
        foreach (var character in text)
        {
            if (!Char.IsWhiteSpace(character))
            {
                currWord += character.ToString();
            }
            else
            {
                if (!currWord.Equals(String.Empty))
                {
                    Console.WriteLine($"currWord is: {currWord}");
                    if (KeywordsList.Contains(currWord) || KeywordsList.Contains(currWord.Replace(";", "")))
                        FoundKeyWords.Add(currWord);
                    if (OperatorsList.Contains(currWord))
                        FoundOperators.Add(currWord);
                    if (VariableTypesList.Contains(currWord))
                        FoundVariables.Add(currWord);
                }
                currWord = "";
            }
        }

        Console.WriteLine("\nFound operators:");
        foreach (var oper in FoundOperators)
        {
            Console.WriteLine(oper);
        }
        
        Console.WriteLine("\nFound keywords:");
        foreach (var keyword in FoundKeyWords)
        {
            Console.WriteLine(keyword);
        }
        
        Console.WriteLine("\nFound variable types:");
        foreach (var variable in FoundVariables)
        {
            Console.WriteLine(variable);
        }
    }
}