using BudgetTrackerProject.Models;

namespace BudgetTrackerProject.Events;

public class TransactionAddedEventArgs : EventArgs
{
    public Transaction Transaction { get; }

    public TransactionAddedEventArgs(Transaction transaction)
    {
        Transaction = transaction;
    }
}