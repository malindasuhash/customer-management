// See https://aka.ms/new-console-template for more information
using Client;

Console.WriteLine("Customer Manager Client");


var app = new Application();
app.AddCustomer("e@mail.com");

Console.WriteLine("Enter to terminate");
Console.ReadLine();