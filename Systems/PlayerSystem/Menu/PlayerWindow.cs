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

        public PlayerWindow(Player player)
        {
          this.player = player;
          token = -1;
          closable = new NuiBind<bool>("closable");
          resizable = new NuiBind<bool>("resizable");
          geometry = new NuiBind<NuiRect>("geometry");

          // TODO : à la création initiale de la fenêtre, on charge la geometry à partir des infos de la BDD

        }

        protected void OnAreaChangeCloseWindow(OnServerSendArea onArea)
        {
          CloseWindow();
        }

        public void CloseWindow()
        {
          player.oid.NuiDestroy(token);
          player.openedWindows.Remove(windowId);
        }
      }
    }
  }
}
