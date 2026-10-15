using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Issue 1 ===");
        Issue1Demo.Run();

        Console.WriteLine("\n=== Issue 2 ===");
        Issue2Demo.Run();

        Console.WriteLine("\n=== Issue 3 ===");
        Issue3Demo.Run();

        Console.WriteLine("\n=== Issue 4 ===");
        Issue4Demo.Run();

        Console.WriteLine("\n=== Issue 5 ===");
        Issue5Demo.Run();
    }
}

class Issue1Demo
{
    public static void Run()
    {
        int x = 3;
        x = x++;

        Console.WriteLine($"x의 최종 값: {x}");

        // 단순히 증가 연산의 효과만 고려하면 4로 오해할 수 있음.
        // 실제 실행 결과: 3
        // 원인: x++는 증가 전 값 3을 반환함.
        //       증가된 값 4가 뒤이은 대입에 의해 다시 3으로 덮어씌워짐.
    }
}

class Issue2Demo
{
    public static void Run()
    {
        // 1. 동등 연산자(==)와 논리 부정 연산자(!)의 우선순위 검증
        // bool test1 = !1 == 1;
        //
        // [결과] 컴파일 오류 CS0023
        // [원인] 단항 연산자인 !가 ==보다 우선순위가 높음.
        //       따라서 (!1) == 1로 결합되며,
        //       ! 연산자를 int 형식에 적용할 수 없어 오류가 발생함.
        //
        // 올바른 논리 부정 표현:
        // bool test1 = !(1 == 1);  // false

        // 2. 비트 AND(&)와 동등 연산자(==)의 우선순위 충돌
        int flags = 0b_0101; // 5

        // bool isBitSet = flags & 1 == 1;
        //
        // [결과] 컴파일 오류 CS0019
        // [원인] 동등 연산자(==)가 비트 AND(&)보다 우선순위가 높음.
        //       따라서 flags & (1 == 1)로 결합됨.
        //       int와 bool 사이에 비트 AND 연산을 시도하므로 오류가 발생함.

        // 올바른 비트 검사 표현:
        bool isBitSet = (flags & 1) == 1;
        Console.WriteLine($"첫 번째 비트 설정 여부: {isBitSet}");

        // 3. 시프트 연산자(<<)와 산술 연산자(+)의 우선순위 검증
        // 의도: 1 << 2의 결과인 4에 1을 더해 5를 계산하고자 함.
        int shiftResult = 1 << 2 + 1;

        Console.WriteLine($"1 << 2 + 1 의 결과: {shiftResult}");

        // [결과] 컴파일 오류 없이 8이 출력됨.
        // [원인] 덧셈(+)이 시프트(<<)보다 우선순위가 높음.
        //       따라서 1 << (2 + 1) = 1 << 3 = 8로 계산됨.

        // 의도한 결과를 얻으려면 괄호를 사용해야 함.
        int correctedShiftResult = (1 << 2) + 1;

        Console.WriteLine($"(1 << 2) + 1 의 결과: {correctedShiftResult}");
    }
}

