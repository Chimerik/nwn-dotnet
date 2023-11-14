using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DmSystem))]
  public class DmSystem
  {
    public DmSystem(PlaceableSystem placeableSystem, EventService eventService)
    {
      eventService.SubscribeAll<OnDMSpawnObject, DMEventFactory>(HandleAfterDmSpawnObject, EventCallbackType.After);
      eventService.SubscribeAll<OnDMSpawnTrapOnObject, DMEventFactory>(HandleAfterDmSpawnTrapOnObject, EventCallbackType.After);
      NwModule.Instance.OnDMJumpTargetToPoint += HandleAfterDmJumpTarget;
      NwModule.Instance.OnDMJumpAllPlayersToPoint += HandleBeforeDMJumpAllPlayers;
      NwModule.Instance.OnDMGiveXP += HandleBeforeDmGiveXP;
      NwModule.Instance.OnDMGiveGold += HandleBeforeDmGiveGold;
      eventService.SubscribeAll<OnDMGiveItem, DMEventFactory>(HandleAfterDmGiveItem, EventCallbackType.After);
    }
    public static void HandleAfterDmSpawnObject(OnDMSpawnObject onSpawn)
    {
      if (onSpawn.SpawnedObject is NwItem oItem)
      {
        oItem.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").Value = onSpawn.DungeonMaster.PlayerName;
        LogUtils.LogMessage($"{onSpawn.DungeonMaster.PlayerName} créé {oItem.Name}", LogUtils.LogType.DMAction);
      }
      else if (onSpawn.SpawnedObject is NwCreature creature)
        creature.OnDeath += CreatureUtils.MakeInventoryUndroppable;
      else if (onSpawn.SpawnedObject is NwTrigger trigger)
        trigger.OnTrapTriggered += PlaceableSystem.OnTrapTriggered;
    }
    public static void HandleAfterDmSpawnTrapOnObject(OnDMSpawnTrapOnObject onSpawn)
    {
      if (onSpawn.Target is NwPlaceable plc)
      {
        plc.OnTrapTriggered += PlaceableSystem.OnTrapTriggered;
      }
      else if (onSpawn.Target is NwDoor door)
        door.OnTrapTriggered += PlaceableSystem.OnTrapTriggered;
    }
    public static void HandleAfterDmJumpTarget(OnDMJumpTargetToPoint onJump)
    {
      foreach (NwGameObject target in onJump.Targets)
        if (target is NwCreature targetCreature && targetCreature.Area != null && targetCreature.Area.Tag == "LaBrume" && PlayerSystem.Players.TryGetValue(targetCreature, out PlayerSystem.Player player))
          player.DestroyPlayerCorpse();
    }
    public static void HandleBeforeDMJumpAllPlayers(OnDMJumpAllPlayersToPoint onJump)
    {
      onJump.Skip = true;
      onJump.DungeonMaster.SendServerMessage("La fonctionnalité de téléportation massive est désactivée.", ColorConstants.Orange);
    }

    public static void HandleBeforeDmGiveXP(OnDMGiveXP onGive)
    {
      onGive.Skip = true;
      LogUtils.LogMessage($"{onGive.DungeonMaster.PlayerName} vient d'essayer de donner de l'xp à {onGive.Target.Name}", LogUtils.LogType.DMAction);
    }

    public static void HandleBeforeDmGiveGold(OnDMGiveGold onGive)
    {
      LogUtils.LogMessage($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Amount} po à {onGive.Target.Name}", LogUtils.LogType.DMAction);
    }

    public static void HandleAfterDmGiveItem(OnDMGiveItem onGive)
    {
      onGive.Item.GetObjectVariable<LocalVariableString>("DM_ITEM_CREATED_BY").Value = onGive.DungeonMaster.PlayerName;
      LogUtils.LogMessage($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Item.Name} à {onGive.Target.Name}", LogUtils.LogType.DMAction);
    }
  }
}
