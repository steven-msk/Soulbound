using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Common {
	public sealed class BufferedQueue<T> where T : struct {
		private readonly T[] buffer;
		private readonly int capacity;
		private int head = 0;
		private int tail = 0;
		private int count = 0;

		public int Count => count;
		public int Size => buffer.Length;
		public int Capacity => capacity;

		public BufferedQueue(int capacity) {
			buffer = new T[capacity];
			this.capacity = capacity;
		}

		public bool TryEnqueue(in T item) {
			if (count == buffer.Length) return false;

			buffer[tail] = item;
			tail = (tail + 1) % buffer.Length;
			count++;
			return true;
		}

		public bool TryDequeue(out T item) {
			if (count == 0) {
				item = default;
				return false;
			}

			item = buffer[head];
			head = (head + 1) % buffer.Length;
			count--;
			return true;
		}

		public void Clear() {
			head = 0;
			tail = 0;
			count = 0;
		}
	}
}
