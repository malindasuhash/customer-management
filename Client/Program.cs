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
    Console.WriteLine("2. Add new customer and submit");
    Console.WriteLine("3. Update customer customer");
    Console.WriteLine("4. Submit customer");
    Console.WriteLine("5. Get customer");
    Console.WriteLine();
    Console.WriteLine("6. Add new legal entity (attach to latest customer)");
    Console.WriteLine("7. Add new legal entity and submit (attach to latest customer)");
    Console.WriteLine("8. Update legal entity");
    Console.WriteLine("9. Submit legal entity");
    Console.WriteLine();
    Console.WriteLine("A. Display data");

    Console.WriteLine("0. Exit");

    Console.WriteLine("Choice:"); input = Console.ReadLine();

    switch (input)
    {
        case "1":
            app.AddCustomer("email1@mail.com");
            break;

        case "2":
            app.AddCustomer("email2@mail.com", true);
            break;

        case "3":
          var customerIndex = GetIndex();

            Console.WriteLine("--> Customer emailAddress: ");
            var customerEmail = Console.ReadLine();

            app.UpdateCustomer(customerIndex, customerEmail);
            break;

        case "4":
            var index = GetIndex();
            app.UpdateCustomer(index);

            break;

        case "5":
            var showIndex = GetIndex();
            var customer = app.GetCustomer(showIndex);
            DisplayDetails(customer);

            Console.ReadKey();

            break;

        case "6":
            var i = GetIndex();
            app.AddLegalEntity(i, new LegalEntityClient { LegalName = "Cambridge Bakery" });
            break;

        case "a":
            app.ShowData();
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
