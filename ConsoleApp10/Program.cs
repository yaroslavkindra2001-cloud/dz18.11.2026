
using System;

class CreditCard
{
    public string Number { get; set; }
    public string Owner { get; set; }
    public string ExpirationDate { get; set; }
    private string pin;
    public decimal CreditLimit { get; set; }
    public decimal Balance { get; private set; }

    public event Action<decimal> AccountReplenished;
    public event Action<decimal> MoneySpent;
    public event Action CreditMoneyStarted;
    public event Action<decimal> TargetAmountReached;
    public event Action PinChanged;

    public CreditCard(string number, string owner, string expirationDate,
        string pin, decimal creditLimit, decimal balance)
    {
        Number = number;
        Owner = owner;
        ExpirationDate = expirationDate;
        this.pin = pin;
        CreditLimit = creditLimit;
        Balance = balance;
    }

    public void Replenish(decimal amount)
    {
        if (amount <= 0)
            return;

        Balance += amount;
        AccountReplenished?.Invoke(amount);
    }

    public void Spend(decimal amount)
    {
        if (amount <= 0)
            return;

        if (Balance - amount < -CreditLimit)
        {
            Console.WriteLine("Недостатньо коштів!");
            return;
        }

        if (Balance >= 0 && Balance - amount < 0)
            CreditMoneyStarted?.Invoke();

        Balance -= amount;
        MoneySpent?.Invoke(amount);
    }

    public void CheckTargetAmount(decimal target)
    {
        if (Balance >= target)
            TargetAmountReached?.Invoke(target);
    }

    public void ChangePin(string oldPin, string newPin)
    {
        if (pin == oldPin && newPin.Length == 4)
        {
            pin = newPin;
            PinChanged?.Invoke();
        }
        else
        {
            Console.WriteLine("Невірний PIN!");
        }
    }

    public void ShowInfo()
    {
        Console.WriteLine("Власник: " + Owner);
        Console.WriteLine("Номер картки: " + Number);
        Console.WriteLine("Термін дії: " + ExpirationDate);
        Console.WriteLine("Баланс: " + Balance);
        Console.WriteLine("Кредитний ліміт: " + CreditLimit);
    }
}

class Program
{
    static void Main()
    {
        CreditCard card = new CreditCard(
            "1234567890123456",
            "Ivan Ivanov",
            "12/28",
            "1234",
            5000,
            2000
        );

        card.AccountReplenished += amount =>
            Console.WriteLine("Рахунок поповнено на " + amount);

        card.MoneySpent += amount =>
            Console.WriteLine("Витрачено " + amount);

        card.CreditMoneyStarted += () =>
            Console.WriteLine("Почалося використання кредитних коштів!");

        card.TargetAmountReached += amount =>
            Console.WriteLine("Досягнуто заданої суми: " + amount);

        card.PinChanged += () =>
            Console.WriteLine("PIN успішно змінено!");

        card.ShowInfo();

        Console.WriteLine();

        card.Replenish(1000);
        card.Spend(3500);
        card.CheckTargetAmount(1000);
        card.ChangePin("1234", "5678");

        Console.WriteLine();
        card.ShowInfo();
    }
}
