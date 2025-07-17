using System.Collections.Generic;

using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class CustomDiceRolls : PlayerWindow
      {
        private readonly NuiColumn rootRow = new();
        private readonly List<NuiElement> rootChildren = new();

        private readonly NuiBind<string> dice = new ("dice");
        private readonly NuiBind<string> nbDices = new ("nbDices");

        public CustomDiceRolls(Player player) : base(player)
        {
          windowId = "customDiceRolls";

          rootRow.Children = rootChildren;

          CreateWindow();
        }
        public void CreateWindow()
        {
          windowWidth = player.guiScaledWidth * 0.15f;
          windowHeight = player.guiScaledHeight * 0.1f;

          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(0, 0, windowWidth, windowHeight);

          rootChildren.Clear();

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() 
          { 
            new NuiSpacer(),
            new NuiTextEdit("", nbDices, 2, false) { Width = windowWidth / 6, Height = windowWidth / 6 },
            new NuiLabel("d") { Width = windowWidth / 12, Height = windowWidth / 6, VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center },
            new NuiTextEdit("", dice, 3, false) { Width = windowWidth / 5, Height = windowWidth / 6 },
            new NuiSpacer(),
            new NuiButtonImage("diceRoll") { Id = "roll", Width = windowWidth / 6, Height = windowWidth / 6, Tooltip = "Lancer les dés" },
            new NuiSpacer()
          } });

          window = new NuiWindow(rootRow, "Jets de dés personnalisés")
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

            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
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
                case "roll":

                  if(!int.TryParse(dice.GetBindValue(player.oid, nuiToken.Token), out int selectedDice) || selectedDice < 0)
                  {
                    player.oid.SendServerMessage("Veuillez saisir un type de dé valide.", ColorConstants.Red);
                    return;
                  }

                  if (!int.TryParse(nbDices.GetBindValue(player.oid, nuiToken.Token), out int selectedNBDices) || selectedNBDices < 0)
                    selectedNBDices = 1;

                  StringUtils.BroadcastRollToPlayersInRange(player.oid.ControlledCreature, $"{player.oid.ControlledCreature.Name.ColorString(ColorConstants.Cyan)} - {StringUtils.ToWhitecolor(selectedNBDices)}d{StringUtils.ToWhitecolor(selectedDice)} = {StringUtils.ToWhitecolor(Utils.Roll(selectedDice, selectedNBDices))}", ColorConstants.Orange);

                  break;
              }

              break;
          }
        }
      }
    }
  }
}
