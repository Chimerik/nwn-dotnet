using System.Collections.Generic;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteSaveAppearanceCommand(ChatSystem.Context ctx, Options.Result options)
    {
      ctx.oSender.SendServerMessage("Veuillez sélectionnner l'objet dont vous souhaitez sauvegarder l'apparence.", Color.ROSE);
      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, OnAppearanceSelected, API.Constants.ObjectTypes.Item, API.Constants.MouseCursor.Create);
    }
    private static void OnAppearanceSelected(CursorTargetData selection)
    {
      if (selection.TargetObj is null || !(selection.TargetObj is NwItem) || !PlayerSystem.Players.TryGetValue(selection.Player, out PlayerSystem.Player player))
        return;

      NwItem item = (NwItem)selection.TargetObj;

      // TODO : ajouter un métier permettant de modifier n'importe quelle tenue
      if (item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").HasValue && item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value != player.oid.Name)
      {
        player.oid.SendServerMessage($"Impossible de sauvegarder cette apparence. Il est indiqué : Pour tout modification, s'adresser à {item.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value.ColorString(Color.WHITE)}", Color.ORANGE);
        return;
      }

      int ACValue = -1;
      if (item.BaseItemType == API.Constants.BaseItemType.Armor)
        ACValue = ItemPlugin.GetBaseArmorClass(selection.TargetObj);

      player.menu.Clear();

      player.menu.titleLines = new List<string>() {
        "Veuillez indiquer le nom sous lequel vous souhaitez sauvegarder l'apparence de cet objet."
      };

      Task playerInput = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_PLAYER_INPUT_STRING").Value = 1;
        player.setString = "";
        await NwTask.WaitUntil(() => player.setString != "");

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerItemAppearance (characterId, appearanceName, serializedAppearance, baseItemType, AC) VALUES (@characterId, @appearanceName, @serializedAppearance, @baseItemType, @AC)" +
              $"ON CONFLICT (characterId, appearanceName) DO UPDATE SET serializedAppearance = @serializedAppearance, baseItemType = @baseItemType, AC = @AC where characterId = @characterId and appearanceName = @appearanceName");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@appearanceName", player.setString);
        NWScript.SqlBindString(query, "@serializedAppearance", ItemPlugin.GetEntireItemAppearance(selection.TargetObj));
        NWScript.SqlBindInt(query, "@baseItemType", (int)item.BaseItemType);
        NWScript.SqlBindInt(query, "@AC", ACValue);
        NWScript.SqlStep(query);
      
        player.oid.SendServerMessage($"L'apparence de votre {selection.TargetObj.Name.ColorString(Color.WHITE)} a été sauvegardée sous le nom {player.setString.ColorString(Color.WHITE)}.", Color.GREEN);
        player.setString = "";
        player.menu.Close();
      });

      player.menu.Draw();
    }
  }
}
