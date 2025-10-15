namespace CustomDictionaryImplementation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var map = new CustomDictionary<string, int>();

            AddTest(map);

            LookupTest(map);

            // Count
            Console.WriteLine("Count: " + map.Count);

            IterateTest(map);

            RemoveElementTest(map);
        }

        private static void RemoveElementTest(CustomDictionary<string, int> map)
        {
            bool removed = map.Remove("two");
            Console.WriteLine("Removed two: " + removed);
            Console.WriteLine("Count after remove: " + map.Count);
        }

        private static void IterateTest(CustomDictionary<string, int> map)
        {
            foreach (var kv in map)
            {
                Console.WriteLine($"{kv.Key} -> {kv.Value}");
            }
        }

        private static void LookupTest(CustomDictionary<string, int> map)
        {
            Console.WriteLine("two = " + map.Get("two")); // 22
            try
            {

                Console.WriteLine("sixteen = " + map.Get("sixteen")); // will throw
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (map.TryGetValue("four", out var val))
            {
                Console.WriteLine("four: " + val);
            }
            else
            {
                Console.WriteLine("four not present");
            }
        }

        private static void AddTest(CustomDictionary<string, int> map)
        {
            // Add
            map.Add("one", 1);
            map.Add("two", 2);

            try
            {
                map.Add("two", 22); // will throw
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("AddOrThrow error: " + ex.Message);
            }

            try
            {
                map.TryAdd("two", 200); // will throw
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("AddOrThrow error: " + ex.Message);
            }

            bool added = map.TryAdd("three", 3);
            Console.WriteLine($"Added 'three': {added}"); // true
        }
    }
}
