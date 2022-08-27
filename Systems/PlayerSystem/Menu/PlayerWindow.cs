﻿using Anvil.API;
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
        protected readonly NuiBind<bool> closable = new("closable");
        protected readonly NuiBind<bool> resizable = new("resizable");
        protected readonly NuiBind<NuiRect> geometry = new("geometry");
        protected readonly Player player;
        protected NuiWindow window { get; set; }
        public NuiWindowToken nuiToken { get; set; }
        public bool IsOpen { get; set; }
        protected NuiRect rectangle { get; set; }

        public PlayerWindow(Player player)
        {
          this.player = player;
          //token = -1;
        }

        protected void OnAreaChangeCloseWindow(OnServerSendArea _)
        {
          player.oid.OnServerSendArea -= OnAreaChangeCloseWindow;
          CloseWindow();
        }

        public void CloseWindow()
        {
          nuiToken.Close();
          IsOpen = false;
        }
        public void ResizeWidgets()
        {
          switch(windowId)
          {
            case "bodyAppearanceModifier":
              CloseWindow();
              ((BodyAppearanceWindow)this).CreateWindow(((BodyAppearanceWindow)this).targetCreature);
              break;
          }
        }
      }
    }
  }
}
