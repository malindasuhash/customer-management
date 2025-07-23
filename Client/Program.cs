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
    Console.WriteLine("6. Get legal entity");
    Console.WriteLine();
    Console.WriteLine("a. Debug data");
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
          var customerIndex = GetIndex();

            Console.WriteLine("--> Customer emailAddress: ");
            var customerEmail = Console.ReadLine();

            app.UpdateCustomer(customerIndex, customerEmail);
            break;

        case "3":
            var showIndex = GetIndex();
            var customer = app.GetCustomer(showIndex);
            DisplayCustomer(customer);

            Console.ReadKey();

            break;

        case "4":
            var i = GetIndex();
            app.AddLegalEntity(i, new LegalEntityClient { LegalName = "Cambridge Bakery" });
            break;

        case "5":
            var legalEntityIndex = GetIndex("Legal Entity Index");
            Console.WriteLine("--> New Legal Name: ");
            var newLegalName = Console.ReadLine();
            app.UpdateLegalEntity(legalEntityIndex, newLegalName);
            break;

        case "6":
            var legalEntityIndexToDisplay = GetIndex("Legal Entity Index");
            var legalEntity = app.GetLegalEntity(legalEntityIndexToDisplay);
            DisplayLegalEntityDetails(legalEntity);
            Console.ReadKey();

            break;

        case "a":
            app.ShowData();
            break;

        case "x":
            var customerIndexToSubmit = GetIndex();
            app.Submit(customerIndexToSubmit);
            break;
    }

    Console.Clear();

} while (!input.Equals("0", StringComparison.OrdinalIgnoreCase));

void DisplayCustomer(Document<Customer> customer)
{
    if (customer != null)
    {
        Console.WriteLine($"Customer ID: {customer.Id}");
        Console.WriteLine($"Draft({customer.DraftVersion}): {customer.Draft}");
        Console.WriteLine($"Submitted({customer.SubmittedVersion}): {customer.Submitted}");
        Console.WriteLine($"Approved({customer.Approved}): {customer.Approved}");
    }
    else
    {
        Console.WriteLine("No customer details available.");
    }
}

void DisplayLegalEntityDetails(EntityLayout<LegalEntity, LegalEntityClient> legalEntity)
{
    if (legalEntity != null)
    {
        Console.WriteLine($"LegalEntity ID: {legalEntity.Id}");
        Console.WriteLine($"Client Copy: {legalEntity.ClientCopy}");
        Console.WriteLine($"Latest Submitted Copy: {legalEntity.LastestSubmittedCopy}");
        if (legalEntity.WorkingCopy != null)
        {
            Console.WriteLine($"Working Copy: {string.Join("| ", legalEntity.WorkingCopy)}");
        }
        else
        {
            Console.WriteLine("Working Copy:");
        }

        Console.WriteLine($"Ready Copy: {legalEntity.ReadyCopy}");
    }
    else
    {
        Console.WriteLine("No legalEntity details available.");
    }
}

Console.WriteLine("Enter to terminate");
Console.ReadLine();

int GetIndex(string display = "Customer Index")
{
    Console.WriteLine();
    Console.WriteLine("Input needed:");
    Console.WriteLine($"--> {display}: ");
    int index;
    var result = int.TryParse(Console.ReadLine(), out index);

    if (!result || index < 0)
    {
        Console.WriteLine("Invalid index. Please enter a valid non-negative integer.");
        return GetIndex(); // Recursive call to get a valid index
    }

    return index;
}
