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

      if (!ItemUtils.IsArmor(player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest))
        && !ItemUtils.IsShield(player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand))
        && player.oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1))
      {
        EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.MonkSpeedEffectTag);
        WaitNextFrameToApplyMonkSpeedEffect(player.oid.LoginCreature, player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Monk).Level);
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
