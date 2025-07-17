// See https://aka.ms/new-console-template for more information
using Client;
using Models;
using Models.Infrastructure;
using System;

Thread.Sleep(2000);

string input = "0";
var app = new Application();

do
{
    Console.WriteLine("--> Customer Manager Client <--\n");

    Console.WriteLine("OPTIONS:\n");

    Console.WriteLine("1. Add new customer");
    Console.WriteLine("2. Update customer customer");
    Console.WriteLine("3. Get customer");
    Console.WriteLine();
    Console.WriteLine("4. Add new legal entity");
    Console.WriteLine("5. Update legal entity");
    Console.WriteLine("6. [TODO] Get legal entity");
    Console.WriteLine();
    Console.WriteLine("a. Display data");
    Console.WriteLine("x. Submit!");
    Console.WriteLine();
    Console.WriteLine("0. Exit");

    Console.WriteLine("Choice:"); input = Console.ReadLine();

    switch (input)
    {
        case "1":
            app.AddCustomer("email1@mail.com");
            break;

        case "2":
          var customerIndex = GetCustomerIndex();

            Console.WriteLine("--> Customer emailAddress: ");
            var customerEmail = Console.ReadLine();

            app.UpdateCustomer(customerIndex, customerEmail);
            break;

        case "3":
            var showIndex = GetCustomerIndex();
            var customer = app.GetCustomer(showIndex);
            DisplayDetails(customer);

            Console.ReadKey();

            break;

        case "4":
            var i = GetCustomerIndex();
            app.AddLegalEntity(i, new LegalEntityClient { LegalName = "Cambridge Bakery" });
            break;

        case "a":
            app.ShowData();
            break;

        case "x":
            var customerIndexToSubmit = GetCustomerIndex();
            app.Submit(customerIndexToSubmit);
            break;
    }

    Console.Clear();

} while (!input.Equals("0", StringComparison.OrdinalIgnoreCase));

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

int GetCustomerIndex()
{
    Console.WriteLine();
    Console.WriteLine("Input needed:");
    Console.WriteLine("--> Customer Index: ");
    int index;
    var result = int.TryParse(Console.ReadLine(), out index);

    if (!result || index < 0)
    {
        Console.WriteLine("Invalid index. Please enter a valid non-negative integer.");
        return GetCustomerIndex(); // Recursive call to get a valid index
    }

    return index;
}
