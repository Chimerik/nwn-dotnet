using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Anvil.API;
using Anvil.API.Events;

using Microsoft.Data.Sqlite;

using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class BankContractWindow : PlayerWindow
      {
        NuiGroup rootGroup { get; }
        NuiColumn rootColumn { get; }
        List<NuiElement> rootChidren { get; }
        NuiRow introTextRow { get; }
        NuiRow signRow { get; }
        NwItem contract { get; }

        public BankContractWindow(Player player, NwItem contract) : base(player)
        {
          windowId = "bankContract";
          this.contract = contract;

          rootChidren = new List<NuiElement>();
          rootColumn = new NuiColumn() { Children = rootChidren };
          rootGroup = new NuiGroup() { Id = "rootGroup", Border = true, Layout = rootColumn };

          introTextRow = new NuiRow() { Width = 510, Children = new List<NuiElement>() { new NuiText(contract.Description) } };

          signRow = new NuiRow()
          {
            Width = 510,
            Children = new List<NuiElement>()
            {
              new NuiSpacer(),
              new NuiButton("Signer.") { Id = "sign" },
              new NuiSpacer()
            }
          };

          CreateWindow();
        }
        public void CreateWindow()
        {
          rootChidren.Clear();
          rootChidren.Add(introTextRow);
          rootChidren.Add(signRow);

          NuiRect windowRectangle = new NuiRect(0, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.02f, 540, 340);

          window = new NuiWindow(rootGroup, "Contrat d'ouverture de compte Skalsgard")
          {
            Geometry = geometry,
            Resizable = true,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          player.oid.OnNuiEvent -= HandleBankContractEvents;
          player.oid.OnNuiEvent += HandleBankContractEvents;

          token = player.oid.CreateNuiWindow(window, windowId);

          geometry.SetBindValue(player.oid, token, windowRectangle);
          geometry.SetBindWatch(player.oid, token, true);

          player.openedWindows[windowId] = token;
        }
        private async void HandleBankContractEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          if (nuiEvent.Player.NuiGetWindowId(nuiEvent.WindowToken) != windowId)
            return;

          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "sign":

                  player.oid.NuiDestroy(token);
                  contract.Description += player.oid.LoginCreature.Name;

                  foreach (var ip in contract.ItemProperties)
                    contract.RemoveItemProperty(ip);

                  int id = 1;

                  using (var connection = new SqliteConnection(Config.dbPath))
                  {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT max(id) from bankPlaceables";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                      while (reader.Read())
                        id += reader.GetInt32(0);
                    }
                  }

                  SqLiteUtils.InsertQuery("bankPlaceables",
                  new List<string[]>() { new string[] { "id", id.ToString() }, new string[] { "areaTag", "similissebanque" }, new string[] { "ownerId", player.characterId.ToString() }, new string[] { "ownerName", player.oid.LoginCreature.Name } });

                  NwPlaceable bank = NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").FirstOrDefault(b => b.GetObjectVariable<LocalVariableInt>("id").Value == id);
                  bank.GetObjectVariable<LocalVariableInt>("ownerId").Value = player.characterId;
                  bank.Name = player.oid.LoginCreature.Name;
                  bank.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpKnock));

                  player.oid.SendServerMessage("Le contrat est désormais signé. Votre emplacement réservé vous attend dans la salle des coffres de la banque Skalsgard.", new Color(32, 255, 32));

                  InitializeGiftItems();

                  break;
              }
              break;
          }
        }
        private async void InitializeGiftItems()
        {
          List<string> serializedItems = new List<string>();

          serializedItems.Add(CreateTempGiftItem("bad_armor", 1));
          serializedItems.Add(CreateTempGiftItem("bad_club", 1));
          serializedItems.Add(CreateTempGiftItem("bad_shield", 1));
          serializedItems.Add(CreateTempGiftItem("bad_sling", 1));
          serializedItems.Add(CreateTempGiftItem("NW_WAMBU001", 99));

          Task<string> serializeBank = Task.Run(() => JsonConvert.SerializeObject(serializedItems));
          await Task.WhenAll(serializeBank);

          SqLiteUtils.UpdateQuery("playerCharacters",
            new List<string[]>() { new string[] { "persistantStorage", serializeBank.Result } },
            new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });
        }
        private string CreateTempGiftItem(string itemTemplate, int stackSize)
        {
          NwItem tempItem = NwItem.Create("itemTemplate", NwModule.Instance.StartingLocation, false, stackSize);
          tempItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
          tempItem.Destroy();
          return tempItem.Serialize().ToBase64EncodedString();
        }
      }
    }
  }
}
