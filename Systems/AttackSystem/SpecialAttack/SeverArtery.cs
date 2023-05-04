using System;
using Anvil.API;
using Anvil.Services;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static async void SeverArtery(Player player, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature targetCreature || targetCreature.Race.RacialType == RacialType.Construct || targetCreature.Race.RacialType == RacialType.Undead)
      {
        player.oid.SendServerMessage($"La cible {StringUtils.ToWhitecolor(targetObject.Name)} ne peut pas être affectée par le saignement", ColorConstants.Orange);
        return;
      }

      foreach (var eff in targetCreature.ActiveEffects)
        if (eff.Tag == "CUSTOM_CONDITION_BLEEDING")
          targetCreature.RemoveEffect(eff);

      int duration = 5 + (int)(player.learnableSkills[CustomSkill.SeverArtery].totalPoints * 1.5);

      await NwTask.NextFrame();
      //targetCreature.ApplyEffect(EffectDuration.Instant, Effect.Death(false, false));
      targetCreature.ApplyEffect(EffectDuration.Temporary, bleeding, TimeSpan.FromSeconds(duration));
    }
  }
}
