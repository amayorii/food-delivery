namespace food_delivery;

public class Menu : ICommand
{
    private readonly string _name;
    private readonly List<ICommand> _children = new();

    public string Name => _name;

    public Menu(string name)
    {
        _name = name;
    }

    public void Add(ICommand command)
    {
        _children.Add(command);
    }

    public void Execute()
    {
        while (true)
        {
            Console.WriteLine($"\n=== {_name} ===");
            Console.WriteLine("0. Back / Exit");
            for (int i = 0; i < _children.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_children[i].Name}");
            }

            Console.Write("\nSelect option: ");
            string? input = Console.ReadLine();

            if (input == "0")
            {
                break; // returns to previous menu or exits app
            }

            if (int.TryParse(input, out int choice) && choice > 0 && choice <= _children.Count)
            {
                try
                {
                    _children[choice - 1].Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
}