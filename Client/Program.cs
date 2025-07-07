// See https://aka.ms/new-console-template for more information
using Client;
using Models;
using Models.Infrastructure;

Thread.Sleep(2000);

var input = new ConsoleKeyInfo();
var app = new Application();

do
{
    Console.WriteLine("--> Customer Manager Client <--\n");

    Console.WriteLine("OPTIONS:\n");

    Console.WriteLine("1. Add new customer");
    Console.WriteLine("2. Add new customer and submit");
    Console.WriteLine("3. Update customer customer");
    Console.WriteLine("4. Submit customer");
    Console.WriteLine("5. Get customer");
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

        case ConsoleKey.D3:
          var customerIndex = GetIndex();

            Console.WriteLine("--> Customer emailAddress: ");
            var customerEmail = Console.ReadLine();

            app.UpdateCustomer(customerIndex, customerEmail);
            break;

        case ConsoleKey.D4:
            var index = GetIndex();

            app.UpdateCustomer(index);

            break;

        case ConsoleKey.D5:
            var showIndex = GetIndex();
            var customer = app.GetCustomer(showIndex);
            DisplayDetails(customer);

            Console.ReadKey();

            break;
        case ConsoleKey.A:
            app.ShowData();
            break;
    }

    Console.Clear();

} while (input.Key != ConsoleKey.D0);

void DisplayDetails(EntityLayout<Customer, CustomerClient> customer)
{
    if (customer != null)
    {
        Console.WriteLine($"Customer ID: {customer.Id}");
        Console.WriteLine($"Client Copy: {customer.ClientCopy}");
        Console.WriteLine($"Latest Submitted Copy: {customer.LastestSubmittedCopy}");
        if (customer.WorkingCopy != null)
        {
            Console.WriteLine($"Working Copy: {string.Join("| ", customer.WorkingCopy)}");
        }
        else
        {
            Console.WriteLine("Working Copy:");
        }

        Console.WriteLine($"Ready Copy: {customer.ReadyCopy}");
    }
    else
    {
        Console.WriteLine("No customer details available.");
    }
}

Console.WriteLine("Enter to terminate");
Console.ReadLine();

int GetIndex()
{
    Console.WriteLine();
    Console.WriteLine("Input needed:");
    Console.WriteLine("--> Customer Index: ");
    int index;
    var result = int.TryParse(Console.ReadLine(), out index);

    if (!result || index < 0)
    {
        Console.WriteLine("Invalid index. Please enter a valid non-negative integer.");
        return GetIndex(); // Recursive call to get a valid index
    }

    return index;
}
