using System;
using System.Drawing;
using Identicon;


namespace Identicon
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name");
            string input = Console.ReadLine();
            Identicon i = new Identicon(input);
            i.Write(@"./identicon.png");
        }
    }
}
