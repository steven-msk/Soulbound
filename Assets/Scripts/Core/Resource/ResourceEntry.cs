using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Resource {
    public struct ResourceEntry {
        public UnityEngine.Object resource;
        public string name;
        public string path;

        public ResourceEntry(UnityEngine.Object resource, string name, string path) {
            this.resource = resource;
            this.name = name;
            this.path = path;
        }

        public override bool Equals(object obj) {
            return obj is ResourceEntry other && name == other.name && path == other.path;
        }

        public override int GetHashCode() => HashCode.Combine(resource, name, path);
    }
}
