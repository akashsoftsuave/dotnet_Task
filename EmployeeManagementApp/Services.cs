class Services
{
    public static readonly List<Employee> Employees = new();
    public static async Task AddEmployee()
    {
        Console.WriteLine("Adding a new employee...");
        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Email: ");
        string email = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Phone Number: ");
        string phoneNumber = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Job Title: ");
        string jobTitle = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Department: ");
        string department = Console.ReadLine() ?? string.Empty;
        Console.Write("Enter Salary: ");
        decimal salary = decimal.TryParse(Console.ReadLine(), out decimal parsedSalary) ? parsedSalary : 0;

        Employees.Add(new Employee
        {
            EmployeeId = Employees.Count + 1,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            JobTitle = jobTitle,
            Department = department,
            Salary = salary
        });
        await Task.Delay(1500);
        Console.WriteLine("Employee added successfully.");
    }

    public static async Task ViewEmployees()
    {
        Console.WriteLine("Viewing employees...");
        foreach (var employee in Employees)
        {
            Console.WriteLine($"ID: {employee.EmployeeId}, Name: {employee.FirstName} {employee.LastName}, Email: {employee.Email}, Phone: {employee.PhoneNumber}, Job Title: {employee.JobTitle}, Department: {employee.Department}, Salary: {employee.Salary}");
        }
        await Task.Delay(1500);
        Console.WriteLine("Finished viewing employees.");
    }

    public static async Task UpdateEmployee()
    {
        Console.WriteLine("Updating an employee...");
        Console.Write("Enter Employee ID to update: ");
        int employeeId = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;
        var employee = Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        if (employee != null)
        {
            Console.Write("Enter new First Name: ");
            employee.FirstName = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Last Name: ");
            employee.LastName = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Email: ");
            employee.Email = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Phone Number: ");
            employee.PhoneNumber = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Job Title: ");
            employee.JobTitle = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Department: ");
            employee.Department = Console.ReadLine() ?? string.Empty;
            Console.Write("Enter new Salary: ");
            employee.Salary = decimal.TryParse(Console.ReadLine(), out decimal parsedSalary) ? parsedSalary : 0;
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
        await Task.Delay(1500);
        Console.WriteLine("Finished updating employee.");
    }

    public static async Task DeleteEmployee()
    {
        Console.WriteLine("Deleting an employee...");
        Console.Write("Enter Employee ID to delete: ");
        int employeeId = int.TryParse(Console.ReadLine(), out int parsedId) ? parsedId : 0;
        var employee = Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        if (employee != null)
        {
            Employees.Remove(employee);
            await Task.Delay(1500);
            Console.WriteLine("Employee deleted successfully.");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }
}