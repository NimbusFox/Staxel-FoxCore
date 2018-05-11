using System;
using Plukit.Base;
using Staxel.Commands;
using Staxel.Server;

namespace NimbusFox.FoxCore.Interpreters.Classes {
    internal class CommandBuilder {
        public Func<string[], Blob, ClientServerConnection, ICommandsApi, CommandOutput> Execute { get; set; }
        
        public string Usage { get; set; }
        public bool Public { get; set; }
    }

    internal class CommandOutput {
        public string TranslationCode { get; set; }
        public object[] ResponseParams { get; set; }
    }
}
