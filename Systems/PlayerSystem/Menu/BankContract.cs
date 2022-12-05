using System;
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

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleBankContractEvents;

            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private async void HandleBankContractEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "sign":

                  CloseWindow();
                  contract.Description += player.oid.LoginCreature.Name;

                  foreach (var ip in contract.ItemProperties)
                    contract.RemoveItemProperty(ip);

                  string ownerName = player.oid.LoginCreature.Name;

                  var result = await SqLiteUtils.SelectQueryAsync("bankPlaceables",
                    new List<string>() { { "max(id)" } },
                    new List<string[]>() { });

                  int id = 1;

                  if (result != null && result.Count > 0)
                    if (int.TryParse(result.FirstOrDefault()[0], out id))
                      id += 1;

                  await NwTask.SwitchToMainThread();

                  try
                  {
                    SqLiteUtils.InsertQuery("bankPlaceables",
                    new List<string[]>() { new string[] { "id", id.ToString() }, new string[] { "areaTag", "similissebanque" }, new string[] { "ownerId", player.characterId.ToString() }, new string[] { "ownerName", ownerName } });

                    NwPlaceable bank = NwObject.FindObjectsWithTag<NwPlaceable>("player_bank").FirstOrDefault(b => b.GetObjectVariable<LocalVariableInt>("id").Value == id);
                    bank.GetObjectVariable<LocalVariableInt>("ownerId").Value = player.characterId;
                    bank.Name = player.oid.LoginCreature.Name;
                    bank.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpKnock));

                    player.oid.SendServerMessage("Le contrat est désormais signé. Votre emplacement réservé vous attend dans la salle des coffres de la banque Skalsgard.", new Color(32, 255, 32));
                    Utils.LogMessageToDMs($"{player.oid.LoginCreature.Name} ({player.oid.PlayerName}) signature contrat Skaslgard - Id {id} ");

                    InitializeGiftItems();
                    Utils.mailReceiverEntries.Add(new NuiComboEntry(player.oid.LoginCreature.Name, player.characterId));
                  }
                  catch(Exception e)
                  {
                    Utils.LogMessageToDMs($"BANK SYSTEM - ERROR - {player.oid.LoginCreature.Name} ({player.oid.PlayerName}) Impossible de créer le compte en banque - Id {id}");
                    Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}");
                  }

                  break;
              }
              break;
          }
        }
        private async void InitializeGiftItems()
        {
          List<string> serializedItems = new()
          {
            CreateTempGiftItem("bad_armor", 1),
            CreateTempGiftItem("bad_club", 1),
            CreateTempGiftItem("bad_shield", 1),
            CreateTempGiftItem("bad_sling", 1),
            CreateTempGiftItem("NW_WAMBU001", 99)
          };

          Task<string> serializeBank = Task.Run(() => JsonConvert.SerializeObject(serializedItems));
          await serializeBank;

          SqLiteUtils.UpdateQuery("playerCharacters",
            new List<string[]>() { new string[] { "persistantStorage", serializeBank.Result } },
            new List<string[]>() { new string[] { "rowid", player.characterId.ToString() } });
        }
        private static string CreateTempGiftItem(string itemTemplate, int stackSize)
        {
          NwItem tempItem = NwItem.Create(itemTemplate, NwModule.Instance.StartingLocation, false, stackSize);
          tempItem.Destroy();
          return tempItem.Serialize().ToBase64EncodedString();
        }
      }
    }
  }
}
