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
    public class PatchController {
        private readonly HarmonyInstance _instance;

        private static readonly List<Event> _events = new List<Event>();

        static PatchController() {
            var controller = new PatchController("NimbusFox.FoxCore.PatchController");

            controller.Add(typeof(Logger), nameof(Logger.LogCriticalException), typeof(PatchController), "DisplayPatchedItemOrigin");
        }

        private static void DisplayPatchedItemOrigin(string message, Exception e) {
            foreach (var eventItem in new List<Event>(_events)) {
                if (e.StackTrace.Contains(eventItem.PatchedMethod.Name) ||
                    e.InnerException?.InnerException != null &&
                    e.InnerException.InnerException.StackTrace.Contains(eventItem.PatchedMethod.Name)) {
                    if (eventItem.Prefix != null) {
                        Logger.WriteLine($"FoxPatch: {eventItem.PatchedMethod} is linked to {eventItem.PrefixParent.FullName}.{eventItem.Prefix.Name}");
                    }

                    if (eventItem.Postfix != null) {
                        Logger.WriteLine($"FoxPatch: {eventItem.PatchedMethod.Name} is linked to {eventItem.PostfixParent.FullName}.{eventItem.Postfix.Name}");
                    }
                }
            }
        }

        public PatchController(string instanceName) {
            _instance = HarmonyInstance.Create(instanceName);
            WriteLogger($"Patch instance initialised");
        }

        private void WriteLogger(string text) {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine($"FoxPatch ({_instance.Id}): {text}");
            Console.ResetColor();
        }

        public void Add(Type owner, string targetMethod, Type runBeforeParent = null, string runBeforeMethodName = null, Type runAfterParent = null, string runAfterMethodName = null) {

            if (!runBeforeMethodName.IsNullOrEmpty() && !runAfterMethodName.IsNullOrEmpty()) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod arguments cannot be both null");
            }

            if (!runBeforeMethodName.IsNullOrEmpty() && runBeforeParent == null || !runAfterMethodName.IsNullOrEmpty() && runAfterParent == null) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod's parents cannot be null if they are not null");
            }

            MethodInfo runBefore = null;
            MethodInfo runAfter = null;

            if (!runBeforeMethodName.IsNullOrEmpty() && runBeforeParent != null) {
                if (runBeforeParent.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Any(x => x.Name == runBeforeMethodName)) {
                    runBefore = runBeforeParent.GetMethod(runBeforeMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (runBefore == null) {
                    throw new MethodNotExistsException($"Unable to access {runBeforeParent.FullName}.{runBeforeMethodName}");
                }
            }

            if (!runAfterMethodName.IsNullOrEmpty() && runAfterParent != null) {
                if (runAfterParent.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Any(x => x.Name == runAfterMethodName)) {
                    runAfter = runAfterParent.GetMethod(runAfterMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (runAfter == null) {
                    throw new MethodNotExistsException($"Unable to access {runAfterParent.FullName}.{runAfterMethodName}");
                }
            }

            MethodInfo original;
            if ((original = owner.GetMethod(targetMethod)) == null) {
                throw new MethodNotExistsException($"{targetMethod} does not exist in {owner.FullName}");
            }

            if (runBefore != null) {
                if (!SameParameters(original.GetParameters(), runBefore.GetParameters())) {
                    throw new InvalidParametersException("The runBefore method must have the exact same parameters as the original function");
                }

                if (!runBefore.IsStatic) {
                    throw new InvalidParametersException($"{runBeforeParent.FullName}.{runBefore.Name} is not a static function");
                }
            }

            if (runAfter != null) {
                if (!SameParameters(original.GetParameters(), runAfter.GetParameters())) {
                    throw new InvalidParametersException("The runAfter method must have the exact same parameters as the original function");
                }

                if (!runAfter.IsStatic) {
                    throw new InvalidParametersException($"{runAfterParent.FullName}.{runAfter.Name} is not a static function");
                }
            }

            if (runBefore != null) {
                WriteLogger($"Got a request from {runBeforeParent.FullName}.{runBefore.Name} for {owner.FullName}.{targetMethod} prefix");
            }

            if (runAfter != null) {
                WriteLogger($"Got a request from {runAfterParent.FullName}.{runAfter.Name} for {owner.FullName}.{targetMethod} postfix");
            }

            var eventItem = new Event(original, runBeforeParent, runBefore, runAfterParent, runAfter);

            eventItem.PatchedMethod = _instance.Patch(eventItem.Original, eventItem.HPrefix, eventItem.HPostfix);

            _events.Add(eventItem);

            if (runBefore != null) {
                WriteLogger($"Adding {runBeforeParent.FullName}.{runBefore.Name} to prefix patch cycle {owner.FullName}.{targetMethod}");

                WriteLogger($"{eventItem.PatchedMethod.Name} is linked to {runBeforeParent.FullName}.{runBefore.Name}");
            }

            if (runAfter != null) {
                WriteLogger($"Adding {runAfterParent.FullName}.{runAfter.Name} to postfix patch cycle {owner.FullName}.{targetMethod}");

                WriteLogger($"{eventItem.PatchedMethod.Name} is linked to {runAfterParent.FullName}.{runAfter.Name}");
            }
        }

        private static bool SameParameters(IReadOnlyCollection<ParameterInfo> original, IReadOnlyList<ParameterInfo> other) {
            if (original.Count != (other.Any(x => x.Name == "__instance") ? other.Count - 1 : other.Count)) {
                return false;
            }

            var check = new List<ParameterInfo>(other);

            if (check.Any(x => x.Name == "__instance")) {
                check.Remove(check.First(x => x.Name == "__instance"));
            }

            return !original.Where((t, i) => t.ParameterType != check[i].ParameterType || t.Name != check[i].Name).Any();
        }
    }
}
