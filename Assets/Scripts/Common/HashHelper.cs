using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Common {
    public class HashHelper {
        public static int StableHash(string id) {
            unchecked {
                const int offset = (int)2166136261;
                const int prime = 16777619;
                int hash = offset;
                foreach (char c in id) {
                    hash = (hash ^ c) * prime;
                }
                return hash;
            }
        }
    }
}
