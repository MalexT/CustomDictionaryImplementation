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

			TestBasicAddRemove();
			TestAddDuplicate();
			TestGetAndTryGet();
			TestResize();
			TestCollisions();
			TestIterator();
			TestNullKeyHandling();
			TestValueTypes();

			Console.WriteLine("All tests executed.");
		}

		private static void RemoveElementTest(CustomDictionary<string, int> map)
		{
			Console.WriteLine("RemoveElementTest: <begin>");

			bool removed = map.Remove("two");
			Console.WriteLine("Removed two: " + removed);
			Console.WriteLine("Count after remove: " + map.Count);

			Console.WriteLine("RemoveElementTest: <end>");
			Console.WriteLine("\n\n");
		}

		private static void IterateTest(CustomDictionary<string, int> map)
		{
			Console.WriteLine("IterateTest: <begin>");

			foreach (var kv in map)
			{
				Console.WriteLine($"{kv.Key} -> {kv.Value}");
			}

			Console.WriteLine("IterateTest: <end>");
			Console.WriteLine("\n\n");
		}

		private static void LookupTest(CustomDictionary<string, int> map)
		{
			Console.WriteLine("LookupTest: <begin>");

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

			Console.WriteLine("LookupTest: <end>");
			Console.WriteLine("\n\n");
		}

		private static void AddTest(CustomDictionary<string, int> map)
		{
			Console.WriteLine("AddTest: <begin>");

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

			added = map.TryAdd("five", 5);
			Console.WriteLine($"Added 'three': {added}"); // true

			added = map.TryAdd("six", 6);
			Console.WriteLine($"Added 'three': {added}"); // true

			added = map.TryAdd("seven", 7);
			Console.WriteLine($"Added 'three': {added}"); // true

			added = map.TryAdd("six", 9);
			Console.WriteLine($"Added 'three': {added}"); // false

			Console.WriteLine("AddTest: <end>");
			Console.WriteLine("\n\n");
		}

		static void PrintMap<TKey, TValue>(CustomDictionary<TKey, TValue> map)
		{
			Console.Write("{ ");
			bool first = true;
			foreach (var kv in map)
			{
				if (!first)
				{
					Console.Write(", ");
				}

				Console.Write($"{kv.Key} = {kv.Value}");

				first = false;
			}
			Console.WriteLine(" }");
		}

		static string SafeGetString<TKey, TValue>(CustomDictionary<TKey, TValue> map, TKey key)
		{
			try
			{
				var v = map.Get(key);
				return v?.ToString() ?? "null";
			}
			catch (KeyNotFoundException)
			{
				return "null";
			}
		}

		static void TestBasicAddRemove()
		{
			Console.WriteLine("TestBasicAddRemove");
			var map = new CustomDictionary<int, string>();
			map.Add(1, "One");
			map.Add(2, "Two");
			map.Add(3, "Three");
			Console.WriteLine("Count after adds: " + map.Count); // expect 3
			PrintMap(map);

			bool removed = map.Remove(2);
			Console.WriteLine("Removed 2: " + removed); // expect true
			Console.WriteLine("Count after remove: " + map.Count); // expect 2
			PrintMap(map);

			removed = map.Remove(42);
			Console.WriteLine("Removed missing 42: " + removed); // expect false
			Console.WriteLine();
		}

		static void TestAddDuplicate()
		{
			Console.WriteLine("TestAddDuplicate");
			var map = new CustomDictionary<string, int>();
			map.Add("a", 1);

			try
			{
				map.Add("a", 100); // will throw
				Console.WriteLine("Add duplicate did NOT throw (unexpected)");
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("Add duplicate threw: " + ex.Message);
			}

			bool tryAddResult = map.TryAdd("a", 200);
			Console.WriteLine("TryAdd duplicate returned: " + tryAddResult); // expect false

			bool tryAddNew = map.TryAdd("b", 2);
			Console.WriteLine("TryAdd new returned: " + tryAddNew); // expect true
			PrintMap(map);
			Console.WriteLine();
		}

		static void TestGetAndTryGet()
		{
			Console.WriteLine("TestGetAndTryGet");
			var map = new CustomDictionary<int, string>();
			map.Add(10, "ten");
			map.Add(20, "twenty");

			Console.WriteLine("Get 10: " + SafeGetString(map, 10)); // expect "ten"
			Console.WriteLine("Get missing 99: " + SafeGetString(map, 99)); // expect "null"

			if (map.TryGetValue(20, out var v20))
			{
				Console.WriteLine("TryGetValue 20 -> " + v20);
			}
			else
			{
				Console.WriteLine("TryGetValue 20 -> missing");
			}

			if (map.TryGetValue(99, out var v99))
			{
				Console.WriteLine("TryGetValue 99 -> " + v99);
			}
			else
			{
				Console.WriteLine("TryGetValue 99 -> missing");
			}
			Console.WriteLine();
		}

		static void TestResize()
		{
			Console.WriteLine("TestResize");
			var map = new CustomDictionary<int, int>(2);
			for (int i = 1; i <= 40; i++)
			{
				map.Add(i, i * 10);
			}
			Console.WriteLine("Count after many adds: " + map.Count); // expect 40
			Console.WriteLine("Sample gets after resize:");
			Console.WriteLine("Get 1 -> " + SafeGetString(map, 1));
			Console.WriteLine("Get 40 -> " + SafeGetString(map, 40));
			Console.WriteLine();
		}

		class PoorHashComparer<T> : IEqualityComparer<T>
		{
			public bool Equals(T? x, T? y) => EqualityComparer<T>.Default.Equals(x, y);
			public int GetHashCode(T obj) => 42; // constant hash (collisions increase)
		}

		static void TestCollisions()
		{
			Console.WriteLine("TestCollisions (forced)");
			var map = new CustomDictionary<string, int>(capacity: 4, comparer: new PoorHashComparer<string>());

			map.Add("Aa", 1);
			map.Add("Bb", 2);
			map.Add("Cc", 3);
			map.Add("Dd", 4);
			map.Add("Ee", 5);

			Console.WriteLine("Count after adds with collisions: " + map.Count);
			PrintMap(map);

			Console.WriteLine("Get Aa -> " + SafeGetString(map, "Aa"));
			Console.WriteLine("Get Ee -> " + SafeGetString(map, "Ee"));

			Console.WriteLine("Remove 'Cc' -> " + map.Remove("Cc"));
			Console.WriteLine("Count after remove: " + map.Count);
			
			PrintMap(map);
			Console.WriteLine();
		}

		static void TestIterator()
		{
			Console.WriteLine("TestIterator");

			var map = new CustomDictionary<int, string>();
			map.Add(1, "one");
			map.Add(2, "two");
			map.Add(3, "three");

			Console.Write("Iterating: ");
			foreach (var kv in map)
			{
				Console.Write($"[{kv.Key}->{kv.Value}] ");
			}
			Console.WriteLine("\n");
		}

		static void TestNullKeyHandling()
		{
			Console.WriteLine("TestNullKeyHandling");
			var map = new CustomDictionary<string, int>();
			try
			{
				map.Add(null, 1);
				Console.WriteLine("Adding null key did NOT throw (unexpected)");
			}
			catch (ArgumentNullException ex)
			{
				Console.WriteLine("Add(null) threw: " + ex.ParamName);
			}

			try
			{
				map.Remove(null);
				Console.WriteLine("Remove(null) did NOT throw (unexpected)");
			}
			catch (ArgumentNullException ex)
			{
				Console.WriteLine("Remove(null) threw: " + ex.ParamName);
			}

			try
			{
				map.TryGetValue(null, out _);
				Console.WriteLine("TryGetValue(null) did NOT throw (unexpected)");
			}
			catch (ArgumentNullException ex)
			{
				Console.WriteLine("TryGetValue(null) threw: " + ex.ParamName);
			}
			Console.WriteLine();
		}

		static void TestValueTypes()
		{
			Console.WriteLine("TestValueTypes");

			var map = new CustomDictionary<int, DateTime>();
			var now = DateTime.UtcNow;
			map.Add(1, now);

			Console.WriteLine("Get 1 -> " + SafeGetString(map, 1));
			Console.WriteLine();
		}
	}
}