class Issue3Demo
{
    public static void Run()
    {
        byte b = 4;

        b += 1; // 정상 동작: 결과를 byte 형식에 맞춰 대입함.
        b &= 5; // 정상 동작: 비트 AND 결과를 byte 형식에 맞춰 대입함.

        Console.WriteLine($"복합 대입 후 b의 값: {b}");

        // b = b + 1;
        // [결과] 컴파일 오류 CS0266
        // [원인] b + 1의 결과는 int이므로 byte에 암시적으로 대입할 수 없음.

        // b = b & 5;
        // [결과] 컴파일 오류 CS0266
        // [원인] 비트 AND 연산 결과도 int이므로 byte에 암시적으로 대입할 수 없음.

        // 130쪽 코드 검증
        byte x = 0b_0000_1010; // 10

        // (1) 왼쪽은 byte로 변환하지만 오른쪽은 int인 경우
        Console.WriteLine(
            $"교재 출력 : {Convert.ToString((byte)~x, 2).PadLeft(8, '0')} → {~x,3}"
        );

        // 출력: 11110101 → -11
        // 왼쪽은 byte의 8비트 패턴이고, 오른쪽은 int 값임.
        // 따라서 비트 표현과 정수 값이 서로 다른 기준으로 표시됨.

        // (2) 왼쪽의 byte 캐스팅을 제거한 경우
        Console.WriteLine(
            $"캐스팅 제거: {Convert.ToString(~x, 2)} → {~x,3}"
        );

        // 출력: 11111111111111111111111111110101 → -11
        // byte 변수 x에 ~를 적용하면 int로 승격되어 연산됨.
        // 따라서 32비트 정수 기준의 비트 표현이 출력됨.

        // (3) 비트 NOT 결과를 byte로 변환한 경우
        byte invertedByte = (byte)~x;

        Console.WriteLine(
            $"실제 byte값: {Convert.ToString(invertedByte, 2).PadLeft(8, '0')} → {invertedByte,3}"
        );

        // 출력: 11110101 → 245
        // byte는 0~255의 범위를 갖는 부호 없는 정수임.
        // 따라서 하위 8비트 11110101은 245로 해석됨.
        // 반면 같은 비트 패턴을 32비트 int로 해석한 ~x의 값은 -11임.
    }
}

class Issue4Demo
{
    public static void Run()
    {
        // 1. 논리 AND(&&)의 단락 평가 검증
        int count = 0;

        bool check = false && (++count > 0);

        Console.WriteLine($"check: {check}, count: {count}");

        // 출력: check: False, count: 0
        // 왼쪽 피연산자가 false이므로 오른쪽 식은 평가되지 않음.
        // 따라서 ++count도 실행되지 않음.

        // 2. null 검사에서 &&와 &의 차이
        string text = null;

        if (text != null && text.Length > 0)
        {
            Console.WriteLine(text);
        }

        // 안전하게 실행됨.
        // text != null이 false이므로 text.Length에 접근하지 않음.

        // 아래 조건문의 주석을 해제하면 NullReferenceException이 발생함.
        //
        // if (text != null & text.Length > 0)
        // {
        //     Console.WriteLine(text);
        // }
        //
        // [원인] bool에 사용하는 &는 양쪽 피연산자를 모두 평가함.
        //        따라서 text가 null이어도 text.Length를 평가하게 됨.
        //
        // 주의: 위의 & 사례는 현재 주석 처리되어 있으므로
        //       이 프로그램의 기본 실행에서는 예외가 발생하지 않음.
    }
}

class Issue5Demo
{
    public static void Run()
    {
        int num = int.MinValue; // -2147483648

        // 교재 138쪽의 절댓값 공식 검증
        // 기본 unchecked 문맥에서의 결과
        int abs = (num < 0) ? -num : num;

        Console.WriteLine(
            $"{num}의 절댓값(교재 공식): {abs}"
        );

        // 기본 unchecked 문맥에서 출력되는 값:
        // -2147483648의 절댓값(교재 공식): -2147483648
        //
        // int의 최댓값은 2147483647이므로
        // int.MinValue의 절댓값 2147483648은 int로 표현할 수 없음.
        //
        // checked 문맥에서는 -num 연산에서
        // OverflowException이 발생할 수 있음.

        // Math.Abs(int.MinValue)의 실제 예외 검증
        try
        {
            int safeAbs = Math.Abs(num);
            Console.WriteLine($"Math.Abs 결과: {safeAbs}");
        }
        catch (OverflowException ex)
        {
            Console.WriteLine(
                $"Math.Abs 예외: {ex.GetType().Name}"
            );
        }

        // 출력: Math.Abs 예외: OverflowException
        // Math.Abs(int.MinValue)는 표현할 수 없는 양수 결과를
        // 반환하지 않고 OverflowException을 발생시킴.
    }
}
