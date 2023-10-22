using System;
using System.Threading;

class Account
{
    private decimal balance;
    private object balanceLock = new object(); // Объект-блокировка для обеспечения потокобезопасности

    public Account(decimal initialBalance)
    {
        this.balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        lock (balanceLock) // Блокировка доступа к полю balance
        {
            balance += amount;
            Console.WriteLine($"Пополнение на сумму {amount}. Баланс: {balance}");
        }
    }

    public void Withdraw(decimal amount)
    {
        lock (balanceLock) // Блокировка доступа к полю balance
        {
            // Если на балансе достаточно средств, производим снятие
            if (balance >= amount)
            {
                balance -= amount;
                Console.WriteLine($"Снятие на сумму {amount}. Баланс: {balance}");
            }
            else
            {
                Console.WriteLine("Недостаточно средств для снятия.");
            }
        }
    }

    public decimal GetBalance()
    {
        lock (balanceLock) // Блокировка доступа к полю balance
        {
            return balance;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var account = new Account(1000);

        // Создание потока для пополнения счета
        Thread depositThread = new Thread(() =>
        {
            Random rnd = new Random();

            while (true)
            {
                decimal amount = rnd.Next(100, 1000); // Случайная сумма пополнения
                account.Deposit(amount);
                Thread.Sleep(1000); // Задержка для имитации произвольного пополнения
            }
        });
        depositThread.Start();

        decimal withdrawalAmount = 2000; // Сумма для снятия

        // Ожидание пополнения счета до требуемой суммы для снятия
        while (account.GetBalance() < withdrawalAmount)
        {
            Thread.Sleep(100); // Задержка перед следующей проверкой
        }

        // Снятие денег со счета
        account.Withdraw(withdrawalAmount);

        // Вывод остатка на балансе
        Console.WriteLine($"Остаток на балансе: {account.GetBalance()}");

        depositThread.Join(); // Ожидание завершения потока пополнения
    }
}