using Plukit.Base;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Player;

namespace NimbusFox.FoxCore {
    public interface IFoxModHookV3 : IModHookV3 {
        void OnPlayerLoadAfter(Blob blob);
        void OnPlayerSaveBefore(PlayerEntityLogic logic, out Blob saveBlob);
        void OnPlayerSaveAfter(PlayerEntityLogic logic, out Blob saveBlob);
        void OnPlayerConnect(Entity entity);
        void OnPlayerDisconnect(Entity entity);
    }
}