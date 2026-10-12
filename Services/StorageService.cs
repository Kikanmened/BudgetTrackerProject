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
        Directory.CreateDirectory(_dataDirectory);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public void SaveTransaction(Transaction transaction)
    {
        string filePath = GetFilePath(transaction.Timestamp);

        List<Transaction> transactions = LoadTransactionsFromFile(filePath);
        transactions.Add(transaction);

        string json = JsonSerializer.Serialize(transactions, _jsonOptions);
        File.WriteAllText(filePath, json);
    }

    public List<Transaction> LoadTransactions(DateOnly startDate, DateOnly endDate)
    {
        List<Transaction> allTransactions = new List<Transaction>();

        foreach (string filePath in Directory.GetFiles(_dataDirectory, "*.json"))
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
        foreach (string filePath in Directory.GetFiles(_dataDirectory, "*.json"))
        {
            List<Transaction> transactions = LoadTransactionsFromFile(filePath);

            Transaction? transactionToRemove = transactions.FirstOrDefault(transaction => transaction.Id == id);

            if (transactionToRemove != null)
            {
                transactions.Remove(transactionToRemove);

                string json = JsonSerializer.Serialize(transactions, _jsonOptions);
                File.WriteAllText(filePath, json);

                return true;
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