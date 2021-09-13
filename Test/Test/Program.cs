using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Beep(500, 500);
                Console.Beep(1000, 500);
                Console.Beep(500, 500);
                Console.Beep(300, 500);
            }
        }
    }
}
