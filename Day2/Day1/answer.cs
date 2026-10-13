#define ON
#if ON
using System;

using System;
using System.Text;

namespace Day1.Answer
{
    /// <summary>
    /// IEEE 754 float 실습 과제 풀이
    /// 과제 1~4 구현 + 과제 5 탐구 문제 답
    /// </summary>
    internal class FloatBitsAnswer
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // ✅ 이모지 출력용

            // 과제 1
            PrintFloatBits(6.25f);

            // 과제 2
            var (s, e, m) = Decompose(6.25f);
            Console.WriteLine($"sign={s}, exponent={e} (실제 {e - 127}), mantissa=0x{m:X6}");

            // 과제 3
            foreach (var v in new[] { 6.25, -6.25, 0.5, 13.375, 100.0 })
            {
                uint mine = EncodeFloat(v);
                uint real = (uint)BitConverter.SingleToInt32Bits((float)v);
                Console.WriteLine($"{v,8}: 0x{mine:X8} vs 0x{real:X8} {(mine == real ? "✅" : "❌")}");
            }

            // 과제 4
            Console.WriteLine(DecodeFloat(0x40C80000)); // 6.25
            Console.WriteLine(DecodeFloat(0xC1560000)); // -13.375

            Console.WriteLine("\n=== 과제 5: 탐구 ===");
            Explore();
        }

        // ───────────────────────────────────────────────
        // 과제 1: 비트 패턴 출력
        // ───────────────────────────────────────────────
        static void PrintFloatBits(float value)
        {
            int bits = BitConverter.SingleToInt32Bits(value);

            // 음수 int도 Convert.ToString(_, 2)는 2의 보수 32자리를 그대로 돌려준다.
            string b = Convert.ToString(bits, 2).PadLeft(32, '0');

            string sign = b.Substring(0, 1);   // 1비트
            string exponent = b.Substring(1, 8);   // 8비트
            string mantissa = b.Substring(9, 23);  // 23비트

            Console.WriteLine($"{value,12}: {sign} | {exponent} | {mantissa}  (0x{bits:X8})");
        }

        // ───────────────────────────────────────────────
        // 과제 2: 시프트와 마스크로 필드 분해
        // ───────────────────────────────────────────────
        static (int sign, int exponent, int mantissa) Decompose(float value)
        {
            int bits = BitConverter.SingleToInt32Bits(value);

            // 주의: int의 >> 는 산술 시프트라서 음수면 왼쪽이 1로 채워진다.
            //       & 1 을 해 주지 않으면 음수일 때 sign 이 -1 이 된다.
            int sign = (bits >> 31) & 1;
            int exponent = (bits >> 23) & 0xFF;      // 8비트 마스크
            int mantissa = bits & 0x7FFFFF;          // 하위 23비트 마스크

            return (sign, exponent, mantissa);
        }

        // ───────────────────────────────────────────────
        // 과제 3: 손 계산 과정을 그대로 코드로 (BitConverter 미사용)
        //  - 정규화 범위의 수만 처리 (비정규화/무한대/NaN 제외)
        //  - 반올림은 IEEE 754 기본 방식인 "가장 가까운 짝수로 반올림"
        // ───────────────────────────────────────────────
        static uint EncodeFloat(double value)
        {
            // 1) 부호
            uint sign = value < 0 || (value == 0 && double.IsNegative(value)) ? 1u : 0u;
            value = Math.Abs(value);
            if (value == 0) return sign << 31;   // ±0

            // 2) 정수부 → 2진수 (6 → "110")
            long intPart = (long)Math.Floor(value);
            double frac = value - intPart;
            string intBits = intPart > 0 ? Convert.ToString(intPart, 2) : "";

            // 3) 소수부 → 2진수 (0.25 → "01"), 2를 곱해서 넘어온 자리를 읽는다
            var fracBits = new StringBuilder();
            while (frac > 0 && fracBits.Length < 1100)
            {
                frac *= 2;
                if (frac >= 1) { fracBits.Append('1'); frac -= 1; }
                else { fracBits.Append('0'); }
            }

            // 4) 정규화: 1.xxxx × 2^exponent 형태로 만든다
            int exponent;
            string afterPoint;   // 맨 앞 1 뒤의 모든 비트 (숨은 비트 제외)
            if (intBits.Length > 0)
            {
                // 110.01 → 1.1001 × 2^2 : 소수점을 왼쪽으로 (정수부 길이 - 1) 칸 이동
                exponent = intBits.Length - 1;
                afterPoint = intBits.Substring(1) + fracBits.ToString();
            }
            else
            {
                // 0.00101 → 1.01 × 2^-3 : 첫 번째 1이 나올 때까지 오른쪽으로 이동
                string f = fracBits.ToString();
                int firstOne = f.IndexOf('1');
                exponent = -(firstOne + 1);
                afterPoint = f.Substring(firstOne + 1);
            }

            // 5) 가수 23비트 + 반올림 (round to nearest, ties to even)
            string padded = afterPoint.PadRight(24, '0');
            uint mantissa = Convert.ToUInt32(padded.Substring(0, 23), 2);
            bool guard = padded[23] == '1';                                   // 버려지는 첫 비트
            bool sticky = afterPoint.Length > 24 && afterPoint.Substring(24).Contains('1'); // 그 뒤에 1이 있나

            if (guard && (sticky || (mantissa & 1) == 1))
            {
                mantissa++;
                if (mantissa == 0x800000)   // 1.111...1 + 1 → 10.000...0 : 자리올림
                {
                    mantissa = 0;
                    exponent++;
                }
            }

            // 6) 바이어스 더하고 조립
            uint biasedExponent = (uint)(exponent + 127);
            return (sign << 31) | (biasedExponent << 23) | mantissa;
        }

        // ───────────────────────────────────────────────
        // 과제 4: 공식 (−1)^s × 1.m × 2^(e−127) 으로 디코딩
        // ───────────────────────────────────────────────
        static float DecodeFloat(uint bits)
        {
            int sign = (int)(bits >> 31);
            int exponent = (int)((bits >> 23) & 0xFF);
            uint mantissa = bits & 0x7FFFFF;

            // 지수 비트가 모두 1 → 무한대 또는 NaN
            if (exponent == 0xFF)
            {
                if (mantissa != 0) return float.NaN;
                return sign == 1 ? float.NegativeInfinity : float.PositiveInfinity;
            }

            double significand;
            int realExponent;
            if (exponent == 0)
            {
                // 비정규화 수: 숨은 비트가 0이고 지수는 -126 고정
                significand = mantissa / (double)(1 << 23);
                realExponent = -126;
            }
            else
            {
                // 정규화 수: 숨은 비트 1을 붙인다
                significand = 1.0 + mantissa / (double)(1 << 23);
                realExponent = exponent - 127;
            }

            double result = significand * Math.Pow(2, realExponent);
            return (float)(sign == 1 ? -result : result);
        }

        // ───────────────────────────────────────────────
        // 과제 5: 탐구 문제
        // ───────────────────────────────────────────────
        static void Explore()
        {
            // Q1. 0.1f
            PrintFloatBits(0.1f);           // 0 | 01111011 | 10011001100110011001101
            Console.WriteLine($"0.1f + 0.2f == 0.3f : {0.1f + 0.2f == 0.3f}");   // True
            Console.WriteLine($"0.1  + 0.2  == 0.3  : {0.1 + 0.2 == 0.3}");      // False
            // A1. 0.1 = 0.000110011001100...₂ 로 '0011'이 무한 반복된다.
            //     (1/10 의 분모에 5가 있어서 2진수로 유한하게 끝나지 않음)
            //     23비트에서 잘라 반올림하므로 마지막이 ...1101 이 되고, 실제 값은 0.100000001490116...
            //     float 에서는 0.1f + 0.2f 의 오차가 반올림으로 우연히 0.3f 와 같은 비트가 되어 True,
            //     double 에서는 0.30000000000000004 가 되어 False.
            //     → 결론: 실수는 == 로 비교하지 말고 Math.Abs(a - b) < 허용오차 로 비교한다.

            // Q2. +0 과 -0
            PrintFloatBits(0.0f);           // 0 | 00000000 | 000...0  (0x00000000)
            PrintFloatBits(-0.0f);          // 1 | 00000000 | 000...0  (0x80000000)
            Console.WriteLine($"0.0f == -0.0f : {0.0f == -0.0f}");               // True
            // A2. 부호 비트만 다르다. 비트는 다르지만 표준에서 같은 값으로 정의하므로 == 는 True.
            //     차이는 1 / 0.0f = +∞, 1 / -0.0f = -∞ 처럼 나눗셈에서 드러난다.

            // Q3. 무한대와 NaN
            PrintFloatBits(float.PositiveInfinity);   // 0 | 11111111 | 000...0  (0x7F800000)
            PrintFloatBits(float.NaN);                // 1 | 11111111 | 100...0  (0xFFC00000, .NET 기본 NaN)
            Console.WriteLine($"NaN == NaN : {float.NaN == float.NaN}");         // False
            // A3. 지수 비트가 모두 1 이면 특수값. 가수가 0 이면 무한대, 0 이 아니면 NaN.
            //     NaN 은 자기 자신과도 같지 않으므로 검사는 float.IsNaN() 을 써야 한다.

            // Q4. float.Epsilon
            PrintFloatBits(float.Epsilon);            // 0 | 00000000 | 000...01 (0x00000001)
            // A4. float 가 표현할 수 있는 가장 작은 양수(약 1.4e-45)로, 비정규화 수다.
            //     정규화 수의 최소값은 2^-126 인데, 그보다 작은 수는 숨은 비트를 0 으로 두고
            //     0.m × 2^-126 으로 표현한다. 이때 지수 비트는 모두 0 이다.
            //     값 = 2^-23 × 2^-126 = 2^-149
            //     (※ double.Epsilon 처럼 '1 다음 수와의 차이'가 아니라는 점에 주의)

            // Q5. 16777216f + 1f
            Console.WriteLine($"16777216f + 1f = {16777216f + 1f:F0}");          // 16777216
            // A5. 16777216 = 2^24. 가수는 숨은 비트 포함 24비트이므로 2^24 이상에서는
            //     인접한 float 사이의 간격이 2 가 된다. 16777217 은 16777216 과 16777218 의
            //     정확히 중간이라 '짝수 쪽으로 반올림' 규칙에 따라 16777216 이 된다.
            //     → float 로 정확히 표현 가능한 정수는 ±2^24 까지 (double 은 ±2^53 까지).
        }
    }
}
#endif