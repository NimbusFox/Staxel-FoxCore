using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Classes.Exceptions;
using NimbusFox.FoxCore.Dependencies.Harmony;
using Plukit.Base;
using Event = NimbusFox.FoxCore.Classes.Event;

namespace NimbusFox.FoxCore.Events {
    public static class PatchController {
        private static readonly HarmonyInstance Instance;

        static PatchController() {
            Instance = HarmonyInstance.Create("nimbusfox.foxcore.patches");
            WriteLogger("Listening for patch requests...");
        }

        private static void WriteLogger(string text) {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine($"FoxPatch: {text}");
            Console.ResetColor();
        }

        public static void Add(Type owner, string targetMethod, object runBeforeParent = null, Delegate runBefore = null, object runAfterParent = null, Delegate runAfter = null) {

            if (runBefore == null && runAfter == null) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod arguments cannot be both null");
            }

            if (runBefore != null && runBeforeParent == null || runAfter != null && runAfterParent == null) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod's parents cannot be null if they are not null");
            }

            MethodInfo original;
            if ((original = owner.GetMethod(targetMethod)) == null) {
                throw new MethodNotExistsException($"{targetMethod} does not exist in {owner.FullName}");
            }

            if (runBefore != null) {
                if (!SameParameters(original.GetParameters(), runBefore.Method.GetParameters())) {
                    throw new InvalidParametersException("The runBefore method must have the exact same parameters as the original function");
                }
            }

            if (runAfter != null) {
                if (!SameParameters(original.GetParameters(), runAfter.Method.GetParameters())) {
                    throw new InvalidParametersException("The runAfter method must have the exact same parameters as the original function");
                }
            }

            if (runBefore != null) {
                WriteLogger($"Got a request from {runBeforeParent.GetType().FullName}.{runBefore.Method.Name} for {owner.FullName}.{targetMethod} prefix");
            }

            if (runAfter != null) {
                WriteLogger($"Got a request from {runAfterParent.GetType().FullName}.{runAfter.Method.Name} for {owner.FullName}.{targetMethod} postfix");
            }

            var eventItem = new Event(original, runBeforeParent, runBefore?.Method, runAfterParent, runAfter?.Method);

            eventItem.PatchedMethod = Instance.Patch(eventItem.Original, eventItem.HPrefix, eventItem.HPostfix);

            if (runBefore != null) {
                WriteLogger($"Adding {runBeforeParent.GetType().FullName}.{runBefore.Method.Name} to prefix patch cycle {owner.FullName}.{targetMethod}");
            }

            if (runAfter != null) {
                WriteLogger($"Adding {runAfterParent.GetType().FullName}.{runAfter.Method.Name} to postfix patch cycle {owner.FullName}.{targetMethod}");
            }
        }

        private static bool SameParameters(IReadOnlyCollection<ParameterInfo> original, IReadOnlyList<ParameterInfo> other) {
            if (original.Count != other.Count) {
                return false;
            }

            return !original.Where((t, i) => t.ParameterType != other[i].ParameterType || t.Name != other[i].Name).Any();
        }
    }
}
