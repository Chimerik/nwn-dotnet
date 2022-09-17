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
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<Color> color = new ("color");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly List<string> channelList = new ()
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
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "set", Tooltip = "Applique la couleur sélectionnée à ce canal.", Height = 35 }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "reinit", Tooltip = "Réinitialise ce canal à la couleur par défaut.", Height = 35 }) { Width = 35},
          };

          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          window = new NuiWindow(rootGroup, "Couleurs de chat - Sélection")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleChatColorsEvents;

            buttonText.SetBindValues(player.oid, nuiToken.Token, channelList);
            listCount.SetBindValue(player.oid, nuiToken.Token, channelList.Count);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleChatColorsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "set":

                  Color newColor = color.GetBindValue(player.oid, nuiToken.Token);
                  int chatChannel = GetChatChannelFromIndex(nuiEvent.ArrayIndex);

                  if(!player.chatColors.TryAdd(chatChannel, new byte[4] { newColor.Red, newColor.Green, newColor.Blue, newColor.Alpha }))
                    player.chatColors[chatChannel] = new byte[4] { newColor.Red, newColor.Green, newColor.Blue, newColor.Alpha };

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
        private static int GetChatChannelFromIndex(int index)
        {
          return index switch
          {
            1 => (int)ChatChannel.DmTalk,
            2 => (int)ChatChannel.PlayerWhisper,
            3 => (int)ChatChannel.DmWhisper,
            4 => (int)ChatChannel.PlayerParty,
            5 => (int)ChatChannel.PlayerTell,
            //6 => (int)ChatChannel.DmShout,
            6 => 100,// emotes
            7 => 101,// correctif
            _ => (int)ChatChannel.PlayerTalk,
          };
        }
      }
    }
  }
}
