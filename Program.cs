using System.Globalization;
using BudgetTrackerProject.Models;
using BudgetTrackerProject.Services;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-IE");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-IE");

StorageService storageService;
TransactionService transactionService;

try
{
    Directory.CreateDirectory("data");
    Directory.CreateDirectory("logs");

    storageService = new StorageService("data");
    transactionService = new TransactionService(storageService);
    LoggerService loggerService = new LoggerService("logs");
    loggerService.Subscribe(transactionService);
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or StorageException)
{
    Console.WriteLine(ex is StorageException ? ex.Message : $"Could not start the application: {ex.Message}");
    return;
}

bool isRunning = true;

while (isRunning)
{
    try
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
                RemoveTransaction(transactionService);
                break;

            case "3":
                ShowReport(transactionService);
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
    catch (StorageException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Something went wrong: {ex.Message}");
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

static Guid AskForTransactionId()
{
    while (true)
    {
        Console.Write("Enter transaction Id: ");
        string? input = Console.ReadLine();

        bool isValidId = Guid.TryParse(input, out Guid id);

        if (isValidId)
        {
            return id;
        }

        Console.WriteLine("Invalid Id. Please enter a valid transaction Id.");
    }
}

static void RemoveTransaction(TransactionService transactionService)
{
    Guid id = AskForTransactionId();

    bool wasRemoved = transactionService.RemoveTransaction(id);

    if (wasRemoved)
    {
        Console.WriteLine("Transaction removed successfully.");
    }
    else
    {
        Console.WriteLine("No transaction found with that Id.");
    }
}

static void ShowReport(TransactionService transactionService)
{
    DateOnly startDate = AskForDate("Enter start date (yyyy-MM-dd): ");
    DateOnly endDate = AskForDate("Enter end date (yyyy-MM-dd): ");

    if (endDate < startDate)
    {
        Console.WriteLine("End date cannot be before start date.");
        return;
    }

    List<Transaction> transactions = transactionService.GetTransactions(startDate, endDate);
    List<Transaction> filteredTransactions = FilterTransactionsByType(transactions);

    PrintTransactions(filteredTransactions);

    if (filteredTransactions.Count > 0)
    {
        PrintTotals(filteredTransactions);
    }
}

static DateOnly AskForDate(string message)
{
    while (true)
    {
        Console.Write(message);
        string? input = Console.ReadLine();

        bool isValidDate = DateOnly.TryParseExact(
            input,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly date
        );

        if (isValidDate)
        {
            return date;
        }

        Console.WriteLine("Invalid date. Please use yyyy-MM-dd.");
    }
}

static List<Transaction> FilterTransactionsByType(List<Transaction> transactions)
{
    while (true)
    {
        Console.Write("Filter by type? (All/Income/Expense): ");
        string? input = Console.ReadLine();

        if (string.Equals(input, "All", StringComparison.OrdinalIgnoreCase))
        {
            return transactions;
        }

        bool isValidType = Enum.TryParse(input, true, out TransactionType type);
        bool isDefinedType = Enum.IsDefined(type);

        if (isValidType && isDefinedType && !decimal.TryParse(input, out _))
        {
            return transactions
                .Where(transaction => transaction.Type == type)
                .ToList();
        }

        Console.WriteLine("Invalid filter. Please enter All, Income, or Expense.");
    }
}

static void PrintTransactions(List<Transaction> transactions)
{
    Console.WriteLine();
    Console.WriteLine("=== Report ===");

    if (transactions.Count == 0)
    {
        Console.WriteLine("No transactions found.");
        return;
    }

    foreach (Transaction transaction in transactions.OrderBy(transaction => transaction.Timestamp))
    {
        Console.WriteLine($"Id: {transaction.Id}");
        Console.WriteLine($"Date: {transaction.Timestamp:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Type: {transaction.Type}");
        Console.WriteLine($"Description: {transaction.Description}");
        Console.WriteLine($"Amount: {transaction.Amount:C}");
        Console.WriteLine();
    }
}

static void PrintTotals(List<Transaction> transactions)
{
    decimal totalIncome = transactions
        .Where(transaction => transaction.Type == TransactionType.Income)
        .Sum(transaction => transaction.Amount);

    decimal totalExpenses = transactions
        .Where(transaction => transaction.Type == TransactionType.Expense)
        .Sum(transaction => transaction.Amount);

    decimal balance = totalIncome - totalExpenses;

    Console.WriteLine("=== Totals ===");
    Console.WriteLine($"Income: {totalIncome:C}");
    Console.WriteLine($"Expenses: {totalExpenses:C}");
    Console.WriteLine($"Balance: {balance:C}");
}
