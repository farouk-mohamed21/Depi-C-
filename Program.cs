using System;
using System.Collections.Generic;

namespace IntegratedBankSystem
{
	public class Transaction
	{
		public string Type { get; set; }
		public double Amount { get; set; }
		public DateTime Date { get; set; }

		public Transaction(string type, double amount)
		{
			Type = type;
			Amount = amount;
			Date = DateTime.Now;
		}
	}

	public abstract class BankAccount
	{
		private static int _nextAccNum = 1000;

		public int AccountNumber { get; private set; }
		public double Balance { get; protected set; } 
		public List<Transaction> History { get; set; }

		public BankAccount(double initialBalance)
		{
			AccountNumber = _nextAccNum;
			_nextAccNum++;

			if (initialBalance >= 0) Balance = initialBalance;
			else Balance = 0;

			History = new List<Transaction>();
			History.Add(new Transaction("Opening Balance", Balance));
		}

		public virtual void Deposit(double amount)
		{
			if (amount > 0)
			{
				Balance += amount;
				History.Add(new Transaction("Deposit", amount));
				Console.WriteLine("Deposit Successful.");
			}
			else Console.WriteLine("Invalid Amount.");
		}

		public virtual bool Withdraw(double amount)
		{
			if (Balance >= amount)
			{
				Balance -= amount;
				History.Add(new Transaction("Withdraw", amount));
				Console.WriteLine("Withdraw Successful.");
				return true;
			}
			else
			{
				Console.WriteLine("Error: Insufficient Funds.");
				return false;
			}
		}

		public virtual double CalculateInterest()
		{
			return 0;
		}
	}


	public class SavingsAccount : BankAccount
	{
		public double InterestRate { get; set; }

		public SavingsAccount(double balance, double rate) : base(balance)
		{
			InterestRate = rate;
		}

		public override double CalculateInterest()
		{
			return Balance * (InterestRate / 100);
		}
	}

	public class CurrentAccount : BankAccount
	{
		public double OverdraftLimit { get; set; }

		public CurrentAccount(double balance, double limit) : base(balance)
		{
			OverdraftLimit = limit;
		}

		public override bool Withdraw(double amount)
		{
			if ((Balance + OverdraftLimit) >= amount)
			{
				Balance -= amount;
				History.Add(new Transaction("Withdraw", amount));
				Console.WriteLine("Withdraw Successful (Overdraft Used).");
				return true;
			}
			else
			{
				Console.WriteLine("Error: Exceeded Overdraft Limit.");
				return false;
			}
		}
	}

	public class Customer
	{
		private static int _nextCustId = 1;
		public int ID { get; private set; }
		public string Name { get; set; }
		public string NationalID { get; set; }

		public List<BankAccount> Accounts { get; set; }

		public Customer(string name, string nid)
		{
			ID = _nextCustId;
			_nextCustId++;
			Name = name;
			NationalID = nid;
			Accounts = new List<BankAccount>();
		}
	}

	class Program
	{
		static List<Customer> AllCustomers = new List<Customer>();

		static void Main(string[] args)
		{
			Console.WriteLine("=== Welcome to FCI Bank System ===");

			while (true)
			{
				Console.WriteLine("\n--- Main Menu ---");
				Console.WriteLine("1. Add New Customer");
				Console.WriteLine("2. Open New Account");
				Console.WriteLine("3. Deposit / Withdraw");
				Console.WriteLine("4. Show Full Report");
				Console.WriteLine("5. Exit");
				Console.Write("Choose (1-5): ");
				string choice = Console.ReadLine();

				if (choice == "1") AddCustomer();
				else if (choice == "2") OpenAccount();
				else if (choice == "3") DoTransaction();
				else if (choice == "4") ShowReport();
				else if (choice == "5") break;
				else Console.WriteLine("Invalid choice!");
			}
		}


		static void AddCustomer()
		{
			Console.Write("Enter Name: ");
			string name = Console.ReadLine();
			Console.Write("Enter National ID: ");
			string nid = Console.ReadLine();

			Customer c = new Customer(name, nid);
			AllCustomers.Add(c);
			Console.WriteLine($"Customer Added! Your ID is: {c.ID}");
		}

		static void OpenAccount()
		{
			Console.Write("Enter Customer ID: ");
			int id = int.Parse(Console.ReadLine());

			// بحث عن العميل
			Customer c = FindCustomer(id);
			if (c == null) return;

			Console.WriteLine("Account Type: 1. Savings   2. Current");
			string type = Console.ReadLine();

			Console.Write("Initial Balance: ");
			double bal = double.Parse(Console.ReadLine());

			if (type == "1")
			{
				c.Accounts.Add(new SavingsAccount(bal, 10)); 
				Console.WriteLine("Savings Account Created.");
			}
			else if (type == "2")
			{
				c.Accounts.Add(new CurrentAccount(bal, 1000)); 
				Console.WriteLine("Current Account Created.");
			}
		}

		static void DoTransaction()
		{
			Console.Write("Enter Customer ID: ");
			int id = int.Parse(Console.ReadLine());
			Customer c = FindCustomer(id);
			if (c == null) return;

			if (c.Accounts.Count == 0)
			{
				Console.WriteLine("No accounts found.");
				return;
			}

			BankAccount acc = c.Accounts[0];
			Console.WriteLine($"Current Balance: {acc.Balance}");

			Console.WriteLine("1. Deposit   2. Withdraw");
			string op = Console.ReadLine();
			Console.Write("Amount: ");
			double amount = double.Parse(Console.ReadLine());

			if (op == "1") acc.Deposit(amount);
			else if (op == "2") acc.Withdraw(amount);
		}

		static void ShowReport()
		{
			Console.WriteLine("\n=== BANK REPORT ===");
			foreach (Customer c in AllCustomers)
			{
				Console.WriteLine($"ID: {c.ID} | Name: {c.Name}");
				foreach (BankAccount acc in c.Accounts)
				{
					double interest = acc.CalculateInterest();

					Console.WriteLine($"   - Acc#: {acc.AccountNumber} | Bal: {acc.Balance} | Interest: {interest}");

					foreach (var t in acc.History)
					{
						Console.WriteLine($"     * {t.Date} - {t.Type}: {t.Amount}");
					}
				}
				Console.WriteLine("-------------------");
			}
		}

		static Customer FindCustomer(int id)
		{
			foreach (Customer c in AllCustomers)
			{
				if (c.ID == id) return c;
			}
			Console.WriteLine("Customer Not Found!");
			return null;
		}
	}
}