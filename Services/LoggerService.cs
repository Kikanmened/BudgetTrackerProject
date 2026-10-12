using BudgetTrackerProject.Events;

namespace BudgetTrackerProject.Services;

public class LoggerService
{
    private readonly string _logFilePath;

    public LoggerService(string logDirectory)
    {
        Directory.CreateDirectory(logDirectory);
        _logFilePath = Path.Combine(logDirectory, "transactions.log");
    }

    public void Subscribe(TransactionService transactionService)
    {
        transactionService.TransactionAdded += OnTransactionAdded;
    }

    private void OnTransactionAdded(object? sender, TransactionAddedEventArgs e)
    {
        string logLine = $"{DateTimeOffset.Now:u} Added {e.Transaction.Type}: {e.Transaction.Description} - {e.Transaction.Amount:C}";
        File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
    }
}