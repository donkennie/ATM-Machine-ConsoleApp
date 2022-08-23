// See https://aka.ms/new-console-template for more information
using ATMApp.UI;

APPScreen.Welcome();

long cardNumber = Validator.Convert<long>("your card number");

Console.WriteLine($"Your card number is {cardNumber}");

Utility.PressEnterToContinue();