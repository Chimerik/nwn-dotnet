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
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }
        NuiRow introTextRow { get; }
        NuiRow contractRow { get; }
        NuiRow exitRow { get; }
        NuiText widgetBankerText { get; }
        NuiBind<string> bankerText { get; }

        public BankCounterWindow(Player player, NwCreature banker) : base(player)
        {
          windowId = "bankCounter";

          bankerText = new NuiBind<string>("bankerText");

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          widgetBankerText = new NuiText(bankerText) { Width = 450 };

          introTextRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiImage(banker.PortraitResRef + "M") { ImageAspect = NuiAspect.ExactScaled, Width = 60, Height = 100 },
              widgetBankerText
            }
          };

          contractRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Laissez moi jeter un oeil à ce contrat.") { Id = "contract", Width = 510 }
            }
          };

          exitRow = new NuiRow()
          {
            Children = new List<NuiElement>()
            {
              new NuiButton("Merci, je voulais seulement vous saluer.") { Id = "exit", Width = 510 }
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();
          rootChidren.Add(introTextRow);
          string tempText = "";

          if (NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").Any(b => b.GetObjectVariable<LocalVariableInt>("ownerId").Value == player.characterId))
          {
            tempText = $"Bonjour, bonjour, cher {player.oid.ControlledCreature.Name}, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Très estimé client, votre coffre vous attend comme de coutume.\n\n" +
              $"Y a-t-il autre chose pour votre service ?";

            widgetBankerText.Height = 110;
          }
          else
          {
            rootChidren.Add(contractRow);

            tempText = $"Bonjour, bonjour, très estimé prospect, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Je ne crois pas que nous ayons l'honneur de vous compter parmis nos clients.\n\n" +
              $"Souhaiteriez-vous ouvrir un compte et bénéficier de nos services ?\n\n" +
              $"Rien de plus simple, il suffit de parapher le contrat d'ouverture de compte et le tour sera joué !";

            widgetBankerText.Height = 140;
          }

          rootChidren.Add(exitRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 340);

          window = new NuiWindow(rootGroup, "Ernesto Arna")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleBankCounterEvents;
          player.oid.OnNuiEvent += HandleBankCounterEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          bankerText.SetBindValue(player.oid, token, tempText);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private async void HandleBankCounterEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "contract":

                  player.oid.NuiDestroy(token);

                  if (!player.oid.LoginCreature.Inventory.Items.Any(i => i.Tag == "bank_contract"))
                  {
                    NwItem contract = await NwItem.Create("shop_clearance", player.oid.LoginCreature);
                    contract.Tag = "bank_contract";
                    contract.Name = "Contrat d'ouverture de compte Skalsgard";
                    contract.Description = "Le contrat que vous avez entre les mains déclare sur des pages et des pages des conditions d'ouverture de compte et de services sommes toutes classiques.\n\n" +
                    "Les suivantes sortent tout de même sensiblement de l'ordinaire :\n" +
                    " - La banque autorise un découvert illimité et automatique avec intérêts de 30 %.\n" +
                    " - La banque se réserve la possibilité de demander le remboursement d'un prêt à n'importe quel moment.\n" +
                    " - En cas de défaut de paiement, le signataire s'engage à rembourser sa dette sous forme de Substance Pure, récoltée dans les tréfons de Similisse.\n" +
                    " - La banque assure la sécurité des coffres : seuls les clients sont autorisés à voir et interagir au coffre qui leur a été attribué et à son contenu.\n" +
                    " - Le client signataire s'engage à ne pas tenter de voir ou d'interagir avec les coffres d'autres clients, ou leur contenu.\n\n" +
                    "Bon pour accord, signature : ";
                  }
                  break;

                case "exit":
                  player.oid.NuiDestroy(token);
                  break;
              }
              break;
          }
        }
      }
    }
  }
}
