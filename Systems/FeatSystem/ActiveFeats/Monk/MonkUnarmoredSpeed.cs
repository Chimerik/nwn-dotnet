using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkUnarmoredSpeed(NwCreature caster)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.MonkSpeedEffectTag);
        caster.GetObjectVariable<LocalVariableInt>("_MONK_SPEED_DISABLED").Value = 1;
        caster.LoginPlayer?.SendServerMessage("Vitesse du moine désactivée");
      }
      else if (Players.TryGetValue(caster, out Player player))
      {
        SkillSystem.OnLearnUnarmoredSpeed(player, CustomSkill.MonkUnarmoredSpeed);
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_MONK_SPEED_DISABLED").Delete();
        player.oid.SendServerMessage("Vitesse du moine activée");
      }
    }
  }
}
