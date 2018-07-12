using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NimbusFox.FoxCore.Dependencies.Harmony;

namespace NimbusFox.FoxCore.Classes {
    internal class Event {
        public MethodInfo Original { get; }
        public object PrefixParent { get; }
        public MethodInfo Prefix { get; }
        public object PostfixParent { get; }
        public MethodInfo Postfix { get; }
        public DynamicMethod PatchedMethod { get; set; }

        public HarmonyMethod HPrefix { get; }
        public HarmonyMethod HPostfix { get; }

        public Event(MethodInfo original, object prefixParent, MethodInfo prefix, object postfixParent, MethodInfo postfix) {
            Original = original;
            if (prefix != null && prefixParent != null) {
                Prefix = prefix;
                HPrefix = new HarmonyMethod(prefix);
            }

            if (postfix != null && postfixParent != null) {
                Postfix = postfix;
                HPostfix = new HarmonyMethod(postfix);
            }
        }
    }
}
