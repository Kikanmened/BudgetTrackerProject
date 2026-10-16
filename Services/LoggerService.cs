using BudgetTrackerProject.Events;

namespace BudgetTrackerProject.Services;

public class LoggerService
{
    private readonly string _logFilePath;

    public LoggerService(string logDirectory)
    {
        try
        {
            Directory.CreateDirectory(logDirectory);
        }
        catch (IOException ex)
        {
            throw new StorageException($"Could not create log directory: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Access denied for log directory: {ex.Message}", ex);
        }

        _logFilePath = Path.Combine(logDirectory, "transactions.log");
    }

    public void Subscribe(TransactionService transactionService)
    {
        transactionService.TransactionAdded += OnTransactionAdded;
    }

    private void OnTransactionAdded(object? sender, TransactionAddedEventArgs e)
    {
        string logLine = $"{DateTimeOffset.Now:u} Added {e.Transaction.Type}: {e.Transaction.Description} - {e.Transaction.Amount:C}";

        try
        {
            WriteLog(logLine);
        }
        catch (StorageException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void WriteLog(string logLine)
    {
        try
        {
            File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
        }
        catch (IOException ex)
        {
            throw new StorageException($"Transaction was saved, but it could not be logged: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Transaction was saved, but the log file could not be written: {ex.Message}", ex);
        }
    }
}