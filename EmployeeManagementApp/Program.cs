class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the Employee Management App!");
    
        while (true)
        {
            await Task.Delay(1500);
            Console.WriteLine("Please select an option:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. View Employees");
            Console.WriteLine("3. Update Employee");
            Console.WriteLine("4. Delete Employee");
            int choice = int.TryParse(Console.ReadLine(), out int parsedChoice) ? parsedChoice : 0;

            switch (choice)
            {
                case 1:
                    await Services.AddEmployee();
                    break;
                case 2:
                    await Services.ViewEmployees();
                    break;
                case 3:
                    await Services.UpdateEmployee();
                    break;
                case 4:
                    await Services.DeleteEmployee();
                    break;
                case 0:
                    Console.WriteLine("Exiting the application.");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            if (choice == 0)
            {
                break;
            }
        }
    }
}