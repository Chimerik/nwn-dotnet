using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void CreateChatWindow()
      {
        NuiBind<string> writingChat = new NuiBind<string>("writingChat");
        NuiBind<string> receivedChat = new NuiBind<string>("receivedChat");
        NuiBind<int> channel = new NuiBind<int>("channel");
        NuiBind<bool> closable = new NuiBind<bool>("closable");
        NuiBind<bool> resizable = new NuiBind<bool>("resizable");
        NuiBind<bool> makeStatic = new NuiBind<bool>("static");
        NuiBind<NuiRect> geometry = new NuiBind<NuiRect>("geometry");
        NuiRect windowRectangle = windowRectangles.ContainsKey("chat") ? windowRectangles["chat"] : new NuiRect(420.0f, 10.0f, 600.0f, 400.0f);

        List<NuiComboEntry> comboValues = new List<NuiComboEntry>
          {
            new NuiComboEntry("Parler", 1),
            new NuiComboEntry("Chuchoter", 3),
            new NuiComboEntry("Groupe", 6),
            new NuiComboEntry("MD", 14),
            new NuiComboEntry("Crier", 2)
          };

        // Construct the window layout.
        NuiColumn root = new NuiColumn
        {
          Children = new List<NuiElement>
        {
          new NuiRow
          {
            Children = new List<NuiElement>
            {
              new NuiCombo
              {
                Entries = comboValues,
                Selected = channel
              },
              new NuiSpacer(),
              new NuiCheck("Figer", makeStatic) { Id = "fix", Tooltip = "Permet d'ancrer la fenêtre à l'écran" }
            }
          },
          new NuiRow
          {
            Children = new List<NuiElement>
            {
              new NuiText(receivedChat) { }
            }
          },
          new NuiRow
          {
            Children = new List<NuiElement>
            {
              new NuiTextEdit("", writingChat, 3000, true) { Height = windowRectangle.Height * 0.1f, Id = "chatWriter" },
              new NuiButton("Envoyer") { Height = windowRectangle.Height * 0.1f, Width = 60, Id = "send" }
            }
          }
        }
        };

        NuiWindow window = new NuiWindow(root, "")
        {
          Geometry = geometry,
          Resizable = resizable,
          Collapsed = false,
          Closable = closable,
          Transparent = true,
          Border = false,
        };

        int token = oid.CreateNuiWindow(window, "chat");

        receivedChat.SetBindValue(oid, token, "");
        writingChat.SetBindValue(oid, token, "");
        channel.SetBindValue(oid, token, 0);
        makeStatic.SetBindValue(oid, token, false);
        resizable.SetBindValue(oid, token, true);
        closable.SetBindValue(oid, token, true);
        writingChat.SetBindWatch(oid, token, true);

        geometry.SetBindValue(oid, token, windowRectangle);
        geometry.SetBindWatch(oid, token, true);
      }
    }
  }
}
