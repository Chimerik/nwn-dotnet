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
      public class BankCounterWindow : PlayerWindow
      {
        private readonly NuiGroup rootGroup;
        private readonly NuiColumn rootColumn;
        private readonly List<NuiElement> rootChildren = new List<NuiElement>();
        private readonly NuiRow introTextRow;
        private readonly NuiRow contractRow;
        private readonly NuiRow exitRow;
        private readonly NuiRow auctionHouseRow;
        private readonly NuiRow mailBoxRow;
        private readonly NuiText widgetBankerText;
        private readonly NuiBind<string> bankerText;
        private readonly NwCreature banker;

        public BankCounterWindow(Player player, NwCreature banker) : base(player)
        {
          windowId = "bankCounter";
          this.banker = banker;

          bankerText = new("bankerText");

          rootColumn = new NuiColumn() { Children = rootChildren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          widgetBankerText = new NuiText(bankerText) { Width = 450, Height = 250 };

          introTextRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiImage(banker.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100 },
              widgetBankerText
            }
          };

          contractRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Laissez moi jeter un oeil à ce contrat.") { Id = "contract", Width = 510 } } };
          exitRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("Merci, je voulais seulement vous saluer.") { Id = "exit", Width = 510 } } };
          auctionHouseRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("J'aimerais accéder à l'hôtel des ventes.") { Id = "auctionHouse", Width = 510 } } };
          mailBoxRow = new NuiRow() { Children = new List<NuiElement>() { new NuiButton("J'aimerais consulter les services de boîte aux lettres.") { Id = "mailBox", Width = 510 } } };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChildren.Clear();
          rootChildren.Add(introTextRow);
          string tempText = "";
          int windowSize = 350;

          if (NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").Any(b => b.GetObjectVariable<LocalVariableInt>("ownerId").Value == player.characterId))
          {
            tempText = $"Bonjour, bonjour, cher {player.oid.ControlledCreature.Name}, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Très estimé client, votre coffre vous attend comme de coutume.\n\n" +
              $"Y a-t-il autre chose pour votre service ?";

            widgetBankerText.Height = 140;

            rootChildren.Add(auctionHouseRow);
            rootChildren.Add(mailBoxRow);
          }
          else
          {
            rootChildren.Add(contractRow);

            tempText = $"Bonjour, bonjour, très estimé prospect, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Je ne crois pas que nous ayons l'honneur de vous compter parmis nos clients.\n\n" +
              $"Souhaiteriez-vous ouvrir un compte et bénéficier de nos services ?\n\n" +
              $"Rien de plus simple, il suffit de parapher le contrat d'ouverture de compte et le tour sera joué !";

            widgetBankerText.Height = 260;
            windowSize = 450;
          }

          rootChildren.Add(exitRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 550, windowSize);

          window = new NuiWindow(rootGroup, "Ernesto Arna")
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
            nuiToken.OnNuiEvent += HandleBankCounterEvents;

            bankerText.SetBindValue(player.oid, nuiToken.Token, tempText);

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleBankCounterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "contract":

                  CloseWindow();

                  if (!player.oid.LoginCreature.Inventory.Items.Any(i => i.Tag == "bank_contract"))
                    banker.Inventory.Items.FirstOrDefault(i => i.Tag == "bank_contract").Clone(player.oid.LoginCreature);
                  else
                  {
                    if (!player.windows.ContainsKey("bankContract")) player.windows.Add("bankContract", new BankContractWindow(player, player.oid.LoginCreature.Inventory.Items.FirstOrDefault(i => i.Tag == "bank_contract")));
                    else ((BankContractWindow)player.windows["bankContract"]).CreateWindow();
                  }

                  break;

                case "auctionHouse":

                  if (!player.windows.ContainsKey("auctionHouse")) player.windows.Add("auctionHouse", new AuctionHouseWindow(player));
                  else ((AuctionHouseWindow)player.windows["auctionHouse"]).CreateWindow();

                  CloseWindow();

                  break;

                case "mailBox": LoadServicesLayout(); break;

                case "notify":

                  if(player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailNotification))
                  {
                    Subscription notifSub = player.subscriptions.FirstOrDefault(s => s.type == Utils.SubscriptionType.MailNotification);
                    player.bankGold -= notifSub.fee;
                    player.subscriptions.Remove(notifSub);

                    player.oid.SendServerMessage($"Votre résiliation au service des pièces jumelles a bien été prise en compte. {StringUtils.ToWhitecolor(notifSub.fee)} pièces ont été prélevés de votre compte Skarlsgard.", ColorConstants.Orange);
                  }
                  else
                  {
                    player.subscriptions.Add(new Subscription(Utils.SubscriptionType.MailNotification, 300, DateTime.Now.AddDays(30), 30));
                    player.bankGold -= 300;
                    player.oid.SendServerMessage($"Votre souscription au service des pièces jumelles a bien été prise en compte. {StringUtils.ToWhitecolor(300)} pièces seront prélevées tous les mois de votre compte Skarlsgard.", ColorConstants.Orange);
                  }

                  LoadServicesLayout();

                  break;

                case "corbak":

                  if (player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailDistantAccess))
                  {
                    Subscription notifSub = player.subscriptions.FirstOrDefault(s => s.type == Utils.SubscriptionType.MailDistantAccess);
                    player.bankGold -= notifSub.fee;
                    player.subscriptions.Remove(notifSub);

                    player.oid.SendServerMessage($"Votre résiliation au service de corbeaux voyageurs a bien été prise en compte. {StringUtils.ToWhitecolor(notifSub.fee)} pièces ont été prélevés de votre compte Skarlsgard.", ColorConstants.Orange);
                  }
                  else
                  {
                    player.subscriptions.Add(new Subscription(Utils.SubscriptionType.MailDistantAccess, 1500, DateTime.Now.AddDays(30), 30));
                    player.bankGold -= 1500;
                    player.oid.SendServerMessage($"Votre souscription au service des pièces jumelles a bien été prise en compte. {StringUtils.ToWhitecolor(1500)} pièces seront prélevées tous les mois de votre compte Skarlsgard.", ColorConstants.Orange);
                  }

                  LoadServicesLayout();

                  break;

                case "exit":
                  CloseWindow();
                  break;
              }
              break;
          }
        }
        private void LoadServicesLayout()
        {
          rootChildren.Clear();

          bankerText.SetBindValue(player.oid, nuiToken.Token, $"Mais bien entendu très honoré client, à quel service souhaitez-vous souscrire ?\n\n" +
            $"Pour toute souscription, le paiement s'effectue tous les 30 jours.\n\n" +
            $"En cas de résiliation, le paiment du mois en cours est immédiat.");

          string notifTitle = player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailNotification) ? "Résilier les pièces jumelles de notification" : "Souscrire aux pièces jumelles de notification";
          string corbakTitle = player.subscriptions.Any(s => s.type == Utils.SubscriptionType.MailDistantAccess) ? "Résilier les corbeaux voyageurs" : "Souscrire au service de corbeaux voyageurs";

          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton(notifTitle) { Id = "notify", Tooltip = "Coût : 300 pièces par mois", Width = 510 } } });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiButton(corbakTitle) { Id = "corbak", Tooltip = "Coût : 1500 pièces par mois", Width = 510 } } });
          rootGroup.SetLayout(player.oid, nuiToken.Token, rootColumn);
        }
      }
    }
  }
}
