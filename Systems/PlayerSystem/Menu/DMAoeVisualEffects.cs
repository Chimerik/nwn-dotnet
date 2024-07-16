using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Newtonsoft.Json;

using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class DMAoEVisualEffectsWindow : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();

        private readonly NuiBind<string> vfxDurations = new("vfxDurations");
        private readonly NuiBind<string> vfxId = new("vfxId");

        private int selectedVFXId;
        private int selectedVFXDuration;

        public DMAoEVisualEffectsWindow(Player player) : base(player)
        {
          windowId = "DMAoEVisualEffects";

          rootRow.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiTextEdit("Id", vfxId, 5, false) { Width = 70, Height = 35, Tooltip = "Identifiant de l'effet (visualeffects.2da)" },
            new NuiTextEdit("Durée", vfxDurations, 4, false) { Width = 70, Height = 35, Tooltip = "Durée de l'effet en secondes" },
            new NuiButton("Tester") { Id = "test", Width = 60, Height = 35, Tooltip = "Essayer l'effet sur un objet ou une zone ciblée" },
          } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 400, 650);

          window = new NuiWindow(rootRow, "Gestion des effets visuels en AoE")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = closable,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleVisualEffectsEvents;

            vfxId.SetBindValue(player.oid, nuiToken.Token, "");

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandleVisualEffectsEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "test":

                  if (!int.TryParse(vfxId.GetBindValue(player.oid, nuiToken.Token), out selectedVFXId) || selectedVFXId < 0)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un identifiant d'effet visuel existant.", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(vfxDurations.GetBindValue(player.oid, nuiToken.Token), out selectedVFXDuration) || selectedVFXDuration < 0)
                    selectedVFXDuration = 10;

                  player.oid.SendServerMessage("Veuillez sélectionner une cible pour essayer votre effet visuel.", ColorConstants.Orange);
                  player.oid.EnterTargetMode(OnTargetSelected, Config.selectLocationTargetMode);

                  break;
              }
              break;
          }
        }
        private void OnTargetSelected(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled)
            return;

          var vfxRow = NwGameTables.VisualEffectTable.GetRow(selectedVFXId);

          if(vfxRow == null)
          {
            player.oid.SendServerMessage($"Aucun effet visuel ne correspond à l'entrée : {selectedVFXId.ToString().ColorString(ColorConstants.White)}", ColorConstants.Red);
            return;
          }

          if (selection.TargetObject is NwGameObject target)
            target.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect((PersistentVfxType)selectedVFXId), TimeSpan.FromSeconds(selectedVFXDuration));
          else
            Location.Create(selection.Player.ControlledCreature.Location.Area, selection.TargetPosition, selection.Player.ControlledCreature.Rotation)
              .ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect((PersistentVfxType)selectedVFXId), TimeSpan.FromSeconds(selectedVFXDuration));
        }
      }
    }
  }
}
