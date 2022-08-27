using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DmSystem))]
  public class DmSystem
  {
    private readonly PlaceableSystem placeableSystem;
    public DmSystem(PlaceableSystem placeableSystem)
    {
      this.placeableSystem = placeableSystem;

      NwModule.Instance.OnDMSpawnObjectAfter += HandleAfterDmSpawnObject;
      NwModule.Instance.OnDMJumpTargetToPoint += HandleAfterDmJumpTarget;
      NwModule.Instance.OnDMJumpAllPlayersToPoint += HandleBeforeDMJumpAllPlayers;
      NwModule.Instance.OnDMGiveXP += HandleBeforeDmGiveXP;
      NwModule.Instance.OnDMGiveGold += HandleBeforeDmGiveGold;
      NwModule.Instance.OnDMGiveItemAfter += HandleAfterDmGiveItem;
    }
    public void HandleAfterDmSpawnObject(OnDMSpawnObjectAfter onSpawn)
    {
      if (onSpawn.SpawnedObject is NwPlaceable oPLC)
      {
        if (onSpawn.DungeonMaster.LoginCreature.GetObjectVariable<LocalVariableInt>("_SPAWN_PERSIST").HasValue)
        {
          oPLC.OnDeath += placeableSystem.HandleCleanDMPLC;

          SqLiteUtils.InsertQuery("dm_persistant_placeable",
            new List<string[]>() { new string[] { "accountID", "0" }, new string[] { "serializedPlaceable", oPLC.Serialize().ToBase64EncodedString() }, new string[] { "areaTag", oPLC.Area.Tag }, new string[] { "position", oPLC.Position.ToString() }, new string[] { "facing", oPLC.Rotation.ToString() } });

          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
          query.Execute();
          oPLC.GetObjectVariable<LocalVariableInt>("_ID").Value = query.Result.GetInt(0);

          onSpawn.DungeonMaster.SendServerMessage($"Création persistante - Vous posez le placeable  {oPLC.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
        }
        else
          onSpawn.DungeonMaster.SendServerMessage($"Création temporaire - {oPLC.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
      }
      else if(onSpawn.SpawnedObject is NwItem oItem)
      {
        oItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
        Utils.LogMessageToDMs($"{onSpawn.DungeonMaster.PlayerName} vient de créer {oItem.Name}");
      }
    }
    public void HandleAfterDmJumpTarget(OnDMJumpTargetToPoint onJump)
    {
      foreach(NwGameObject target in onJump.Targets)
        if(target is NwCreature targetCreature && targetCreature.Area != null && targetCreature.Area.Tag == "LaBrume" && PlayerSystem.Players.TryGetValue(targetCreature, out PlayerSystem.Player player))
          player.DestroyPlayerCorpse();
    }
    public void HandleBeforeDMJumpAllPlayers(OnDMJumpAllPlayersToPoint onJump)
    {
      onJump.Skip = true;
      onJump.DungeonMaster.SendServerMessage("La fonctionnalité de téléportation massive est désactivée.", ColorConstants.Orange);
    }

    public void HandleBeforeDmGiveXP(OnDMGiveXP onGive)
    {
      onGive.Skip = true;
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient d'essayer de donner de l'xp à {onGive.Target.Name}");
    }

    public void HandleBeforeDmGiveGold(OnDMGiveGold onGive)
    {
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Amount} po à {onGive.Target.Name}");
    }

    public void HandleAfterDmGiveItem(OnDMGiveItemAfter onGive)
    {
      onGive.Item.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Item.Name} à {onGive.Target.Name}");
    }
  }
}
