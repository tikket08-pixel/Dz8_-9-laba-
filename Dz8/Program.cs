using System;
using System.Collections;

namespace Task9_2
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

        public void Print()
        {
            Console.WriteLine(_dateTime.ToString("dd.MM.yyyy HH:mm:ss") + "   " + _amount);
        }
    }

    class BankAccount
    {
        private static int _lastNumber = 1000;
        private int _number;
        private decimal _balance;
        private AccountType _type;
        private Queue _transactions = new Queue();

        public BankAccount() { _number = GenerateNumber(); _balance = 0; _type = AccountType.Savings; }
        public BankAccount(decimal balance) { _number = GenerateNumber(); _balance = balance; _type = AccountType.Savings; }
        public BankAccount(AccountType type) { _number = GenerateNumber(); _balance = 0; _type = type; }
        public BankAccount(decimal balance, AccountType type) { _number = GenerateNumber(); _balance = balance; _type = type; }

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

        public void ShowTransactions()
        {
            Console.WriteLine("Транзакции по счету #" + _number + " (всего " + _transactions.Count + "):");
            foreach (BankTransaction t in _transactions)
                t.Print();
        }

        public void Print()
        {
            Console.WriteLine("Счет #" + _number + ", тип: " + _type + ", баланс: " + _balance);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Задача 9.2. Класс BankTransaction и очередь транзакций.");
            Console.WriteLine("Каждое изменение баланса создаёт BankTransaction с датой и суммой,");
            Console.WriteLine("и складывает его в System.Collections.Queue.");
            Console.WriteLine("Действий не требуется.");
            Console.WriteLine();

            BankAccount acc = new BankAccount(1000, AccountType.Current);
            acc.Print();
            acc.Deposit(500);
            acc.Withdraw(200);
            acc.Deposit(100);
            acc.Withdraw(50);
            Console.WriteLine();
            acc.ShowTransactions();

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}