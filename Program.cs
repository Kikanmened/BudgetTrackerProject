using BudgetTrackerProject.Models;
using BudgetTrackerProject.Services;
Directory.CreateDirectory("data");
Directory.CreateDirectory("logs");

StorageService storageService = new StorageService("data");
TransactionService transactionService = new TransactionService(storageService);
LoggerService loggerService = new LoggerService("logs");
loggerService.Subscribe(transactionService);

bool isRunning = true;

while (isRunning)
{
    Console.WriteLine();
    Console.WriteLine("=== Budget Tracker ===");
    Console.WriteLine("1. Add Transaction");
    Console.WriteLine("2. Remove Transaction");
    Console.WriteLine("3. View Report");
    Console.WriteLine("4. Exit");
    Console.Write("Choose an option: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddTransaction(transactionService);
            break;

        case "2":
            Console.WriteLine("Remove Transaction selected.");
            break;

        case "3":
            Console.WriteLine("View Report selected.");
            break;

        case "4":
            Console.WriteLine("Goodbye!");
            isRunning = false;
            break;

        default:
            Console.WriteLine("Invalid option. Please choose 1, 2, 3, or 4.");
            break;
    }
}

static void AddTransaction(TransactionService transactionService)
{
    TransactionType type = AskForTransactionType();
    string description = AskForDescription();
    decimal amount = AskForAmount();

    Transaction transaction = transactionService.AddTransaction(type, description, amount);

    Console.WriteLine();
    Console.WriteLine("Transaction added successfully.");
    Console.WriteLine($"Id: {transaction.Id}");
}

static TransactionType AskForTransactionType()
{
    while (true)
    {
        Console.Write("Enter transaction type (Income/Expense): ");
        string? input = Console.ReadLine();

        bool isValidType = Enum.TryParse(input, true, out TransactionType type);
        bool isDefinedType = Enum.IsDefined(type);

        if (isValidType && isDefinedType && !decimal.TryParse(input, out _))
        {
            return type;
        }

        Console.WriteLine("Invalid type. Please enter Income or Expense.");
    }
}

static string AskForDescription()
{
    while (true)
    {
        Console.Write("Enter description: ");
        string? description = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(description))
        {
            return description;
        }

        Console.WriteLine("Description cannot be empty.");
    }
}

static decimal AskForAmount()
{
    while (true)
    {
        Console.Write("Enter amount: ");
        string? input = Console.ReadLine();

        bool isValidAmount = decimal.TryParse(input, out decimal amount);

        if (isValidAmount && amount > 0)
        {
            return amount;
        }

        Console.WriteLine("Invalid amount. Please enter a positive number.");
    }
}
