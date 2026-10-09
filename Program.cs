Directory.CreateDirectory("data");
Directory.CreateDirectory("logs");

bool isRunning = true;

while (isRunning)
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
            Console.WriteLine("Add Transaction selected.");
            break;

        case "2":
            Console.WriteLine("Remove Transaction selected.");
            break;

        case "3":
            Console.WriteLine("View Report selected.");
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
