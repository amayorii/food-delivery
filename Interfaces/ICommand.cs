namespace food_delivery;

public interface ICommand
{
    string Name { get; }
    void Execute();
}