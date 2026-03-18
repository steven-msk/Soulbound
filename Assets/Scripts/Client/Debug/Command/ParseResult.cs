using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace SoulboundBackend.Core.Debug.Commands {
	public struct ParseResult<T> {
		public string message;
		public T value;
		public bool success;

		private ParseResult(T value)
			: this(value, message: string.Empty, success: true) {
		}

		private ParseResult(string message)
			: this(value: default, message, success: false) {
		}

		private ParseResult(T value, string message, bool success) {
			this.value = value;
			this.message = message;
			this.success = success;
		}

		public static ParseResult<T> Success(T value) {
			return new ParseResult<T>(value);
		}

		public static ParseResult<T> Fail() {
			return new ParseResult<T>($"Failed to parse arg type {typeof(T).Name}");
		}

		public static ParseResult<T> Fail(string message) {
			return new ParseResult<T>(message);
		}
	}
}
