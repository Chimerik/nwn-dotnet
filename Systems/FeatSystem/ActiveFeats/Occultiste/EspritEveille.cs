using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void EspritEveille(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      var eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.EspritEveilleEffectTag && e.Creator != caster);

      if(eff is not null)
      {
        if(eff.Creator is NwCreature previousTarget)
          EffectUtils.RemoveTaggedEffect(previousTarget, caster, EffectSystem.EspritEveilleEffectTag, EffectSystem.EspritEveilleDisadvantageEffectTag);

        caster.RemoveEffect(eff);
      }

      TimeSpan duration =
        TimeSpan.FromMinutes(caster.GetClassInfo((ClassType)CustomClass.Occultiste).Level);
      NWScript.AssignCommand(oTarget, () => caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveille, duration));
      NWScript.AssignCommand(caster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveille, duration));

      if (caster.KnowsFeat((Feat)CustomSkill.CombattantClairvoyant))
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);
        if(CreatureUtils.GetSavingThrow(caster, target, Ability.Wisdom, spellDC) == SavingThrowResult.Failure)
          NWScript.AssignCommand(caster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritEveilleDisadvantage, duration));
      }      
    }
  }
}
