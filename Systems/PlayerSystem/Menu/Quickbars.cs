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
        private readonly List<NuiElement> rootChidren = new List<NuiElement>();
        private readonly NuiBind<string> buttonText = new ("buttonText");
        private readonly NuiBind<int> listCount = new ("listCount");
        private readonly NuiBind<string> quickbarName = new ("quickbarName");
        private readonly NuiBind<bool> saveQuickbarEnabled = new ("saveQuickbarEnabled");
        private readonly List<string> quickbarNamesList = new();

        public QuickbarsWindow(Player player) : base(player)
        {
          windowId = "quickbars";

          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          rootChidren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiTextEdit("Nom d'une nouvelle barre de raccourcis", quickbarName, 40, false) { Width = 300, Height = 35, Tooltip = "Afin d'enregistrer une nouvelle barre de raccourcis, un nom doit être renseigné." },
            new NuiButtonImage("ir_empytqs") { Id = "new", Width = 35, Height = 35, Enabled = saveQuickbarEnabled, Tooltip = "Enregistre une nouvelle barre avec vos raccourcis tels qu'actuellement configurés." }
          } });

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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleQuickbarEvents;

            foreach (Quickbar quickbar in player.quickbars)
              quickbarNamesList.Add(quickbar.name);

            buttonText.SetBindValues(player.oid, nuiToken.Token, quickbarNamesList);
            listCount.SetBindValue(player.oid, nuiToken.Token, quickbarNamesList.Count);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            quickbarName.SetBindValue(player.oid, nuiToken.Token, "");
            quickbarName.SetBindWatch(player.oid, nuiToken.Token, true);

            saveQuickbarEnabled.SetBindValue(player.oid, nuiToken.Token, false);
          } 
        }
        private void HandleQuickbarEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "new":

                  SaveQuickbar(quickbarName.GetBindValue(player.oid, nuiToken.Token));
                  quickbarName.SetBindValue(player.oid, nuiToken.Token, "");
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

                  if (quickbarName.GetBindValue(player.oid, nuiToken.Token).Length > 0)
                    saveQuickbarEnabled.SetBindValue(player.oid, nuiToken.Token, true);
                  else
                    saveQuickbarEnabled.SetBindValue(player.oid, nuiToken.Token, false);

                  break;

              }

              break;
          }
        }
        private void SaveQuickbar(string quickbarName)
        {
          player.quickbars.Add(new Quickbar(quickbarName, player.oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString()));

          quickbarNamesList.Add(quickbarName);
          buttonText.SetBindValues(player.oid, nuiToken.Token, quickbarNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, quickbarNamesList.Count);
        }

        private void DeleteQuickbar(int index)
        {
          player.grimoires.RemoveAt(index);
          quickbarNamesList.RemoveAt(index);
          buttonText.SetBindValues(player.oid, nuiToken.Token, quickbarNamesList);
          listCount.SetBindValue(player.oid, nuiToken.Token, quickbarNamesList.Count);
        }

        private void LoadQuickbar(int index)
        {
          player.oid.ControlledCreature.DeserializeQuickbar(player.quickbars[index].serializedQuickbar.ToByteArray());
        }
      }
    }
  }
}
