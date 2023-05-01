namespace lab2;

public static class Program
{
    public static void Main()
    {
        var text = File.ReadAllText("D:/labs/mtran/lab2/lab2/test.cpp");
        
        Console.WriteLine(text);
        Lexer lex = new Lexer();
        lex.Print(text);
    }
}