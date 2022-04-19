using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class ChatColorsWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new NuiBind<string>("buttonText");
        private readonly NuiBind<Color> color = new NuiBind<Color>("color");
        private readonly NuiBind<int> listCount = new NuiBind<int>("listCount");
        private readonly List<string> channelList = new List<string>()
        { "Parler - Joueur", "Parler - DM", "Murmurer - Joueur", "Murmurer - DM", "Groupe - Joueur", "Message privé - Joueur", "Emotes", "Correctif" };

        public ChatColorsWindow(Player player) : base(player)
        {
          windowId = "chatColors";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();

          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiColorPicker(color) { Width = windowRectangle.Width - 20 },
              new NuiSpacer()
            }
          });

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "set", Tooltip = "Applique la couleur sélectionnée à ce canal.", Height = 35 }) { Width = windowRectangle.Width - 120 },
            new NuiListTemplateCell(new NuiButton("Réinitialiser") { Id = "reinit", Tooltip = "Réinitialise ce canal à la couleur par défaut.", Height = 35 }),
          };

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });


          window = new NuiWindow(rootGroup, "Descriptions - Sélection")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleChatColorsEvents;
          player.oid.OnNuiEvent += HandleChatColorsEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          buttonText.SetBindValues(player.oid, token, channelList);
          listCount.SetBindValue(player.oid, token, channelList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleChatColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "set":

                  Color newColor = color.GetBindValue(player.oid, token);
                  int chatChannel = GetChatChannelFromIndex(nuiEvent.ArrayIndex);

                  if (player.chatColors.ContainsKey(chatChannel))
                    player.chatColors[chatChannel] = new byte[4] { newColor.Red, newColor.Green, newColor.Blue, newColor.Alpha };
                  else
                    player.chatColors.Add(chatChannel, new byte[4] { newColor.Red, newColor.Green, newColor.Blue, newColor.Alpha });

                  player.oid.SendServerMessage("Couleur de canal enregistrée.", ColorConstants.Orange);

                  break;

                case "reinit":
                  int chan = GetChatChannelFromIndex(nuiEvent.ArrayIndex);

                  if(player.chatColors.ContainsKey(chan))
                   player.chatColors.Remove(chan);

                  player.oid.SendServerMessage("Couleur de canal réinitialisée.", ColorConstants.Orange);

                  break;
              }
              break;
          }
        }
        private int GetChatChannelFromIndex(int index)
        {
          switch (index)
          {
            case 2:
              return (int)ChatChannel.DmTalk;
            case 3:
              return (int)ChatChannel.PlayerWhisper;
            case 4:
              return (int)ChatChannel.DmWhisper;
            case 5:
              return (int)ChatChannel.PlayerParty;
            case 6:
              return (int)ChatChannel.PlayerTell;
            case 7:
              return (int)ChatChannel.DmShout;
            case 8:
              return 100; // emotes
            case 9:
              return 101; // correctif
            default:
              return (int)ChatChannel.PlayerTalk;
          }
        }
      }
    }
  }
}
