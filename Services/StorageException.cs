namespace BudgetTrackerProject.Services;

public class StorageException : Exception
{
    public StorageException(string message)
        : base(message)
    {
    }

    public StorageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class InvalidTransactionFileException : StorageException
{
    public InvalidTransactionFileException(string filePath)
        : base($"Warning: {filePath} contains invalid JSON.")
    {
    }
}
