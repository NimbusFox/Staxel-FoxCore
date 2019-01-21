using System;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

namespace NimbusFox.FoxCore.Classes {
    internal class Event {
        public MethodInfo Original { get; }
        public ConstructorInfo OriginalConstructor { get; }
        public Type PrefixParent { get; }
        public MethodInfo Prefix { get; }
        public Type PostfixParent { get; }
        public MethodInfo Postfix { get; }
        public DynamicMethod PatchedMethod { get; set; }

        public HarmonyMethod HPrefix { get; }
        public HarmonyMethod HPostfix { get; }

        public Event(MethodInfo original, Type prefixParent, MethodInfo prefix, Type postfixParent, MethodInfo postfix) {
            Original = original;
            if (prefix != null && prefixParent != null) {
                Prefix = prefix;
                PrefixParent = prefixParent;
                HPrefix = new HarmonyMethod(prefix);
            }

            if (postfix != null && postfixParent != null) {
                Postfix = postfix;
                PostfixParent = postfixParent;
                HPostfix = new HarmonyMethod(postfix);
            }
        }

        public Event(ConstructorInfo original, Type parent, MethodInfo method) {
            OriginalConstructor = original;
            PrefixParent = parent;
            Prefix = method;
        }
    }
}
