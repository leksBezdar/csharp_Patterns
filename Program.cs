using System.Runtime.InteropServices;

namespace SingletonPattern
{
    public class Singleton<T> where T : class, new()
    {
        private static T? instance = null;
        private static readonly object lockObject = new object();

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
        private GCHandle? objectID = null;

        public GCHandle ObjectID
        {
            get
            {
                if (objectID == null)
                {
                    objectID = GCHandle.Alloc(Instance, GCHandleType.Pinned);
                }
                return objectID.Value;
            }
        }

        public void Free()
        {
            if (objectID.HasValue && objectID.Value.IsAllocated)
            {
                objectID.Value.Free();
                objectID = null;
            }
        }
    }
}

namespace EventBrokerPattern
{
    public class EventBroker
    {
        private readonly Dictionary<string, List<Action<object>>> _subscribers = new();

        public void Subscribe(string eventName, Action<object> action)
        {
            if (!_subscribers.ContainsKey(eventName))
            {
                _subscribers[eventName] = new List<Action<object>>();
            }

            _subscribers[eventName].Add(action);
        }

        public void Unsubscribe(string eventName, Action<object> action)
        {
            if (_subscribers.ContainsKey(eventName))
            {
                _subscribers[eventName].Remove(action);
            }
        }

        public void Publish(string eventName, object eventData)
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
            eventBroker.Subscribe("eventOccured", OnEventOccured);
        }

        private void OnEventOccured(object eventData)
        {
            Console.WriteLine($"Subscriber: Event data received: {eventData}");
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
}

namespace AdapterPattern
{
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
}

namespace HelperPattern
{
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

            if (string.IsNullOrEmpty(input))
            {
                return frequency;
            }

            foreach (char c in input)
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
}

class Program
{
    static void Main()
    {
        TestSingletonPattern();
        TestEventBrokerPattern();
        TestAdapterPattern();
        TestHelperPattern();

        Console.ReadKey();
    }

    private static void TestSingletonPattern()
    {
        var singletonObject1 = SingletonPattern.SingletonObject.Instance;
        var singletonObject2 = SingletonPattern.SingletonObject.Instance;

        IntPtr address1 = singletonObject1.ObjectID.AddrOfPinnedObject();
        IntPtr address2 = singletonObject2.ObjectID.AddrOfPinnedObject();
        Console.WriteLine($"{address1} == {address2}: {address1 == address2}");
        singletonObject1.Free();
        singletonObject2.Free();
    }

    private static void TestEventBrokerPattern()
    {
        var eventBroker = new EventBrokerPattern.EventBroker();
        var publisher = new EventBrokerPattern.Publisher(eventBroker);
        var subscriber = new EventBrokerPattern.Subscriber(eventBroker);
        publisher.PublishEvent();
    }

    private static void TestAdapterPattern()
    {
        AdapterPattern.PaymentSystemAdapter.Perform(1000, "Rub");
    }

    private static void TestHelperPattern()
    {
        Console.WriteLine($" Слово 'кабак' - это палиндром? {HelperPattern.StringHelper.IsPalindrome("кабак")}");
        Console.WriteLine($" Слово 'палиндром' - это палиндром? {HelperPattern.StringHelper.IsPalindrome("палиндром")}");

        var frequency = HelperPattern.StringHelper.GetCharacterFrequency("Тетрагидропиранилциклопентилтетрагидропиридопиридиновые");
        Console.WriteLine("Частота символов:");
        foreach (var kvp in frequency)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }
}
