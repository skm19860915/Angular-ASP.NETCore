using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new DAL.Repositories.UserRepo().Login("1", "2");
            Console.WriteLine("setve");
        }
    }
}
