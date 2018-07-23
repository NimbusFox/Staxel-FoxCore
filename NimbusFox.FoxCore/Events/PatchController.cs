using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NimbusFox.FoxCore.Classes;
using NimbusFox.FoxCore.Classes.Exceptions;
using NimbusFox.FoxCore.Dependencies.Harmony;
using NimbusFox.FoxCore.Managers;
using Plukit.Base;
using Event = NimbusFox.FoxCore.Classes.Event;

namespace NimbusFox.FoxCore.Events {
    public class PatchController {
        private readonly HarmonyInstance _instance;

        private static readonly List<Event> Events = new List<Event>();

        static PatchController() {
            var controller = new PatchController("NimbusFox.FoxCore.PatchController");

            controller.Add(typeof(Logger), nameof(Logger.LogCriticalException), typeof(PatchController), nameof(DisplayPatchedItemOrigin));
        }

        private static void DisplayPatchedItemOrigin(string message, ref Exception e) {
            if (e?.StackTrace == null) {
                return;
            }

            if (e.StackTrace.IsNullOrEmpty()) {
                return;
            }

            try {
                var exceptionMessage = Environment.NewLine;

                foreach (var eventItem in new List<Event>(Events)) {
                    if (e.StackTrace.Contains(eventItem.PatchedMethod.Name)) {
                        if (eventItem.Prefix != null) {
                            exceptionMessage +=
                                $"FoxPatch: {eventItem.PatchedMethod} is linked to {eventItem.PrefixParent.FullName}.{eventItem.Prefix.Name}{Environment.NewLine}";
                        }

                        if (eventItem.Postfix != null) {
                            exceptionMessage +=
                                $"FoxPatch: {eventItem.PatchedMethod.Name} is linked to {eventItem.PostfixParent.FullName}.{eventItem.Postfix.Name}{Environment.NewLine}";
                        }

                        continue;
                    }

                    var exception = e.InnerException;

                    while (exception != null) {
                        if (!exception.StackTrace.IsNullOrEmpty() &&
                            exception.StackTrace.Contains(eventItem.PatchedMethod.Name)) {
                            if (eventItem.Prefix != null) {
                                exceptionMessage +=
                                    $"FoxPatch: {eventItem.PatchedMethod} is linked to {eventItem.PrefixParent.FullName}.{eventItem.Prefix.Name}{Environment.NewLine}";
                            }

                            if (eventItem.Postfix != null) {
                                exceptionMessage +=
                                    $"FoxPatch: {eventItem.PatchedMethod.Name} is linked to {eventItem.PostfixParent.FullName}.{eventItem.Postfix.Name}{Environment.NewLine}";
                            }

                            break;
                        }

                        exception = exception.InnerException;
                    }
                }

                if (exceptionMessage == "") {
                    return;
                }

                exceptionMessage += message;

                e = new Exception(exceptionMessage, e);
            } catch (Exception ex) {
                if (CoreHook.FxCore != null) {
                    CoreHook.FxCore.ExceptionManager.HandleException(ex, new Dictionary<string, object> { { "message", message }, { "exception", e } });
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
            if (runBeforeMethodName.IsNullOrEmpty() && runAfterMethodName.IsNullOrEmpty()) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod arguments cannot be both null");
            }

            if (!runBeforeMethodName.IsNullOrEmpty() && runBeforeParent == null || !runAfterMethodName.IsNullOrEmpty() && runAfterParent == null) {
                throw new ArgumentException("runBeforeMethod and runAfterMethod's parents cannot be null if they are not null");
            }

            MethodInfo runBefore = null;
            MethodInfo runAfter = null;

            if (!runBeforeMethodName.IsNullOrEmpty() && runBeforeParent != null) {
                if (runBeforeParent.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Any(x => x.Name == runBeforeMethodName)) {
                    runBefore = runBeforeParent.GetMethod(runBeforeMethodName,
                        BindingFlags.Instance | BindingFlags.Public |
                        BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (runBefore == null) {
                    throw new MethodNotExistsException($"Unable to access {runBeforeParent.FullName}.{runBeforeMethodName}");
                }
            }

            if (!runAfterMethodName.IsNullOrEmpty() && runAfterParent != null) {
                if (runAfterParent.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Any(x => x.Name == runAfterMethodName)) {
                    runAfter = runAfterParent.GetMethod(runAfterMethodName,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (runAfter == null) {
                    throw new MethodNotExistsException($"Unable to access {runAfterParent.FullName}.{runAfterMethodName}");
                }
            }

            MethodInfo original;
            if ((original = owner.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(x => runBefore != null && SameParameters(x.GetParameters(), runBefore.GetParameters())
                                         || runAfter != null && SameParameters(x.GetParameters(), runAfter.GetParameters()))) == default) {
                throw new MethodNotExistsException($"{targetMethod} does not exist in {owner.FullName}");
            }

            if (runBefore != null) {
                if (!SameParameters(original.GetParameters(), runBefore.GetParameters())) {
                    throw new InvalidParametersException($"{runBeforeParent.FullName}.{runBeforeMethodName} must have the exact same parameters as the original function");
                }

                if (!runBefore.IsStatic) {
                    throw new InvalidParametersException($"{runBeforeParent.FullName}.{runBefore.Name} is not a static function");
                }
            }

            if (runAfter != null) {
                if (!SameParameters(original.GetParameters(), runAfter.GetParameters())) {
                    throw new InvalidParametersException($"{runAfterParent.FullName}.{runAfterMethodName} must have the exact same parameters as the original function");
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

            Events.Add(eventItem);

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
            var check = new List<ParameterInfo>(other);

            if (check.Any(x => x.Name == "__instance")) {
                check.Remove(check.First(x => x.Name == "__instance"));
            }

            if (check.Any(x => x.Name == "__result")) {
                check.Remove(check.First(x => x.Name == "__result"));
            }

            if (check.Any(x => x.Name == "__state")) {
                check.Remove(check.First(x => x.Name == "__state"));
            }

            if (check.Any(x => x.Name == "__originalMethod")) {
                check.Remove(check.First(x => x.Name == "__originalMethod"));
            }

            foreach (var item in new List<ParameterInfo>(check.Where(x => x.Name.StartsWith("___")))) {
                check.Remove(item);
            }

            if (original.Count != check.Count) {
                return false;
            }

            return !original.Where((t, i) =>
                t.ParameterType != check[i].ParameterType &&
                 t.ParameterType.Name + "&" != check[i].ParameterType.Name || t.Name != check[i].Name).Any();
        }

        public void Add(Type owner, string targetMethod, Type overrideParent, string overrideMethodName,
            Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>> transpiler) {
            if (overrideMethodName.IsNullOrEmpty()) {
                throw new ArgumentException("overrideMethodName cannot be null or empty");
            }

            if (overrideParent == null) {
                throw new ArgumentException("overrideParent cannot be null");
            }

            MethodInfo overrideMethod = null;

            if (overrideParent.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Any(x => x.Name == overrideMethodName)) {
                overrideMethod = overrideParent.GetMethod(overrideMethodName,
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Static);
            }

            if (overrideMethod == null) {
                throw new MethodNotExistsException($"Unable to access {overrideParent.FullName}.{overrideMethodName}");
            }

            MethodInfo original;
            if ((original = owner.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(x => overrideMethod != null && SameParameters(x.GetParameters(), overrideMethod.GetParameters()))) == default) {
                throw new MethodNotExistsException($"{targetMethod} does not exist in {owner.FullName}");
            }

            if (!SameParameters(original.GetParameters(), overrideMethod.GetParameters())) {
                throw new InvalidParametersException($"{overrideParent.FullName}.{overrideMethodName} must have the exact same parameters as the original function");
            }

            if (!overrideMethod.IsStatic) {
                throw new InvalidParametersException($"{overrideParent.FullName}.{overrideMethod.Name} is not a static function");
            }

            WriteLogger($"Got a request from {overrideParent.FullName}.{overrideMethod.Name} for {owner.FullName}.{targetMethod}");

            var eventItem = new Event(original, overrideParent, overrideMethod, null, null);

            eventItem.PatchedMethod = _instance.Patch(eventItem.Original, eventItem.HPrefix, null, new HarmonyMethod(transpiler.Method));

            Events.Add(eventItem);

            WriteLogger($"Adding {overrideParent.FullName}.{overrideMethod.Name} to patch cycle {owner.FullName}.{targetMethod}");

            WriteLogger($"{eventItem.PatchedMethod.Name} is linked to {overrideParent.FullName}.{overrideMethod.Name}");
        }
    }
}
