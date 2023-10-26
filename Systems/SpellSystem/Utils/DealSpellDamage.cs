using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DealSpellDamage(NwGameObject target, int casterLevel, int damageDice, int nbDices, DamageType damageType, VfxType damageVisualEffect)
    {
      int damage = Utils.random.Roll(damageDice, nbDices);
      //int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);
      target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(damageVisualEffect), Effect.Damage(damage, damageType)));
      LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{damageDice} (caster lvl {casterLevel}) = {damage}", LogUtils.LogType.Combat);
    }
  }
}
