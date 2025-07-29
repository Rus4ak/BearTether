public class Bank
{
    private static Bank _instance;

    // Implementation of the Singleton pattern
    public static Bank Instance
    {
        get
        {
            _instance ??= new Bank();

            return _instance;
        }
    }

    public int Coins { get; set; } = 10000;
}
