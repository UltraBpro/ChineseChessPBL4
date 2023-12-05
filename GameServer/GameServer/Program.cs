using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Nhap IP de host, mac dinh la 127.0.0.1 neu khong nhap gi(khong nhap Port, mac dinh la 1006): ");
            Server.Chay(Console.ReadLine());
            while (true) if (Console.ReadKey().Key == ConsoleKey.C) Console.Clear();
        }
    }
}