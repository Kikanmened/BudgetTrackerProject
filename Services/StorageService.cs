using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetTrackerProject.Models;

namespace BudgetTrackerProject.Services;

public class StorageService
{
    private readonly string _dataDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public StorageService(string dataDirectory)
    {
        _dataDirectory = dataDirectory;

        try
        {
            Directory.CreateDirectory(_dataDirectory);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not create data directory: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied for data directory: {ex.Message}");
        }

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public bool SaveTransaction(Transaction transaction)
    {
        try
        {
            string filePath = GetFilePath(transaction.Timestamp);

            List<Transaction> transactions = LoadTransactionsFromFile(filePath);
            transactions.Add(transaction);

            string json = JsonSerializer.Serialize(transactions, _jsonOptions);
            File.WriteAllText(filePath, json);

            return true;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not save transaction: {ex.Message}");
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied while saving transaction: {ex.Message}");
            return false;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Could not format transaction data: {ex.Message}");
            return false;
        }
    }

    public List<Transaction> LoadTransactions(DateOnly startDate, DateOnly endDate)
    {
        List<Transaction> allTransactions = new List<Transaction>();

        string[] filePaths;

        try
        {
            filePaths = Directory.GetFiles(_dataDirectory, "*.json");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not read the data directory: {ex.Message}");
            return allTransactions;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied while reading the data directory: {ex.Message}");
            return allTransactions;
        }

        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            bool isValidDate = DateOnly.TryParseExact(
                fileName,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly fileDate
            );

            if (isValidDate && fileDate >= startDate && fileDate <= endDate)
            {
                List<Transaction> transactions = LoadTransactionsFromFile(filePath);
                allTransactions.AddRange(transactions);
            }
        }

        return allTransactions;
    }

    public bool RemoveTransaction(Guid id)
    {
        string[] filePaths;

        try
        {
            filePaths = Directory.GetFiles(_dataDirectory, "*.json");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not read the data directory: {ex.Message}");
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access denied while reading the data directory: {ex.Message}");
            return false;
        }

        foreach (string filePath in filePaths)
        {
            List<Transaction> transactions = LoadTransactionsFromFile(filePath);

            Transaction? transactionToRemove = transactions.FirstOrDefault(transaction => transaction.Id == id);

            if (transactionToRemove != null)
            {
                transactions.Remove(transactionToRemove);

                try
                {
                    string json = JsonSerializer.Serialize(transactions, _jsonOptions);
                    File.WriteAllText(filePath, json);

                    return true;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Could not remove transaction: {ex.Message}");
                    return false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Access denied while removing transaction: {ex.Message}");
                    return false;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Could not format transaction data: {ex.Message}");
                    return false;
                }
            }
        }

        return false;
    }

    private string GetFilePath(DateTimeOffset timestamp)
    {
        string fileName = timestamp.ToString("yyyy-MM-dd") + ".json";
        return Path.Combine(_dataDirectory, fileName);
    }

    private List<Transaction> LoadTransactionsFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<Transaction>();
        }

        try
        {
            string json = File.ReadAllText(filePath);
            List<Transaction>? transactions = JsonSerializer.Deserialize<List<Transaction>>(json, _jsonOptions);

            return transactions ?? new List<Transaction>();
        }
        catch (JsonException)
        {
            Console.WriteLine($"Warning: {filePath} contains invalid JSON.");
            return new List<Transaction>();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Could not read {filePath}: {ex.Message}");
            return new List<Transaction>();
        }
    }
}
