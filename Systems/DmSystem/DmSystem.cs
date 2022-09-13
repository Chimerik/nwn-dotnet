using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(DmSystem))]
  public class DmSystem
  {
    private readonly PlaceableSystem placeableSystem;
    public DmSystem(PlaceableSystem placeableSystem, EventService eventService)
    {
      this.placeableSystem = placeableSystem;

      eventService.SubscribeAll<OnDMSpawnObject, DMEventFactory>(HandleAfterDmSpawnObject, EventCallbackType.After);
      NwModule.Instance.OnDMJumpTargetToPoint += HandleAfterDmJumpTarget;
      NwModule.Instance.OnDMJumpAllPlayersToPoint += HandleBeforeDMJumpAllPlayers;
      NwModule.Instance.OnDMGiveXP += HandleBeforeDmGiveXP;
      NwModule.Instance.OnDMGiveGold += HandleBeforeDmGiveGold;
      eventService.SubscribeAll<OnDMGiveItem, DMEventFactory>(HandleAfterDmGiveItem, EventCallbackType.After);
    }
    public void HandleAfterDmSpawnObject(OnDMSpawnObject onSpawn)
    {
      if (onSpawn.SpawnedObject is NwItem oItem)
      {
        oItem.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
        Utils.LogMessageToDMs($"{onSpawn.DungeonMaster.PlayerName} vient de créer {oItem.Name}");
      }
      else if(onSpawn.SpawnedObject is NwCreature creature)
        creature.OnDeath += CreatureUtils.MakeInventoryUndroppable;
    }
    public void HandleAfterDmJumpTarget(OnDMJumpTargetToPoint onJump)
    {
      foreach (NwGameObject target in onJump.Targets)
        if (target is NwCreature targetCreature && targetCreature.Area != null && targetCreature.Area.Tag == "LaBrume" && PlayerSystem.Players.TryGetValue(targetCreature, out PlayerSystem.Player player))
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

    public void HandleAfterDmGiveItem(OnDMGiveItem onGive)
    {
      onGive.Item.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      Utils.LogMessageToDMs($"{onGive.DungeonMaster.PlayerName} vient de donner {onGive.Item.Name} à {onGive.Target.Name}");
    }
  }
}
