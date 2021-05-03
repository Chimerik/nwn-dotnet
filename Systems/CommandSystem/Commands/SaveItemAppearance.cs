using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  class SaveItemAppearance
  {
    public SaveItemAppearance(NwPlayer oPC)
    {
      oPC.SendServerMessage("Veuillez sélectionnner l'objet dont vous souhaitez sauvegarder l'apparence.", Color.ROSE);
      PlayerSystem.cursorTargetService.EnterTargetMode(oPC, OnAppearanceSelected, API.Constants.ObjectTypes.Item, API.Constants.MouseCursor.Create);
    }
    private static async void OnAppearanceSelected(ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.TargetObject is null || !(selection.TargetObject is NwItem) || !PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      NwItem item = (NwItem)selection.TargetObject;

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.Name)
      {
        player.oid.SendServerMessage($"Impossible de sauvegarder cette apparence. Il est indiqué : Pour tout modification, s'adresser à {item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(Color.WHITE)}", Color.ORANGE);
        return;
      }

      int ACValue = -1;
      if (item.BaseItemType == API.Constants.BaseItemType.Armor)
        ACValue = ItemPlugin.GetBaseArmorClass(selection.TargetObject);

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez indiquer le nom sous lequel vous souhaitez sauvegarder l'apparence de cet objet."
      };

      player.menu.Draw();

      bool awaitedValue = await player.WaitForPlayerInputInt();

      if (awaitedValue)
      {
        string input = player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Value;

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerItemAppearance (characterId, appearanceName, serializedAppearance, baseItemType, AC) VALUES (@characterId, @appearanceName, @serializedAppearance, @baseItemType, @AC)" +
              $"ON CONFLICT (characterId, appearanceName) DO UPDATE SET serializedAppearance = @serializedAppearance, baseItemType = @baseItemType, AC = @AC where characterId = @characterId and appearanceName = @appearanceName");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@appearanceName", input);
        NWScript.SqlBindString(query, "@serializedAppearance", ItemPlugin.GetEntireItemAppearance(selection.TargetObject));
        NWScript.SqlBindInt(query, "@baseItemType", (int)item.BaseItemType);
        NWScript.SqlBindInt(query, "@AC", ACValue);
        NWScript.SqlStep(query);

        player.oid.SendServerMessage($"L'apparence de votre {selection.TargetObject.Name.ColorString(Color.WHITE)} a été sauvegardée sous le nom {input.ColorString(Color.WHITE)}.", Color.GREEN);
        player.menu.Close();

        player.oid.GetLocalVariable<string>("_PLAYER_INPUT").Delete();
      }
    }
  }
}
