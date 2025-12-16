using System.Security.Cryptography;
using System.Text;

public static class RobustCodeGenerator
{
    static long counter = 0;
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    const long Salt = 0x7E3779B97F4A7C15L;


    public static string Generate(int length)
    {
        long value = Interlocked.Increment(ref counter);

        value ^= Salt;

        Span<char> buffer = stackalloc char[length];
        for (int i = length - 1; i >= 0; i--)
        {
            buffer[i] = chars[(int)(value % 36)];
            value /= 36;
        }

        return new string(buffer);
    }

    //const long A = 6364136223846793005;
    //const long C = 1442695040888963407;

    //public static string Generate(int length)
    //{
    //    long value = Interlocked.Increment(ref counter);
    //    value = unchecked(A * value + C);

    //    Span<char> buffer = stackalloc char[length];
    //    for (int i = length - 1; i >= 0; i--)
    //    {
    //        buffer[i] = chars[(int)(value % 36)];
    //        value /= 36;
    //    }

    //    return new string(buffer);
    //}
}

public class Program
{
    public static void Main()
    {
        const int total = 1000_000_000;
        var codes = new HashSet<string>();
        var filePath = "codes.txt";

        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        Console.WriteLine($"Gerando {total:N0} códigos únicos...");

        int duplicates = 0;
        for (int i = 0; i < total; i++)
        {
            var code = RobustCodeGenerator.Generate(8);

            if (!codes.Add(code))
            {
                duplicates++;
                Console.WriteLine($"Duplicado detectado: {code}");
            }

            writer.WriteLine(code);

            if (i % 1_000_000 == 0 && i > 0)
                Console.WriteLine($"{i:N0} códigos gerados...");
        }

        Console.WriteLine($"Finalizado! Duplicados encontrados: {duplicates}");
        Console.WriteLine($"Arquivo salvo em: {Path.GetFullPath(filePath)}");
    }
}