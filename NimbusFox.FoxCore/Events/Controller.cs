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
    public static class Controller {
        private static readonly HarmonyInstance Instance;

        static Controller() {
            Instance = HarmonyInstance.Create("nimbusfox.foxcore.patches");
            WriteLogger($"Listening for patch requests...");
        }

        private static void WriteLogger(string text) {
            Console.ForegroundColor = ConsoleColor.Green;
            Logger.WriteLine($"FoxPatch: {text}");
            Console.ResetColor();
        }

        public static void Add(Type owner, string targetMethod, object runBeforeParent = null, string runBeforeMethod = null, object runAfterParent = null, string runAfterMethod = null) {
            MethodInfo runBefore = null;
            MethodInfo runAfter = null;
            if (runBeforeParent != null && runBeforeMethod != null) {
                runBefore = runBeforeParent.GetType().GetMethod(runBeforeMethod);
            }

            if (runAfterParent != null && runAfterMethod != null) {
                runAfter = runAfterParent.GetType().GetMethod(runAfterMethod);
            }

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
                WriteLogger($"Got a request from {runBeforeParent.GetType().FullName}.{runBefore.Name} for {owner.FullName}.{targetMethod} prefix");
            }

            if (runAfter != null) {
                WriteLogger($"Got a request from {runAfterParent.GetType().FullName}.{runAfter.Name} for {owner.FullName}.{targetMethod} postfix");
            }

            var eventItem = new Event(original, runBeforeParent, runBefore, runAfterParent, runAfter);

            WriteLogger(Instance.Patch(eventItem.Original, eventItem.HPrefix, eventItem.HPostfix).Name);

            if (runBefore != null) {
                WriteLogger($"Adding {runBeforeParent.GetType().FullName}.{runBefore.Name} to prefix patch cycle {owner.FullName}.{targetMethod}");
            }

            if (runAfter != null) {
                WriteLogger($"Adding {runAfterParent.GetType().FullName}.{runAfter.Name} to postfix patch cycle {owner.FullName}.{targetMethod}");
            }
        }
    }
}
