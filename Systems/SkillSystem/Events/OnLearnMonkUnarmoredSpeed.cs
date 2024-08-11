using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnUnarmoredSpeed(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkUnarmoredSpeed))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MonkUnarmoredSpeed);

      NwItem armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
      NwItem shield = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (armor is null || armor.BaseACValue < 1)
      {
        if (shield is not null)
        {
          switch (shield.BaseItem.ItemType)
          {
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:
              return true;
          }
        }

        if (player.oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1))
        {
          EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.MonkSpeedEffectTag);
          WaitNextFrameToApplyMonkSpeedEffect(player.oid.LoginCreature, player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Monk).Level);
        }
      }

      return true;
    }
    private static async void WaitNextFrameToApplyMonkSpeedEffect(NwCreature creature, int monkLevel)
    {
      if (monkLevel > 0)
      {
        await NwTask.NextFrame();
        creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(monkLevel));
      }
    }
  }
}
