# Budget Tracker

This app touches almost everything learned in the C# lessons.

As we build it, we recap each topic:

| Building block | C# topic |
| --- | --- |
| Transaction model | records, properties, and enums |
| Main menu | methods, conditionals, loops, and `switch` |
| Add and remove flows | validation, `Guid`, and service methods |
| Save and load | file I/O and JSON serialisation |
| Reports | LINQ, filtering, ordering, and totals |
| Notifications | events and `EventArgs` |
| Safety | exceptions and `TryParse` methods |
| Project structure | namespaces and separation of responsibilities |

## What This Project Recaps

The Budget Tracker brings together the following Backend lessons:

- Top-level statements for starting a console application.
- Records and enums for representing transaction data.
- Collections for storing multiple transactions.
- Methods, loops, and `switch` statements for the menu flow.
- File I/O and JSON serialisation for permanent storage.
- LINQ for reports, filtering, ordering, and totals.
- Events for notifying the logger when a transaction is added.
- Exceptions and input validation for safe application behavior.

## Project Responsibilities

The project is split into small responsibilities:

```text
BudgetTrackerProject/
├── Program.cs    <- starts the app and handles user interaction
├── Models/
│   ├── Transaction.cs       <- transaction data
│   └── TransactionType.cs   <- Income and Expense enum
├── Events/
│   └── TransactionAddedEventArgs.cs <- event data
└── Services/
    ├── StorageService.cs    <- all file and JSON work
    ├── TransactionService.cs <- transaction operations and events
    └── LoggerService.cs     <- transaction log writing
```

Keeping these responsibilities separate makes each part easier to understand, test, and change.

## Requirement And Lesson Map

| Requirement | What it provides | C# lesson or topic |
| --- | --- | --- |
| FR001 | Console project bootstrap | top-level statements |
| FR002 | Repeating main menu | loops, methods, and `switch` |
| FR003 | Add a transaction | enums, `TryParse`, `Guid`, and methods |
| FR004 | Daily JSON storage | file I/O and JSON serialisation |
| FR005 | Remove by ID | `Guid.TryParse`, loops, and collections |
| FR006 | Reports and totals | LINQ, filtering, ordering, and `Sum` |
| FR007 | Safe error handling | exceptions and validation |
| FR008 | Added transaction event | events and event handlers |
| FR009 | Create application folders | directory and file APIs |
| FR010 | Project documentation | README and technical communication |
| FR011 | Organised namespaces | project structure and separation of concerns |
| FR012 | Transaction models | records, properties, and enums |
| FR013 | Event argument class | inheritance and `EventArgs` |
| FR014 | Storage service | classes, constructors, JSON, and file I/O |
| FR015 | Transaction service | classes, methods, and events |
| FR016 | Logger service | file writing and event subscriptions |

## Requirements

- .NET 10 SDK
- A terminal or code editor

## Run the application

From the project directory, run:

```bash
dotnet build
dotnet run
```

## Features

- Add income or expense transactions.
- Remove a transaction by its ID.
- View reports for a date range.
- Filter reports by income, expense, or all transactions.
- Calculate total income, total expenses, and balance.
- Save transactions as daily JSON files.
- Write successful additions to a transaction log.
- Handle invalid input and file errors without terminating the application.

## Storage

Transactions are saved in the `data` directory using the filename format:

```text
yyyy-MM-dd.json
```

Log entries are saved in:

```text
logs/transactions.log
```

The `data` and `logs` directories are created automatically when the application starts.

## Project structure

```text
BudgetTrackerProject/
├── Events/       Event argument classes
├── Models/       Transaction model and transaction type enum
├── Services/     Storage, transaction, and logger services
├── data/         Daily transaction JSON files
├── logs/         Application log files
├── Program.cs    Console menu and user input
└── README.md     Project documentation
```

## Git workflow

The project uses the following branch workflow:

```text
feature branch -> dev -> main
```

Create a feature branch from the latest `dev` branch:

```bash
git checkout dev
git pull origin dev
git checkout -b feature/feature-name
```

After testing the feature, commit and push the feature branch, then open a pull request into `dev`.

## Final C# Concept Recap

| Topic | Where it appears |
| --- | --- |
| Top-level statements | `Program.cs` |
| Records | `Models/Transaction.cs` |
| Enums | `Models/TransactionType.cs` |
| Collections | `List<Transaction>` in the services and reports |
| Methods and loops | menu and input methods in `Program.cs` |
| `switch` | main menu selection in `Program.cs` |
| File I/O | `StorageService` and `LoggerService` |
| JSON serialisation | `StorageService` using `System.Text.Json` |
| LINQ | report filtering, ordering, and totals |
| Events | `TransactionService` and `TransactionAddedEventArgs` |
| Exceptions | storage and logging error handling |
| Clean structure | `Models`, `Events`, and `Services` folders |
