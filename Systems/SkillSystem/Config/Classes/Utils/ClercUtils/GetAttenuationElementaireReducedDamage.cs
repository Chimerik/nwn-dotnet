using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static int GetAttenuationElementaireReducedDamage(NwCreature target, int damage, DamageType damageType)
    {
      if (damage > 0 && target is not null && target.KnowsFeat((Feat)CustomSkill.ClercAttenuationElementaire) 
        && Utils.In(damageType, DamageType.Acid, DamageType.Fire, DamageType.Cold, DamageType.Electrical)
        && target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).HasValue)
      {
        damage /= 2;
        target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
        LogUtils.LogMessage($"Atténuation Elémentaire - {damage}", LogUtils.LogType.Combat);
      }

      return damage;
    }
  }
}
