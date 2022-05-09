using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public abstract class PlayerWindow
      {
        protected string windowId { get; set; }
        protected NuiBind<bool> closable { get; }
        protected NuiBind<bool> resizable { get; }
        protected NuiBind<NuiRect> geometry { get; }
        protected Player player { get; }
        protected NuiWindow window { get; set; }
        public int token { get; set; }
        public NuiWindowToken nuiToken { get; set; }

        public PlayerWindow(Player player)
        {
          this.player = player;
          token = -1;
          closable = new ("closable");
          resizable = new ("resizable");
          geometry = new NuiBind<NuiRect>("geometry");
        }

        protected void OnAreaChangeCloseWindow(OnServerSendArea onArea)
        {
          CloseWindow();
        }

        public void CloseWindow()
        {
          player.oid.NuiDestroy(nuiToken.Token);
          player.openedWindows.Remove(windowId);
        }
      }
    }
  }
}
