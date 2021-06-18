using System;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DmSystem))]
  public class DmSystem
  {
    /*[ScriptHandler("b_dm_possess")]
    private void HandleBeforeDmPossess(CallInfo callInfo)
    {
      uint oPossessed = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET"));

      if (NWScript.GetIsObjectValid(oPossessed) == 1)
      { // Ici, on prend possession
        if (NWScript.GetIsDMPossessed(callInfo.ObjectSelf) == 1)
        {
          NWScript.SetLocalObject(NWScript.GetLocalObject(callInfo.ObjectSelf, "_POSSESSER"), "_POSSESSING", oPossessed);
          NWScript.SetLocalObject(oPossessed, "_POSSESSER", NWScript.GetLocalObject(callInfo.ObjectSelf, "_POSSESSER"));
        }
        else
        {
          NWScript.SetLocalObject(callInfo.ObjectSelf, "_POSSESSING", oPossessed);
          NWScript.SetLocalObject(oPossessed, "_POSSESSER", callInfo.ObjectSelf);
        }
      }
      else
      {  // Ici, on cesse la possession
        if (NWScript.GetIsDMPossessed(callInfo.ObjectSelf) == 1)
        {
          NWScript.DeleteLocalObject(NWScript.GetLocalObject(callInfo.ObjectSelf, "_POSSESSER"), "_POSSESSING");
          NWScript.DeleteLocalObject(NWScript.GetLocalObject(callInfo.ObjectSelf, "_POSSESSER"), "_POSSESSER");
        }
        else
        {
          NWScript.DeleteLocalObject(NWScript.GetLocalObject(callInfo.ObjectSelf, "_POSSESSER"), "_POSSESSING");
          NWScript.DeleteLocalObject(callInfo.ObjectSelf, "_POSSESSER");
        }
      }
    }*/
    public static void HandleBeforeDmSpawnBefore(OnDMSpawnObjectBefore onSpawn)
    {
      PlayerSystem.Log.Info("dm spawn object before");
      PlayerSystem.Log.Info($"resref : {onSpawn.ResRef}");
    }
    public static void HandleAfterDmSpawnObject(OnDMSpawnObjectAfter onSpawn)
    {
      PlayerSystem.Log.Info("dm spawn object after");
      if (!(onSpawn.SpawnedObject is NwPlaceable oPLC))
      {
        PlayerSystem.Log.Info("dm spawn is not a PLC");
        return;
      }
      else
        PlayerSystem.Log.Info("dm spawn is a PLC !");

      if (onSpawn.DungeonMaster.LoginCreature.GetLocalVariable<int>("_SPAWN_PERSIST").HasValue)
        {
          oPLC.OnDeath += PlaceableSystem.HandleCleanDMPLC;

          var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, "INSERT INTO dm_persistant_placeable(accountID, serializedPlaceable, areaTag, position, facing)" +
            " VALUES(@accountId, @serializedPlaceable, @areaTag, @position, @facing)");
          NWScript.SqlBindInt(query, "@accountId", 0);
          NWScript.SqlBindObject(query, "@serializedPlaceable", oPLC);
          NWScript.SqlBindString(query, "@areaTag", oPLC.Area.Tag);
          NWScript.SqlBindVector(query, "@position", oPLC.Position);
          NWScript.SqlBindFloat(query, "@facing", oPLC.Rotation);
          NWScript.SqlStep(query);

          query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
          NWScript.SqlStep(query);
          NWScript.SetLocalInt(oPLC, "_ID", NWScript.SqlGetInt(query, 0));

          onSpawn.DungeonMaster.SendServerMessage($"Création persistante - Vous posez le placeable  {oPLC.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
        }
        else
          onSpawn.DungeonMaster.SendServerMessage($"Création temporaire - {oPLC.Name.ColorString(ColorConstants.White)}", new Color(32, 255, 32));
    }
    public static void HandleAfterDmJumpTarget(OnDMJumpTargetToPoint onJump)
    {
      foreach(NwGameObject target in onJump.Targets)
        if(target is NwCreature targetCreature && targetCreature.Area.Tag == "LaBrume" && PlayerSystem.Players.TryGetValue(targetCreature, out PlayerSystem.Player player))
          PlayerSystem.DestroyPlayerCorpse(player);
    }
    public static void HandleBeforeDMJumpAllPlayers(OnDMJumpAllPlayersToPoint onJump)
    {
      onJump.Skip = true;
      onJump.DungeonMaster.SendServerMessage("La fonctionnalité de téléportation massive est désactivée.", ColorConstants.Orange);
    }

    public static void HandleBeforeDmGiveXP(OnDMGiveXP onGive)
    {
      onGive.Skip = true;
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient d'essayer de donner de l'xp à {onGive.Target.Name}");
    }

    public static void HandleBeforeDmGiveGold(OnDMGiveGold onGive)
    {
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Amount} po à {onGive.Target.Name}");
    }

    public static void HandleAfterDmGiveItem(OnDMGiveItemAfter onGive)
    {
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Item.Name} à {onGive.Target.Name}");
    }
  }
}
