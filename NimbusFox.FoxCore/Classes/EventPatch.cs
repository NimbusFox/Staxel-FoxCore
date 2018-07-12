using System;
using System.Collections.Generic;

namespace NimbusFox.FoxCore.Classes {
    internal class EventPatch {
        public Type TargetType { get; }
        public string Method { get; }
        public List<Guid> ToRun { get; } = new List<Guid>();
        public Type Prefix { get; set; }
        public Type PostFix { get; set; }

        public EventPatch(Type targetType, string method) {
            TargetType = targetType;
            Method = method;
        }
     }
}
