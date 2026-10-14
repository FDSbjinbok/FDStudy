
using System;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("===== C# 스터디 =====");
            Console.WriteLine("7. 7장");
            Console.WriteLine("8. 8장");
            Console.WriteLine("9, 9장");
            Console.WriteLine("0. 종료");
            Console.Write("실행할 장 선택: ");

            string? input = Console.ReadLine();
            Console.WriteLine();

            switch (input)
            {
                case "7":
                    Chapter7();
                    break;
                case "8":
                    Chapter8();
                    break;
                case "9":
                    Console.WriteLine("9장은 작성된 코드가 없어요.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("올바른 번호를 입력하세요.");
                    break;
            }

            Console.WriteLine();
        }
    }

    static void Chapter7()
    {
        // 7
        char grade = 'A';
        string a = "안녕하세요. 반갑습니다.";
        // 에러
        //string b = "
        //    안녕하세요.
        //    반갑습니다.
        //    ";
        // @ 붙이면 오류 안남
        string c = @"
            안녕하세요.
            반갑습니다.\n
            ";
        Console.WriteLine(a);
        Console.WriteLine(c);

        string message = "String Interpolation";
        Console.WriteLine("Message : {0}", message);
        Console.WriteLine("Message : " + message);
        Console.WriteLine($"Message : {message}");
        Console.WriteLine();

        string name = "C#";
        string version = "14";
        Console.WriteLine($"{name} {version}");
        Console.WriteLine();

        Console.WriteLine(DateTime.Now);
        Console.WriteLine($"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}");
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
        Console.WriteLine();
    }

    static void Chapter8()
    {
        // 8
        Console.Write("키 입력: ");
        ConsoleKeyInfo key = Console.ReadKey();
        Console.WriteLine($"\n입력한 키: {key.Key}");
        Console.WriteLine($"입력한 문자: {key.KeyChar}");
        // 보조 키는 입력 안됨. 아래 코드로 따로 확인 가능
        Console.WriteLine(key.Modifiers);
        Console.WriteLine();

        long n1 = 1234;
        // int n2 = n1; // 오류(명시적 형 변환 필요 int(n1))

        long l = long.MaxValue;
        Console.WriteLine($"l의 값 : {l}");
        int i = (int)l;
        Console.WriteLine($"i의 값 : {i}");
        Console.WriteLine();

        byte x = 10;

        Console.WriteLine(
            $"십진후 : {x} -> 이진수 : {Convert.ToString(x, 2).PadLeft(8, '0')}"); // x를 2진수로 8칸 기준으로 보여줌
        Console.WriteLine();

        int dec = 1_000_000;
        Console.WriteLine(dec);
        Console.WriteLine();

        var t = (100, 200, 300, 400);
        Console.WriteLine(t.Item4);
        Console.WriteLine();
    }
}
