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
    [ScriptHandler("b_dm_possess")]
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
    }
    [ScriptHandler("dm_spawn_object")]
    private void HandleAfterDmSpawnObject(CallInfo callInfo)
    {
      if (int.Parse(EventsPlugin.GetEventData("OBJECT_TYPE")) == NWScript.OBJECT_TYPE_PLACEABLE)
      {
        NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

        if (oPC.GetLocalVariable<int>("_SPAWN_PERSIST").HasValue)
        {
          NwPlaceable oObject = NWScript.StringToObject(EventsPlugin.GetEventData("OBJECT")).ToNwObject<NwPlaceable>();
          oObject.OnDeath += PlaceableSystem.HandleCleanDMPLC;

          var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, "INSERT INTO dm_persistant_placeable(accountID, serializedPlaceable, areaTag, position, facing)" +
            " VALUES(@accountId, @serializedPlaceable, @areaTag, @position, @facing)");
          NWScript.SqlBindInt(query, "@accountId", 0);
          NWScript.SqlBindObject(query, "@serializedPlaceable", oObject);
          NWScript.SqlBindString(query, "@areaTag", oObject.Area.Tag);
          NWScript.SqlBindVector(query, "@position", oObject.Position);
          NWScript.SqlBindFloat(query, "@facing", oObject.Rotation);
          NWScript.SqlStep(query);

          query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
          NWScript.SqlStep(query);
          NWScript.SetLocalInt(oObject, "_ID", NWScript.SqlGetInt(query, 0));

          oPC.SendServerMessage($"Création persistante - Vous posez le placeable  {oObject.Name}");
        }
        else
          oPC.SendServerMessage("Création temporaire - Ce placeable sera effacé par le prochain reboot.");
      }
    }
    [ScriptHandler("a_dm_jump_target")]
    private void HandleAfterDmJumpTarget(CallInfo callInfo)
    {
      NwCreature oTarget = NWScript.StringToObject(EventsPlugin.GetEventData("TARGET_1")).ToNwObject<NwCreature>();

      if (oTarget.Area.Tag == "Labrume")
      {
        if (PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
          PlayerSystem.DestroyPlayerCorpse(player);
      }
    }
  }
}
