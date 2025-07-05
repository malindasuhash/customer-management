// See https://aka.ms/new-console-template for more information
using Client;

Console.WriteLine("--> Customer Manager Client <--");

Thread.Sleep(2000);

var input = new ConsoleKeyInfo();
var app = new Application();
var random = new Random(100);

do
{
    Console.WriteLine("OPTIONS:\n");

    Console.WriteLine("1. Add new customer");
    Console.WriteLine("2. Add new customer and submit");
    Console.WriteLine("3. Update last customer");
    Console.WriteLine("4. Submit last customer");
    Console.WriteLine();
    Console.WriteLine("A. Display data");

    Console.WriteLine("0. Exit");

    input = Console.ReadKey();

    switch (input.Key)
    {
        case ConsoleKey.D1:
            app.AddCustomer("email1@mail.com");
            break;
        case ConsoleKey.D2:
            app.AddCustomer("email2@mail.com", true);
            break;

        case ConsoleKey.A:
            app.ShowData();
            break;
    }

    Console.Clear();

} while (input.Key != ConsoleKey.D0);

Console.WriteLine("Enter to terminate");
Console.ReadLine();