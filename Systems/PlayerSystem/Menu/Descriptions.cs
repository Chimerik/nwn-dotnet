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
        private readonly NuiBind<string> buttonText = new("buttonText");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly List<string> descriptionNamesList = new();

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
            new NuiListTemplateCell(new NuiButton(buttonText) { Id = "load", Height = 35 }) { VariableSize = true },
            new NuiListTemplateCell(new NuiButtonImage("ir_ban") { Id = "delete", Height = 35 }) { Width = 35 },
          };
          rootChidren.Add(new NuiList(rowTemplate, listCount) { RowHeight = 35 });

          CreateWindow();
        }
        public void CreateWindow()
        {
          descriptionNamesList.Clear();

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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleDescriptionsEvents;

            foreach (CharacterDescription description in player.descriptions)
              descriptionNamesList.Add(description.name);

            buttonText.SetBindValues(player.oid, nuiToken.Token, descriptionNamesList);
            listCount.SetBindValue(player.oid, nuiToken.Token, descriptionNamesList.Count);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleDescriptionsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "new":

                  if (!player.windows.ContainsKey("descriptionContent")) player.windows.Add("descriptionContent", new DescriptionContentWindow(player));
                  else ((DescriptionContentWindow)player.windows["descriptionContent"]).CreateWindow();

                  CloseWindow();

                  break;

                case "delete":

                  DeleteDescription(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Description supprimée.", ColorConstants.Orange);

                  break;

                case "load":

                  if (!player.windows.ContainsKey("descriptionContent")) player.windows.Add("descriptionContent", new DescriptionContentWindow(player, player.descriptions[nuiEvent.ArrayIndex]));
                  else ((DescriptionContentWindow)player.windows["descriptionContent"]).CreateWindow(player.descriptions[nuiEvent.ArrayIndex]);

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
          buttonText.SetBindValues(player.oid, nuiToken.Token, descriptionNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, descriptionNamesList.Count);

          if (player.TryGetOpenedWindow("descriptionContent", out PlayerWindow descriptionWindow))
            descriptionWindow.CloseWindow();
        }
      }
    }
  }
}
