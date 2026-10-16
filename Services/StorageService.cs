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
            throw new StorageException($"Could not create data directory: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Access denied for data directory: {ex.Message}", ex);
        }

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
        WriteFile(filePath, json);
    }

    public List<Transaction> LoadTransactions(DateOnly startDate, DateOnly endDate)
    {
        return GetDataFiles()
            .Where(filePath => IsDayFileInRange(filePath, startDate, endDate))
            .SelectMany(ReadDayFile)
            .ToList();
    }

    public bool RemoveTransaction(Guid id)
    {
        foreach (string filePath in GetDataFiles())
        {
            List<Transaction> transactions;

            try
            {
                transactions = LoadTransactionsFromFile(filePath);
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }

            Transaction? transactionToRemove = transactions.FirstOrDefault(transaction => transaction.Id == id);

            if (transactionToRemove != null)
            {
                transactions.Remove(transactionToRemove);
                string json = JsonSerializer.Serialize(transactions, _jsonOptions);
                WriteFile(filePath, json);
                return true;
            }
        }

        return false;
    }

    private string[] GetDataFiles()
    {
        try
        {
            return Directory.GetFiles(_dataDirectory, "*.json");
        }
        catch (IOException ex)
        {
            throw new StorageException($"Could not read the data directory: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Access denied while reading the data directory: {ex.Message}", ex);
        }
    }

    private bool IsDayFileInRange(string filePath, DateOnly startDate, DateOnly endDate)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        bool isValidDate = DateOnly.TryParseExact(
            fileName,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly fileDate
        );

        return isValidDate && fileDate >= startDate && fileDate <= endDate;
    }

    private IEnumerable<Transaction> ReadDayFile(string filePath)
    {
        try
        {
            return LoadTransactionsFromFile(filePath);
        }
        catch (StorageException ex)
        {
            Console.WriteLine(ex.Message);
            return new List<Transaction>();
        }
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

        string json = ReadFile(filePath);

        try
        {
            List<Transaction>? transactions = JsonSerializer.Deserialize<List<Transaction>>(json, _jsonOptions);
            return transactions ?? new List<Transaction>();
        }
        catch (JsonException)
        {
            throw new InvalidTransactionFileException(filePath);
        }
    }

    private string ReadFile(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (IOException ex)
        {
            throw new StorageException($"Could not read {filePath}: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Access denied while reading {filePath}: {ex.Message}", ex);
        }
    }

    private void WriteFile(string filePath, string json)
    {
        try
        {
            File.WriteAllText(filePath, json);
        }
        catch (IOException ex)
        {
            throw new StorageException($"Could not write {filePath}: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new StorageException($"Access denied while writing {filePath}: {ex.Message}", ex);
        }
    }
}
