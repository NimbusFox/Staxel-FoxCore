using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plukit.Base;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Player;

namespace NimbusFox.FoxCore.V3.Interfaces {
    public interface IFoxModHook : IModHookV3 {
        void OnPlayerLoadAfter(Blob blob);
        void OnPlayerSaveBefore(PlayerEntityLogic logic, out Blob saveBlob);
        void OnPlayerSaveAfter(PlayerEntityLogic logic, out Blob saveBlob);
        void OnPlayerConnect(Entity entity);
        void OnPlayerDisconnect(Entity entity);
        void Store(Blob blob);
        void Restore(Blob blob);
    }
}
