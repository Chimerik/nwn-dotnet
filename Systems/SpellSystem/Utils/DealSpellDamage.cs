using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, bool saveFailed = true, NwCreature caster = null)
    {
      int damage = Utils.random.Roll(spellEntry.damageDice, nbDices);

      if (!saveFailed)
        damage /= 2;

      //int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);

      if (caster is not null)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType))));
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType)));
      
      LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{spellEntry.damageDice} (caster lvl {casterLevel}) = {damage} {spellEntry.damageType}", LogUtils.LogType.Combat);
    }
  }
}
