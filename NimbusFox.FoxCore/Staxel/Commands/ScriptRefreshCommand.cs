//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using NimbusFox.FoxCore.Interpreters;
//using Plukit.Base;
//using Staxel.Commands;
//using Staxel.Server;

//namespace NimbusFox.FoxCore.Commands {
//    internal class ScriptRefreshCommand : ICommandBuilder {
//        public string Execute(string[] bits, Blob blob, ClientServerConnection connection, ICommandsApi api,
//            out object[] responseParams) {
//            responseParams = new object[] { };
//            new Thread(() => {
//                var msg = "mods.nimbusfox.foxcore.scriptrefresh.success";

//                try {
//                    CoreIntMods.ReloadMods(true);
//                } catch {
//                    msg = "mods.nimbusfox.foxcore.scriptrefresh.fail";
//                }

//                api.MessagePlayer(connection.Credentials.Uid, msg, new object[0]);
//            }).Start();

//            return "";
//        }

//        public string Kind => "sr";
//        public string Usage => "mods.nimbusfox.foxcore.scriptrefresh.description";
//        public bool Public => false;
//    }
//}
