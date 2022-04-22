using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class QuickbarsWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChidren;
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> quickbarName = new ("quickbarName");
        private readonly NuiBind<bool> saveQuickbarEnabled = new ("saveQuickbarEnabled");
        private readonly List<string> quickbarNamesList = new List<string>();

        public QuickbarsWindow(Player player) : base(player)
        {
          windowId = "quickbars";

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiTextEdit("Nom d'une nouvelle barre de raccourcis", quickbarName, 40, false) { Width = 300, Tooltip = "Afin d'enregistrer une nouvelle barre de raccourcis, un nom doit être renseigné." },
              new NuiButton("Enregistrer") { Id = "new", Width = 80, Enabled = saveQuickbarEnabled, Tooltip = "Enregistre une nouvelle barre avec vos raccourcis tels qu'actuellement configurés." }
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

          window = new NuiWindow(rootGroup, "Barres de raccourcis - Sélection")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleQuickbarEvents;
          player.oid.OnNuiEvent += HandleQuickbarEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          foreach (Quickbar quickbar in player.quickbars)
            quickbarNamesList.Add(quickbar.name);

          buttonText.SetBindValues(player.oid, token, quickbarNamesList);
          listCount.SetBindValue(player.oid, token, quickbarNamesList.Count);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          quickbarName.SetBindValue(player.oid, token, "");
          quickbarName.SetBindWatch(player.oid, token, true);

          saveQuickbarEnabled.SetBindValue(player.oid, token, false);

          player.openedWindows[windowId] = token;
        }
        private void HandleQuickbarEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "new":

                  SaveQuickbar(quickbarName.GetBindValue(player.oid, token));
                  quickbarName.SetBindValue(player.oid, token, "");
                  player.oid.SendServerMessage("Barre de raccourcis sauvegardée.", ColorConstants.Orange);

                  break;

                case "delete":

                  DeleteQuickbar(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Barre de raccourcis supprimée.", ColorConstants.Orange);

                  break;

                case "load":

                  LoadQuickbar(nuiEvent.ArrayIndex);
                  player.oid.SendServerMessage("Barre de raccourcis chargée.", ColorConstants.Orange);

                  break;

              }
              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "quickbarName":

                  if (quickbarName.GetBindValue(player.oid, token).Length > 0)
                    saveQuickbarEnabled.SetBindValue(player.oid, token, true);
                  else
                    saveQuickbarEnabled.SetBindValue(player.oid, token, false);

                  break;

              }

              break;
          }
        }
        private void SaveQuickbar(string quickbarName)
        {
          player.quickbars.Add(new Quickbar(quickbarName, player.oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString()));

          quickbarNamesList.Add(quickbarName);
          buttonText.SetBindValues(player.oid, token, quickbarNamesList);
          listCount.SetBindValue(player.oid, token, quickbarNamesList.Count);
        }

        private void DeleteQuickbar(int index)
        {
          player.grimoires.RemoveAt(index);
          quickbarNamesList.RemoveAt(index);
          buttonText.SetBindValues(player.oid, token, quickbarNamesList);
          listCount.SetBindValue(player.oid, token, quickbarNamesList.Count);
        }

        private void LoadQuickbar(int index)
        {
          player.oid.ControlledCreature.DeserializeQuickbar(player.quickbars[index].serializedQuickbar.ToByteArray());
        }
      }
    }
  }
}
