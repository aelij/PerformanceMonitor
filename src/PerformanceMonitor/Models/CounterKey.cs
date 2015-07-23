using System;

namespace PerformanceMonitor.Models
{
    public struct CounterKey : IEquatable<CounterKey>
    {
        public CounterKey(string category, string name, string instance)
        {
            Category = category;
            Name = name;
            Instance = instance;
        }

        public bool Equals(CounterKey other)
        {
            return string.Equals(Category, other.Category) &&
                   string.Equals(Name, other.Name) &&
                   string.Equals(Instance, other.Instance);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CounterKey && Equals((CounterKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Category.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Instance.GetHashCode();
                return hashCode;
            }
        }

        public string Category { get; }

        public string Name { get; }

        public string Instance { get; }
    }
}