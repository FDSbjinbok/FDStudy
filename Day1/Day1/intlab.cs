using System; // 네임스페이스 선언부


namespace Day1 //네임스페이스
{
    class intlab
    {
        static void Main(string[] args)
        {
            ConsoleWrite._PrintVar();
            ConsoleWrite._Printpath();
        }
    }
    // 출력문
    class ConsoleWrite
    {
        public static void _WriteLine()
        {
            Console.Write("줄바꿈없음");
            Console.WriteLine("줄 바꿈 포함");
        }
        public static void _NewLine()
        {
            Console.WriteLine("줄\n바꿈");
        }
        //한줄 주석
        /*
        여러줄 주석
        */
        public static void _Printpath()
        {
            Console.WriteLine("C:\\Users\\jinbo\\Desktop\\스터디\\1주차");
            Console.WriteLine(@"C:\Users\jinbo\Desktop\스터디\1주차");
            Console.WriteLine("\"");//따옴표는 escatesequence 사용
            Console.WriteLine(@"안녕하세요.
C#을 공부하고 있습니다.");
        }
        public static void _PrintVar()
        {
            Console.WriteLine("{1},{0}", "Hello", "C#");
            int n = 7;
            Console.WriteLine($"{n}");
            Console.WriteLine("{0}", n);
        }

    }

}
