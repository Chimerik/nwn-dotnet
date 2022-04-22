using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class DescriptionsWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly List<string> descriptionNamesList = new List<string>();

        public DescriptionsWindow(Player player) : base(player)
        {
          windowId = "descriptions";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Nouvelle description") { Id = "new", Width = 160, Tooltip = "Enregistre une nouvelle description pour votre personnage." },
              new NuiSpacer()
            }
          });

          List<NuiListTemplateCell> rowTemplate = new List<NuiListTemplateCell>
          {
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "load", Height = 35 }) { Width = 300 },
            new NuiListTemplateCell(new NuiButton("Supprimer") { Id = "delete", Height = 35 }) { Width = 60 },
          };
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.ContainsKey(windowId) ? player.windowRectangles[windowId] : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 450, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.4f);

          window = new NuiWindow(rootGroup, "Descriptions - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleDescriptionsEvents;
          player.oid.OnNuiEvent += HandleDescriptionsEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          foreach (CharacterDescription description in player.descriptions)
            descriptionNamesList.Add(description.name);

          buttonText.SetBindValues(player.oid, token, descriptionNamesList);
          listCount.SetBindValue(player.oid, token, descriptionNamesList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private void HandleDescriptionsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "new":

                  if (player.windows.ContainsKey("descriptionContent"))
                    ((DescriptionContentWindow)player.windows["descriptionContent"]).CreateWindow();
                  else
                    player.windows.Add("descriptionContent", new DescriptionContentWindow(player));

                  CloseWindow();

                  break;

                case "delete":

                  DeleteDescription(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Description supprimée.", ColorConstants.Orange);

                  break;

                case "load":

                  if (player.windows.ContainsKey("descriptionContent"))
                    ((DescriptionContentWindow)player.windows["descriptionContent"]).CreateWindow(player.descriptions[nuiEvent.ArrayIndex]);
                  else
                    player.windows.Add("descriptionContent", new DescriptionContentWindow(player, player.descriptions[nuiEvent.ArrayIndex]));

                  CloseWindow();

                  break;

              }
              break;
          }
        }
        private void DeleteDescription(int index)
        {
          player.descriptions.RemoveAt(index);
          descriptionNamesList.RemoveAt(index);
          buttonText.SetBindValues(player.oid, token, descriptionNamesList);
          listCount.SetBindValue(player.oid, token, descriptionNamesList.Count);

          if (player.openedWindows.ContainsKey("descriptionContent"))
            player.windows["descriptionContent"].CloseWindow();
        }
      }
    }
  }
}
