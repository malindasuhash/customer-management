// See https://aka.ms/new-console-template for more information
using EventViewer;

Console.WriteLine("--> EVENT Receiver <--");
Console.BackgroundColor = ConsoleColor.DarkGray;

var receiver = new Receiver();
receiver.Receive();

