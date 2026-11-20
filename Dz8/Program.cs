using System;
using System.Collections;
using System.IO;

namespace Task9_3
{
    enum AccountType { Current, Savings }

    class BankTransaction
    {
        private readonly DateTime _dateTime;
        private readonly decimal _amount;

        public BankTransaction(decimal amount)
        {
            _dateTime = DateTime.Now;
            _amount = amount;
        }

        public DateTime GetDateTime() { return _dateTime; }
        public decimal GetAmount() { return _amount; }

        public string ToFileString()
        {
            return _dateTime.ToString("dd.MM.yyyy HH:mm:ss") + "   " + _amount;
        }

        public void Print()
        {
            Console.WriteLine(ToFileString());
        }
    }

    class BankAccount : IDisposable
    {
        private static int _lastNumber = 1000;
        private int _number;
        private decimal _balance;
        private AccountType _type;
        private Queue _transactions = new Queue();
        private bool _disposed = false;

        public BankAccount() { _number = GenerateNumber(); _balance = 0; _type = AccountType.Savings; }
        public BankAccount(decimal balance) { _number = GenerateNumber(); _balance = balance; _type = AccountType.Savings; }
        public BankAccount(AccountType type) { _number = GenerateNumber(); _balance = 0; _type = type; }
        public BankAccount(decimal balance, AccountType type) { _number = GenerateNumber(); _balance = balance; _type = type; }

        ~BankAccount()
        {
            Dispose(false);
        }

        private static int GenerateNumber() { _lastNumber++; return _lastNumber; }

        public int GetNumber() { return _number; }
        public decimal GetBalance() { return _balance; }
        public AccountType GetType() { return _type; }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) { Console.WriteLine("Сумма должна быть > 0"); return; }
            _balance += amount;
            _transactions.Enqueue(new BankTransaction(amount));
            Console.WriteLine("Положено " + amount + ". Баланс: " + _balance);
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) { Console.WriteLine("Сумма должна быть > 0"); return; }
            if (amount > _balance) { Console.WriteLine("Недостаточно средств"); return; }
            _balance -= amount;
            _transactions.Enqueue(new BankTransaction(-amount));
            Console.WriteLine("Снято " + amount + ". Баланс: " + _balance);
        }

        public void Print()
        {
            Console.WriteLine("Счет #" + _number + ", тип: " + _type + ", баланс: " + _balance);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_transactions.Count > 0)
                {
                    string fileName = "account_" + _number + ".txt";
                    StreamWriter sw = new StreamWriter(fileName);
                    try
                    {
                        sw.WriteLine("Счет #" + _number + ", тип: " + _type + ", баланс: " + _balance);
                        sw.WriteLine("--- Проводки ---");
                        foreach (BankTransaction t in _transactions)
                            sw.WriteLine(t.ToFileString());
                    }
                    finally
                    {
                        sw.Close();
                    }
                    Console.WriteLine("Данные о проводках сохранены в файл " + fileName);
                }
            }

            _disposed = true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задача 9.3. Метод Dispose сохраняет проводки в файл.");
            Console.WriteLine("Класс реализует IDisposable, в Dispose вызывается GC.SuppressFinalize.");
            Console.WriteLine("Действий не требуется.");
            Console.WriteLine();

            using (BankAccount acc = new BankAccount(1000, AccountType.Savings))
            {
                acc.Print();
                acc.Deposit(500);
                acc.Withdraw(200);
                acc.Deposit(300);
                acc.Withdraw(400);
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}