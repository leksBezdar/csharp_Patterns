using System.Runtime.InteropServices;

public class Singleton<T> where T: class, new()
{
    private static T? instance = null;
    private static object lockObject = new object();
    protected Singleton() { }

    public static T Instance
    {
        get
        {
            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            } 
        }
    }
}

public class SingletonObject : Singleton<SingletonObject>
{
    public GCHandle ObjectID
    {
        get
        {
            return GCHandle.Alloc(Instance, GCHandleType.Pinned);
        }
    }
}


public class EventBroker
{
    private readonly Dictionary<string, List<Action<object>>> _subscribers = new();

    public void subscribe(string eventName, Action<object> action)
    {
        if (!_subscribers.ContainsKey(eventName))
        {
            _subscribers[eventName] = new List<Action<object>>();
        }

        _subscribers[eventName].Add(action);
    }

    public void unsubscribe(string eventName, Action<object> action)
    {
        if (_subscribers.ContainsKey(eventName))
        {
            _subscribers[eventName].Remove(action);
        }
    }

    public void Publish(string eventName, object eventData )
    {
        if (_subscribers.ContainsKey(eventName))
        {
            foreach (var action in _subscribers[eventName])
            {
                action(eventData);
            }
        }
    }
}

public class Subscriber
{
    public Subscriber(EventBroker eventBroker)
    {
        eventBroker.subscribe("eventOccured", OnEventOccured);
    }

    private void OnEventOccured(object eventData)
    {
        Console.WriteLine($"Subscriber: Event data recieved: {eventData}");
    }
}

public class Publisher
{
    private readonly EventBroker _eventBroker;

    public Publisher(EventBroker eventBroker)
    {
        _eventBroker = eventBroker;
    }

    public void PublishEvent()
    {
        _eventBroker.Publish("eventOccured", "Hello, World!");
    }
}


public interface IOldPaymentSystem
{
    void ProcessPayment(double amount);
}

public interface INewPaymentSystem
{
    void ProcessPayment(double amount, string currency);
}

public class OldPaymentSystem : IOldPaymentSystem
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Processing payment of {amount} with an old system logic");
    }

}

public class NewPaymentSystem : INewPaymentSystem
{
    public void ProcessPayment(double amount, string currency)
    {
        Console.WriteLine($"Processing payment of {amount} {currency} with a new system logic");
    }
}

public class PaymentSystemAdapter : IOldPaymentSystem
{
    private readonly INewPaymentSystem _newPaymentSystem;
    private readonly string _defaultCurrency;

    public PaymentSystemAdapter(INewPaymentSystem newPaymentSystem, string defaultCurrency)
    {
        _newPaymentSystem = newPaymentSystem;
        _defaultCurrency = defaultCurrency;
    }

    public void ProcessPayment(double amount)
    {
        _newPaymentSystem.ProcessPayment(amount, _defaultCurrency);
    }

    public static void Perform(double amount, string currency)
    {
        INewPaymentSystem newPaymentSystem = new NewPaymentSystem();
        PaymentSystemAdapter paymentSystemAdapter = new(newPaymentSystem, currency);

        paymentSystemAdapter.ProcessPayment(amount);
    }
}


public class StringHelper
{
    public static bool IsPalindrome(string input)
    {
        if (string.IsNullOrEmpty(input)) 
        {
            return false;
        }
        string reversedString = new string(input.Reverse().ToArray());
        return input.Equals(reversedString, StringComparison.OrdinalIgnoreCase);
    }

    public static Dictionary<char, int> GetCharacterFrequency(string input)
    {
        var frequency = new Dictionary<char, int>();

        if (string.IsNullOrEmpty(input)) {
            return frequency;
        };

        foreach(char c in input)
        {
            if (frequency.ContainsKey(c))
            {
                frequency[c]++;
            }
            else
            {
                frequency[c] = 1;
            }
        }
        return frequency;
    }
}

class Program
{
    public SingletonObject singletonObject1 = new();
    public SingletonObject singletonObject2 = new();
    static void Main(string[] args)
    {
        Program program = new();
        
        IntPtr addres1 = program.singletonObject1.ObjectID.AddrOfPinnedObject();
        IntPtr addres2 = program.singletonObject2.ObjectID.AddrOfPinnedObject();
        //Console.WriteLine($"{addres1} == {addres2}: {addres1 == addres2}");

        EventBroker eventBroker = new();
        Publisher publisher = new(eventBroker);
        Subscriber subscriber = new(eventBroker);

        publisher.PublishEvent();


        //PaymentSystemAdapter.Perform(1000, "Rub");
        Console.ReadKey();
    }
}
