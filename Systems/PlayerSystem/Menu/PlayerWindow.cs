using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public abstract class PlayerWindow
      {
        protected string windowId { get; }
        protected NuiBind<bool> closable { get; }
        protected NuiBind<bool> resizable { get; }
        protected NuiBind<NuiRect> geometry { get; }
        protected Player player { get; }
        protected NuiWindow window { get; set; }
        protected int token { get; set; }

        public PlayerWindow(Player player, string windowId)
        {
          this.windowId = windowId;
          this.player = player;
          closable = new NuiBind<bool>("closable");
          resizable = new NuiBind<bool>("resizable");
          geometry = new NuiBind<NuiRect>("geometry");

          // TODO : à la création initiale de la fenêtre, on charge la geometry à partir des infos de la BDD

        }
      }
    }
  }
}
