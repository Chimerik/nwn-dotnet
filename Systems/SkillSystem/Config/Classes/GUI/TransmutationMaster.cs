using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class TransmutationMasterWindow : PlayerWindow
      {
        private readonly string[] icons = new string[] { "tm_Jouvence", "tm_Panacea", "tm_Restoration", "tm_TransMajor" };
        private readonly string[] names = new string[] { "Jouvence", "Panacée", "Restitution", "Transformation Majeure" };
        private readonly bool[] effect = new bool[] { false, false, false, false };

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> abilityName = new("abilityName");
        private readonly NuiBind<bool> isChecked = new("isChecked");

        private NwItem stone;
        private int choice;

        public TransmutationMasterWindow(Player player) : base(player)
        {
          windowId = "transmutationMasterChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new()
          {
            new(new NuiSpacer()),
            new(new NuiButtonImage(icon)) { Width = 40 },
            new(new NuiLabel(abilityName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new(new NuiSpacer()),
            new(new NuiCheck("", isChecked) { Tooltip = "Sélectionnez l'effet de consumation de votre pierre", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };
          
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, 6) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Sélectionner la cible") { Id = "validate", Width = 80 }, new NuiSpacer() } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          stone = NwObject.FindObjectsWithTag<NwItem>("PierredeTransmutation").FirstOrDefault(s => s.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value == player.characterId);

          if(stone is null || stone.RootPossessor != player.oid.LoginCreature)
          {
            player.oid.SendServerMessage("Vous n'êtes pas en possession de votre pierre de transmutation", ColorConstants.Red);
            return;
          }

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.3f, player.guiHeight * 0.25f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Maître Transmutateur - Effet")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = false,
            Transparent = true,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleTransmutationMaitreEvents;

            LoadAbilityList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleTransmutationMaitreEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (stone is null | !stone.IsValid || stone.RootPossessor != player.oid.LoginCreature)
          {
            player.oid.SendServerMessage("La pierre en question n'existe plus ou n'est plus en votre possession", ColorConstants.Red);
            CloseWindow();
            return;
          }

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "validate":

                  player.oid.SendServerMessage("Veuillez sélectionner une cible", ColorConstants.Orange);
                  player.oid.EnterTargetMode(SelectTarget, Config.selectObjectTargetMode);

                  return;
              }

              break;

            case NuiEventType.Watch:

              switch (nuiEvent.ElementId)
              {
                case "isChecked":

                  for (int i = 0; i < effect.Length; i++)
                    effect[i] = nuiEvent.ArrayIndex == i;

                  choice = nuiEvent.ArrayIndex;

                  LoadAbilityList();

                  break;
              }

              break;

          }
        }
        private void LoadAbilityList()
        {
          isChecked.SetBindWatch(player.oid, nuiToken.Token, false);

          icon.SetBindValues(player.oid, nuiToken.Token, icons);
          abilityName.SetBindValues(player.oid, nuiToken.Token, names);
          isChecked.SetBindValues(player.oid, nuiToken.Token, effect);

          isChecked.SetBindWatch(player.oid, nuiToken.Token, true);
        }
        private void SelectTarget(ModuleEvents.OnPlayerTarget selection)
        {
          if (selection.IsCancelled)
            return;

          stone.Destroy();
          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, $"{EffectSystem.TransmutationStoneEffectTag}{player.characterId}");          
          StringUtils.DisplayStringToAllPlayersNearTarget(player.oid.LoginCreature, $"{player.oid.LoginCreature.Name.ColorString(ColorConstants.Cyan)} utilise " +
            $"{names[choice].ColorString(ColorConstants.White)} sur {selection.TargetObject.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

          CloseWindow();
        }
      }
    }
  }
}
