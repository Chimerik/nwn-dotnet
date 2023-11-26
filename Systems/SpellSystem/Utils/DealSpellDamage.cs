using System.Linq;
using System.Security.AccessControl;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void DealSpellDamage(NwGameObject target, int casterLevel, SpellEntry spellEntry, int nbDices, NwCreature caster, bool saveFailed = true)
    {
      bool isElementalist = PlayerSystem.Players.TryGetValue(caster, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.Elementaliste, out LearnableSkill elementalist)
        && elementalist.featOptions.Any(e => e.Value.Any(d => d == (int)spellEntry.damageType));

      int damage = 0;

      for(int i = 0; i < nbDices; i++) 
      {
        int roll = NwRandom.Roll(Utils.random, spellEntry.damageDice);
        damage += isElementalist && roll < 2 ? 2 : roll;
      }

      damage = ItemUtils.GetShieldMasterReducedDamage(target, damage, saveFailed, spellEntry.savingThrowAbility);

      if (!saveFailed)
        damage /= 2;

      //int nDamage = SpellUtils.MaximizeOrEmpower(iDamage, 1 + nCasterLevel / 6, onSpellCast.MetaMagicFeat);

      damage = HandleResistanceBypass(target, isElementalist, damage, spellEntry);

      if (caster is not null)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType))));
      else
        target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType)));
      
      LogUtils.LogMessage($"Dégâts sur {target.Name} : {nbDices}d{spellEntry.damageDice} (caster lvl {casterLevel}) = {damage} {spellEntry.damageType}", LogUtils.LogType.Combat);
    }
  }
}
