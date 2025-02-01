using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static int GetAttenuationElementaireReducedDamage(NwCreature target, int damage, DamageType damageType)
    {
      if (damage > 0 && target is not null && target.KnowsFeat((Feat)CustomSkill.ClercAttenuationElementaire)
        && Utils.In(damageType, DamageType.Acid, DamageType.Fire, DamageType.Cold, DamageType.Electrical))
      {
        var reaction = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          damage /= 2;
          target.RemoveEffect(reaction);
        LogUtils.LogMessage($"Atténuation Elémentaire - {damage}", LogUtils.LogType.Combat);
        } 
      }

      return damage;
    }
  }
}
