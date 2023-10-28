using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Chay(Console.ReadLine());
            while(true)if(Console.ReadKey().Key==ConsoleKey.C)Console.Clear();
        }
    }
}
