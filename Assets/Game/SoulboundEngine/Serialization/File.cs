using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable

namespace SoulboundEngine.Serialization {
	public sealed class File : IEquatable<File> {
		private static readonly bool IS_WINDOWS = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public string FullPath { get; }

		public File(string path) {
			if (string.IsNullOrWhiteSpace(path)) {
				throw new ArgumentException("Path cannot be null or empty.", nameof(path));
			}

			this.FullPath = Path.GetFullPath(path);
		}

		// internal fast ctor
		private File(string fullPath, bool _) {
			this.FullPath = fullPath;
		}

		public string Name => Path.GetFileName(this.FullPath);

		public string NameWithoutExtension => Path.GetFileNameWithoutExtension(this.FullPath);

		public string Extension => Path.GetExtension(this.FullPath);

		public File? Parent {
			get {
				string parent = Path.GetDirectoryName(this.FullPath);
				return parent is null ? null : new File(parent, true);
			}
		}

		public File Combine(string child) => new(Path.Combine(this.FullPath, child));

		public File WithExtension(string newExtension) => new(Path.ChangeExtension(this.FullPath, newExtension));

		public bool Exists => System.IO.File.Exists(this.FullPath) || Directory.Exists(this.FullPath);

		public void ThrowIfNonExistent() {
			this.ThrowIfNonExistent(new FileNotFoundException("File does not exist", this.FullPath));
		}

		public void ThrowIfNonExistent(Exception exception) {
			if (!this.Exists) throw exception;
		}

		public File EnsureExists() {
			if (!this.Exists) Directory.CreateDirectory(this.FullPath);
			return this;
		}

		public bool IsFile => System.IO.File.Exists(this.FullPath);

		public bool IsDirectory => Directory.Exists(this.FullPath);

		public long Length => this.IsFile ? new FileInfo(this.FullPath).Length : 0;

		public DateTime LastModifiedUtc => this.IsFile
			? System.IO.File.GetLastWriteTimeUtc(this.FullPath)
			: Directory.GetLastWriteTimeUtc(this.FullPath);

		public bool IsReadOnly => this.IsFile && new FileInfo(this.FullPath).IsReadOnly;

		public bool HasChild(string pattern) => this.HasChild(pattern, out _);

		public bool HasChild(string pattern, out File child) {
			if (string.IsNullOrWhiteSpace(pattern)) {
				throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
			}

			child = default!;
			if (!this.IsDirectory) return false;

			string match = Directory.EnumerateFileSystemEntries(this.FullPath, pattern).FirstOrDefault();
			if (match is null) return false;

			child = new File(match, true);
			return true;
		}

		public bool Mkdir() {
			if (Directory.Exists(this.FullPath)) return false;
			Directory.CreateDirectory(this.FullPath);
			return true;
		}

		public IEnumerable<File> ListFiles(string searchPattern = "*", bool recursive = false) {
			if (!this.IsDirectory) return Enumerable.Empty<File>();

			SearchOption option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			return Directory.EnumerateFileSystemEntries(this.FullPath, searchPattern, option)
							.Select(p => new File(p, true));
		}

		public bool CreateNewFile() {
			if (this.Exists) return false;
			using (System.IO.File.Create(this.FullPath)) { }
			return true;
		}

		public bool Delete() {
			try {
				if (this.IsFile) System.IO.File.Delete(this.FullPath);
				else if (this.IsDirectory) Directory.Delete(this.FullPath, recursive: true);
				else return false;
				return true;
			} catch (IOException) { return false; } 
			catch (UnauthorizedAccessException) { return false; }
		}

		public File RenameTo(string newName) {
			string target = Path.Combine(Path.GetDirectoryName(this.FullPath) ?? "", newName);
			if (this.IsFile) System.IO.File.Move(this.FullPath, target);
			else if (this.IsDirectory) Directory.Move(this.FullPath, target);
			return new File(target);
		}

		public string ReadAllText() => System.IO.File.ReadAllText(this.FullPath);

		public byte[] ReadAllBytes() => System.IO.File.ReadAllBytes(this.FullPath);

		public void WriteAllText(string contents) {
			this.EnsureParentExists();
			System.IO.File.WriteAllText(this.FullPath, contents);
		}

		public void WriteAllBytes(byte[] bytes) {
			this.EnsureParentExists();
			System.IO.File.WriteAllBytes(this.FullPath, bytes);
		}

		/// <summary>Opens the file for reading. Caller owns the stream and must dispose it.</summary>
		public FileStream OpenRead() => new(this.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

		/// <summary>
		/// Opens the file for writing, creating it if it doesn't exist and
		/// truncating it if it does. Creates parent directories as needed.
		/// Caller owns the stream and must dispose it.
		/// </summary>
		public FileStream OpenWrite() {
			this.EnsureParentExists();
			return new FileStream(this.FullPath, FileMode.Create, FileAccess.Write, FileShare.None);
		}

		/// <summary>
		/// Opens the file for appending, creating it if it doesn't exist.
		/// Caller owns the stream and must dispose it.
		/// </summary>
		public FileStream OpenAppend() {
			this.EnsureParentExists();
			return new FileStream(this.FullPath, FileMode.Append, FileAccess.Write, FileShare.None);
		}

		/// <summary>
		/// Opens the file with explicit FileMode/FileAccess/FileShare for
		/// cases the convenience methods above don't cover (e.g. read/write
		/// random access into a region file). Caller owns the stream.
		/// </summary>
		public FileStream Open(FileMode mode, FileAccess access, FileShare share = FileShare.None) {
			if (access != FileAccess.Read) this.EnsureParentExists();
			return new FileStream(this.FullPath, mode, access, share);
		}

		/// <summary>Opens the file for text reading. Caller owns the reader and must dispose it.</summary>
		public StreamReader OpenText() => new(this.OpenRead());

		/// <summary>
		/// Opens the file for text writing, creating it if it doesn't exist
		/// and truncating it if it does. Caller owns the writer and must dispose it.
		/// </summary>
		public StreamWriter CreateText() => new(this.OpenWrite());

		/// <summary>
		/// Opens the file for text appending, creating it if it doesn't exist.
		/// Caller owns the writer and must dispose it.
		/// </summary>
		public StreamWriter AppendText() => new(this.OpenAppend());

		private void EnsureParentExists() => this.Parent?.Mkdir();

		public bool Equals(File? other) {
			return other is not null && string.Equals(
				this.FullPath, other.FullPath, IS_WINDOWS ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
			);
		}

		public override bool Equals(object? obj) => this.Equals(obj as File);

		public override int GetHashCode() => IS_WINDOWS ? this.FullPath.ToLowerInvariant().GetHashCode() : this.FullPath.GetHashCode();

		public override string ToString() => this.FullPath;

		public static implicit operator string(File f) => f.FullPath;
	}
}
