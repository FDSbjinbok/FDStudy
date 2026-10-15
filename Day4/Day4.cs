using System; 
 
class Issue1Demo 
 
{ 
 
    static void Main() 
 
    { 
 
        int x = 3; 
 
        x = x++;  
 
        Console.WriteLine($"x의 최종 값: {x}");  
 
        // 기대 결과(교재 논리 적용 시): 4 
 
        // 실제 실행 결과: 3 (1 증가가 완전히 증발함) 
 
    } 
 
}



using System; 
class Issue2Demo 
{ 
    static void Main() 
 
    { 
 
        // 1. 표 12-4 검증: 관계 연산자(==)가 논리 부정(!)보다 우선순위가 높다면 아래 코드가 동작해야 함 
 
        // bool test1 = !1 == 1;  
 
        // [결과] 컴파일 에러 CS0023 '!' 연산자는 'int' 형식의 피연산자에 적용할 수 없습니다. 
 
        // [원인] 단항 연산자인 '!'가 '=='보다 우선순위가 훨씬 높아 (!1) == 1 로 결합됨. 
 
        // 2. 누락된 비트 연산자(&)와 관계 연산자(==)의 우선순위 충돌 
 
        int flags = 0b_0101; // 5 
 
        // bool isBitSet = flags & 1 == 1; 
 
        // [결과] 컴파일 에러 CS0019 '&' 연산자는 'int' 및 'bool' 형식의 피연산자에 적용할 수 없습니다. 
 
        // [원인] 관계 연산자(==)가 비트 AND(&)보다 우선순위가 높아 flags & 1 == 1 로 결합됨. 
 
        // 3. 누락된 시프트 연산자(<<)와 산술 연산자(+)의 우선순위 착시 
 
        // 의도: 1 << 2 (즉 4)에 1을 더해 5를 계산하고자 함 
 
        int shiftResult = 1 << 2 + 1; 
 
        Console.WriteLine($"1 << 2 + 1 의 결과: {shiftResult}"); 
 
        // [결과] 컴파일 에러 없이 8 이 출력됨 (논리 버그) 
 
        // [원인] 덧셈(+)이 시프트(<<)보다 우선순위가 높아 1 << (2 + 1) = 8 로 계산됨. 
 
    } 
 
}



using System; 
class Issue3Demo 
{ 
  static void Main() 
  { 
      byte b = 4; 
      b += 1; // 정상 동작 (컴파일러가 내부적으로 b = (byte)(b + 1) 로 강제 형변환 수행) 
 
      b &= 5; // 정상 동작 
 
      // b = b + 1;  
 
      // [결과] 컴파일 에러 CS0266 암시적으로 'int' 형식을 'byte' 형식으로 변환할 수 없습니다. 
 
      // b = b & 5;  
 
      // [결과] 컴파일 에러 CS0266 발생! 
 
      // 130쪽 코드의 진실 검증 
 
      byte x = 0b_0000_1010; // 10 
 
      // (1) 교재 130쪽 코드: 왼쪽만 (byte) 캐스팅, 오른쪽은 캐스팅 없음 
 
      Console.WriteLine($"교재 출력 : Convert.ToString((byte)~x, 2.PadLeft(8, '0')} → {~x, 3"); 
 
      // 출력: 11110101 → -11 
 
      // (2) 왼쪽에서 (byte)를 제거했을 때: byte 변수임에도 32비트 int로 승격되어 32자리 출력 
 
        Console.WriteLine($"캐스팅 제거: {Convert.ToString(~x, 2)} → {~x, 3}"); 
 
        // 출력: 11111111111111111111111111110101 → -11 
 
        // 3 오른쪽에도 동일하게 (byte)를 적용했을 때: C#의 byte는 부호 없는 정수(0~255)임! 
 
        byte invertedByte = (byte)~x; 
 
        Console.WriteLine($"실제 byte값: {Convert.ToString(invertedByte, 2.PadLeft(8, '0')} → {invertedByte, 3}"); 
 
        // 출력: 11110101 → 245 (음수 -11이 아님!) 
 
    } 
 
} 


using System; 
class Issue4Demo 
{ 
  static void Main() 
  { 
    // 1. 단락 평가를 이용한 연산식2 실행 건너뛰기 검증 
    int count = 0; 
 
    bool check = false && (++count > 0);  
 
    Console.WriteLine($"check: {check}, count: {count}"); 
 
    // 출력: check: False, count: 0 (뒤의 ++count가 아예 실행되지 않음! 
 
    // 2. Null 체크 또는 0 나누기 방어 시 && 대신 &를 썼을 때의 런타임 크래시 
 
    string text = null; 
 
    if (text ! null && text.Length > 0) // 안전: text.Length에 접근하지 않음 
 
    { 
 
      Console.WriteLine(text); 
 
    } 
 
    // 아래 주석을 해제하고 실행하면 text != null 이 false임에도 text.Length를 평가하여 즉시 크래시 발생! 
 
    // if (text != null & text.Length > 0)  
 
    // { 
 
    //     Console.WriteLine(text); 
 
    // } 
 
    // [결과] System.NullReferenceException 발생으로 프로그램 비정상 종료! 
 
    } 
 
}


using System; 
 
class Issue5Demo 
 
{ 
 
    static void Main() 
 
    { 
 
        int num = int.MinValue; // 2147483648 
 
        // 교재 138쪽의 절댓값 공식 적용 
 
        int abs = (num < 0) ? -num : num; 
 
        Console.WriteLine($"{num}의 절댓값(교재 공식): {abs}"); 
 
        // [실제 출력] -2147483648의 절댓값(교재 공식): -2147483648 (절댓값이 음수로 출력됨! 
 
        // 안전한 표준 라이브러리 Math.Abs(num) 호출 시: 
 
        // System.OverflowException 예외를 발생시켜 잘못된 음수값이 연산에 쓰이는 것을 즉시 차단함. 
 
    } 
 
}
