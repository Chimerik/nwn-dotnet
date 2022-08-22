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
        private readonly List<NuiElement> rootChidren;
        private readonly NuiRow introTextRow;
        private readonly NuiRow contractRow;
        private readonly NuiRow exitRow;
        private readonly NuiText widgetBankerText;
        private readonly NuiBind<string> bankerText;
        private readonly NwCreature banker;

        public BankCounterWindow(Player player, NwCreature banker) : base(player)
        {
          windowId = "bankCounter";
          this.banker = banker;

          bankerText = new ("bankerText");

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
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
          int windowSize = 340;

          if (NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").Any(b => b.GetObjectVariable<LocalVariableInt>("ownerId").Value == player.characterId))
          {
            tempText = $"Bonjour, bonjour, cher {player.oid.ControlledCreature.Name}, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Très estimé client, votre coffre vous attend comme de coutume.\n\n" +
              $"Y a-t-il autre chose pour votre service ?";

            widgetBankerText.Height = 140;
          }
          else
          {
            rootChidren.Add(contractRow);

            tempText = $"Bonjour, bonjour, très estimé prospect, bienvenue chez Skalsgard Investissements !\n\n" +
              $"Je ne crois pas que nous ayons l'honneur de vous compter parmis nos clients.\n\n" +
              $"Souhaiteriez-vous ouvrir un compte et bénéficier de nos services ?\n\n" +
              $"Rien de plus simple, il suffit de parapher le contrat d'ouverture de compte et le tour sera joué !";

            widgetBankerText.Height = 260;
            windowSize = 450;
          }

          rootChidren.Add(exitRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, windowSize);

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

                case "exit":
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
