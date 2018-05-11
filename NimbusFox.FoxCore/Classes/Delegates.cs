using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;

namespace NimbusFox.FoxCore.Delegates {
    public delegate string Command<in TStringArray, in TBlob, in TClientServer, in TICommands, TObjectArray, out TString>(TStringArray array, TBlob blob, TClientServer clientServer, TICommands commandsApi, out TObjectArray objectArray);
}
