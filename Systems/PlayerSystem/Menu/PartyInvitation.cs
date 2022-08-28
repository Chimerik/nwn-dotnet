using System;
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
      public class PartyInvitationWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private Player inviter;

        public PartyInvitationWindow(Player player, Player inviter) : base(player)
        {
          windowId = "partyInvitation";
          rootColumn.Children = rootChildren;

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButtonImage("ir_ignore") { Id = "deny", Tooltip = "Refuser l'invitation", Height = 35, Width = 35 },
            new NuiSpacer() { Width = 100 },
            new NuiButtonImage("ir_accept") { Id = "accept", Tooltip = "Rejoindre le groupe", Height = 35, Width = 35 },
            new NuiSpacer()
          } });

          CreateWindow(inviter);
        }
        public void CreateWindow(Player inviter)
        {
          this.inviter = inviter;

          NuiRect windowRectangle = new NuiRect(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiWidth) / 2 - 200, 80, 400, 100);

          window = new NuiWindow(rootColumn, $"Invitation au groupe de {inviter.oid.LoginCreature.Name} ({inviter.oid.PlayerName})")
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
            nuiToken.OnNuiEvent += HandlePartyInvitationEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }

        private void HandlePartyInvitationEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "accept":

                  if (inviter.pcState != PcState.Offline)
                    player.oid.AddToParty(inviter.oid);
                  else
                    player.oid.SendServerMessage("Cette invitation n'est plus valable");

                  CloseWindow();

                  break;

                case "deny":

                  if(inviter.oid != null && inviter.oid.IsValid && inviter.oid.IsConnected)
                    inviter.oid.SendServerMessage($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) a refusé votre invitation.", ColorConstants.Orange);
                  
                  CloseWindow();

                  break;
              }

              break;
          }
        }
      }
    }
  }
}
