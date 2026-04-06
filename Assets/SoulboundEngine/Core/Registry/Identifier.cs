using System;
using System.Linq;

namespace SoulboundEngine.Core.Registry {
	public readonly struct Identifier : IEquatable<Identifier> {
		public const string DEFAULT_RESERVED_NAMESPACE = "soulbound";
		private const string ALLOWED_NAMESPACE_SPECIAL_CHARS = "_-";
		private const string ALLOWED_PATH_SPECIAL_CHARS = "_-";

		public readonly string _namespace;
		public readonly string path;

		public Identifier(string _namespace, string[] pathKeys) {
			if (!ValidateNamespace(_namespace)) throw InvalidNamespace(_namespace);

			string path = GetPath(pathKeys);
			if (!ValidatePath(path)) throw InvalidPath(path);

			this._namespace = _namespace;
			this.path = path;
		}

		public Identifier(params string[] pathKeys)
			: this(DEFAULT_RESERVED_NAMESPACE, pathKeys) {
		}

		private Identifier(string _namespace, string path) {
			this._namespace = _namespace;
			this.path = path;
		}

		private static bool ValidateNamespace(string _namespace) {
			if (string.IsNullOrEmpty(_namespace) || string.IsNullOrWhiteSpace(_namespace)) return false;
			foreach (char c in _namespace) {
				if (ALLOWED_NAMESPACE_SPECIAL_CHARS.Contains(c)) continue;
				if (char.IsLetter(c) && !char.IsLower(c)) return false;
			}
			return true;
		}

		private static bool ValidatePath(string path) {
			if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) return false;

			string[] keys = path.Split('.');
			foreach (var key in keys) {
				if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) return false;

				foreach (char c in key) {
					if (ALLOWED_PATH_SPECIAL_CHARS.Contains(c)) continue;
					if (char.IsLetter(c) && !char.IsLower(c)) return false;
				}
			}
			return true;
		}

		public bool IsPartiallyMatching(string str) {
			return _namespace.StartsWith(str) || path.StartsWith(str);
		}

		public override string ToString() {
			return $"{_namespace}:{path}";
		}

		public bool Equals(Identifier other) {
			return other._namespace == _namespace && other.path == path;
		}

		public override bool Equals(object obj) {
			return obj is Identifier other && ((IEquatable<Identifier>)this).Equals(other);
		}

		public override int GetHashCode() {
			return HashCode.Combine(_namespace, path);
		}

		public static string GetPath(params string[] pathKeys) {
			return string.Join('.', pathKeys);
		}

		public static Identifier FromString(string str) {
			string[] colonSplit = str.Split(':', StringSplitOptions.RemoveEmptyEntries);

			string _namespace = DEFAULT_RESERVED_NAMESPACE;
			if (colonSplit.Length == 2) {
				_namespace = colonSplit[0];
			} else if (colonSplit.Length != 1) throw InvalidIdentifier(str);

			if (!ValidateNamespace(_namespace)) throw InvalidNamespace(str);

			string path = colonSplit.Last();
			if (!ValidatePath(path)) throw InvalidPath(str);

			return new Identifier(_namespace, path);
		}

		public static bool TryFromString(string str, out Identifier identifier) {
			try {
				identifier = FromString(str);
			} catch (ArgumentException) {
				identifier = default;
			}
			return ValidatePath(identifier.path) && ValidateNamespace(identifier._namespace);
		}

		private static ArgumentException InvalidIdentifier(string str) {
			return new ArgumentException($"Invalid identifier: {str}");
		}

		private static ArgumentException InvalidNamespace(string str) {
			return new ArgumentException($"Invalid namespace: {str}");
		}

		private static ArgumentException InvalidPath(string str) {
			return new ArgumentException($"Invalid path: {str}");
		}
	}
}
