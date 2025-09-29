using System;


namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VendingMachine machine = new VendingMachine();
            string command = "";
            while(command != "stop")
            {
                command = Console.ReadLine();
                machine.ReadCommand(command);
            }
            
        }
    }
}
