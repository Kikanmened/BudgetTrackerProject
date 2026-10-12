using BudgetTrackerProject.Events;
using BudgetTrackerProject.Models;

namespace BudgetTrackerProject.Services;

public class TransactionService
{
    private readonly StorageService _storageService;

    public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

    public TransactionService(StorageService storageService)
    {
        _storageService = storageService;
    }

    public Transaction AddTransaction(TransactionType type, string description, decimal amount)
    {
        Transaction transaction = new Transaction(
            Guid.NewGuid(),
            DateTimeOffset.Now,
            type,
            description,
            amount
        );

        _storageService.SaveTransaction(transaction);
        OnTransactionAdded(transaction);

        return transaction;
    }

    public bool RemoveTransaction(Guid id)
    {
        return _storageService.RemoveTransaction(id);
    }

    public List<Transaction> GetTransactions(DateOnly startDate, DateOnly endDate)
    {
        return _storageService.LoadTransactions(startDate, endDate);
    }

    private void OnTransactionAdded(Transaction transaction)
    {
        TransactionAdded?.Invoke(this, new TransactionAddedEventArgs(transaction));
    }
}