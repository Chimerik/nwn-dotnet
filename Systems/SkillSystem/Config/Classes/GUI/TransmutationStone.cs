using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class TransmutationStoneWindow : PlayerWindow
      {
        private readonly string[] icons = new string[] { "ts_Speed", "is_TransCons", "ts_Darkvision", "ts_AcidRes", "ts_ColdRes", "ts_FireRes", "ts_ElecRes", "ts_ThunderRes" };
        private readonly string[] names = new string[] { "+30% Vitesse", "Maîtrise JDS constitution", "Vision dans le noir", "Résistance à l'acide", "Résistance au froid", "Résistance au feu", "Résistance à l'électricité", "Résistance à  la foudre" };
        private readonly bool[] effect = new bool[] { false, false, false, false, false, false, false, false };

        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> icon = new("icon");
        private readonly NuiBind<string> abilityName = new("abilityName");
        private readonly NuiBind<bool> isChecked = new("isChecked");

        private NwItem stone;
        private int choice;

        public TransmutationStoneWindow(Player player, NwItem stone) : base(player)
        {
          windowId = "transmutationStoneChoice";
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> abilitiesTemplate = new()
          {
            new(new NuiSpacer()),
            new(new NuiButtonImage(icon)) { Width = 40 },
            new(new NuiLabel(abilityName) { HorizontalAlign = NuiHAlign.Center, VerticalAlign = NuiVAlign.Middle }) { Width = 220 },
            new(new NuiSpacer()),
            new(new NuiCheck("", isChecked) { Tooltip = "Sélectionnez l'effet de votre pierre de transmutation", Margin = 0.0f }) { Width = 40 },
            new(new NuiSpacer())
          };
          
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiList(abilitiesTemplate, 6) { RowHeight = 40, Scrollbars = NuiScrollbars.None } } });
          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = 35, Children = new List<NuiElement>() { new NuiSpacer(), new NuiButton("Valider") { Id = "validate", Width = 80 }, new NuiSpacer() } });

          CreateWindow(stone);
        }
        public void CreateWindow(NwItem stone)
        {
          if(player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).HasNothing)
          {
            player.oid.SendServerMessage("Vous devez lancer un sort de transmutation de niveau 1+ pour pouvoir changer l'effet de votre pierre");
            return;
          }

          this.stone = stone;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(player.guiScaledWidth * 0.3f, player.guiHeight * 0.25f, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f);

          window = new NuiWindow(rootColumn, "Pierre de transmutation - Effet")
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
            nuiToken.OnNuiEvent += HandleTransmutationStoneEvents;

            LoadAbilityList();

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, player.guiScaledWidth * 0.4f, player.guiScaledHeight * 0.55f));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleTransmutationStoneEvents(ModuleEvents.OnNuiEvent nuiEvent)
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

                  EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, $"{EffectSystem.TransmutationStoneEffectTag}{player.characterId}");

                  player.oid.SendServerMessage($"Votre pierre aura désormais l'effet {names[choice]}");
                  stone.GetObjectVariable<LocalVariableInt>("_TRANSMUTER_STONE_CHOICE").Value = choice;
                  player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetTransmutationStoneEffect(player.oid.LoginCreature, stone, choice));
                  player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Delete();

                  CloseWindow();

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
      }
    }
  }
}
