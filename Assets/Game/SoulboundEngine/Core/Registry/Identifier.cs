using Brigadier.NET;
using System;

namespace SoulboundEngine.Core.Registry {
	public sealed class Identifier : IEquatable<Identifier> {
		public const string DEFAULT_NAMESPACE = "soulbound";
		private const string ALLOWED_PATH_CHARACTERS = "abcdefghijklmnopqrstuvwxyz1234567890-_./";
		private const string ALLOWED_NAMESPACE_CHARACTERS = "abcdefghijklmnopqrstuvwxyz1234567890-_";
		private const char NAMESPACE_SEPARATOR = ':';
		private const char PATH_SEPARATOR = '/';

		private readonly string _namespace;
		private readonly string path;

		private Identifier(string _namespace, string path) {
			this._namespace = _namespace;
			this.path = path;
		}

		public static Identifier Of(string id) {
			if (!IsValid(id)) throw new InvalidIdentifierException(id);
			return OfValidated(id);
		}

		public static Identifier Of(string _namespace, string path) {
			return Of(string.Concat(_namespace, NAMESPACE_SEPARATOR, path));
		}

		public static bool TryParse(string id, out Identifier identifier) {
			try {
				identifier = Of(id);
				return true;
			} catch (InvalidIdentifierException) {
				identifier = null;
				return false;
			}
		}

		public static bool TryParse(string _namespace, string path, out Identifier identifier) {
			try {
				identifier = Of(_namespace, path);
				return true;
			} catch (InvalidIdentifierException) {
				identifier = null;
				return false;
			}
		}

		public static bool TryFromCommandInput(IStringReader reader, out Identifier identifier) {
			string path;

			string _namespace = reader.ReadUnquotedString();

			if (reader.CanRead() && reader.Peek() == NAMESPACE_SEPARATOR) {
				reader.Skip();
				path = reader.ReadUnquotedString();
			} else {
				path = _namespace;
				_namespace = "";
			}

			return TryParse(_namespace, path, out identifier);
		}

		private static Identifier OfValidated(string id) {
			TrySplit(id, out string _namespace, out string path);
			return new Identifier(_namespace, path);
		}

		private static void TrySplit(string id, out string _namespace, out string path) {
			string[] split = id.Split(NAMESPACE_SEPARATOR);

			_namespace = "";
			if (split.Length == 1) path = split[0];
			else {
				_namespace = split[0];
				path = split[1];
			}
		}

		private static bool IsValid(string id) {
			if (string.IsNullOrEmpty(id)) return false;

			string[] colonSplit = id.Split(NAMESPACE_SEPARATOR);
			if (colonSplit.Length > 2) return false;

			if (colonSplit.Length == 1) {
				string path = colonSplit[0];

				if (!IsPathValid(path)) return false;
			} else {
				string _namespace = colonSplit[0];
				string path = colonSplit[1];

				if (!IsNamespaceValid(_namespace)) return false;
				if (!IsPathValid(path)) return false; 
			}

			return true;
		}

		private static bool IsNamespaceValid(string _namespace) {
			foreach (char c in _namespace) {
				if (!IsNamespaceCharacterValid(c)) return false;
			}
			return true;
		}

		private static bool IsPathValid(string path) {
			if (string.IsNullOrEmpty(path)) return false;

			foreach (char c in path) {
				if (!IsPathCharacterValid(c)) return false;
			}
			return true;
		}

		private static bool IsPathCharacterValid(char c) {
			return ALLOWED_PATH_CHARACTERS.Contains(c);
		}

		private static bool IsNamespaceCharacterValid(char c) {
			return ALLOWED_NAMESPACE_CHARACTERS.Contains(c);
		}

		public override string ToString() {
			return string.Concat(GetNamespace(), NAMESPACE_SEPARATOR, GetPath());
		}

		public string GetNamespace() {
			return string.IsNullOrEmpty(_namespace)
				? DEFAULT_NAMESPACE
				: _namespace;
		}

		public string GetPath() => path;

		public bool Equals(Identifier other) {
			return other.GetNamespace() == GetNamespace()
				&& other.GetPath() == GetPath();
		}

		public override bool Equals(object obj) {
			return obj is Identifier other && Equals(other);
		}

		public override int GetHashCode() {
			return HashCode.Combine(GetNamespace(), GetPath());
		}

		[Obsolete("Identifier instances should not be compared with ==. Compare with .Equals instead.", true)]
		public static bool operator ==(Identifier a, Identifier b) => throw new NotImplementedException();

		[Obsolete("Identifier instances should not be compared with !=. Compare with .Equals instead.", true)]
		public static bool operator !=(Identifier a, Identifier b) => throw new NotImplementedException();
	}
}
