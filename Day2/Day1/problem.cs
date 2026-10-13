using System;

namespace Day1
{
    internal class FloatBitsPractice
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
        }

        // 과제 1: 비트 패턴 출력
        static void PrintFloatBits(float value)
        {
            int bits = BitConverter.SingleToInt32Bits(value);
            // TODO: 32자리 2진 문자열로 변환 후 1 / 8 / 23 으로 나누어 출력
            throw new NotImplementedException();
        }

        // 과제 2: 비트 연산으로 필드 분해
        static (int sign, int exponent, int mantissa) Decompose(float value)
        {
            int bits = BitConverter.SingleToInt32Bits(value);
            // TODO: 시프트(>>)와 마스크(&)로 각 필드 추출
            throw new NotImplementedException();
        }

        // 과제 3: 직접 인코딩 (BitConverter 사용 금지)
        static uint EncodeFloat(double value)
        {
            // TODO
            throw new NotImplementedException();
        }

        // 과제 4: 직접 디코딩
        static float DecodeFloat(uint bits)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
