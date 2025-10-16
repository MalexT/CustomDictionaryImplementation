using System.Collections;

namespace CustomDictionaryImplementation
{
    internal class Node<T>
    {
        public T Data { get; set; }
        public Node<T> Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    internal class SinglyLinkedList<T> : IEnumerable<T>
    {
        private Node<T> head;
        private Node<T> tail;
        public Node<T> First => head;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public SinglyLinkedList()
        {
            head = null;
            tail = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var cur = head;
            while (cur != null)
            {
                yield return cur.Data;
                cur = cur.Next;
            }
        }

        // Add a new node at the end
        public void Add(T data)
        {
            Node<T> newNode = new Node<T>(data);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.Next = newNode;
                tail = newNode;
            }
        }

        public bool Remove(Func<T, bool> predicate)
        {
            if (head == null)
            {
                return false;
            }

            // If head matches
            if (predicate(head.Data))
            {
                head = head.Next;
                
                if (head == null)
                {
                    tail = null;
                }

                return true;
            }

            Node<T> current = head;

            while (current.Next != null && !predicate(current.Next.Data))
            {
                current = current.Next;
            }

            if (current.Next == null)
            {
                return false;
            }

            current.Next = current.Next.Next;
            
            if (current.Next == null)
            {
                tail = current;
            }

            return true;
        }

        // Optional: remove by value (uses Equals)
        public bool Remove(T data)
        {
            return Remove(x => EqualityComparer<T>.Default.Equals(x, data));
        }
    }
}
