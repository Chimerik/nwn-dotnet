using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool IsBonusActionSpell(NwCreature caster, int spellId, SpellEntry spellEntry, NwFeat? feat)
    {
      if (spellEntry.isBonusAction || spellEntry.isReaction
        || (spellId == CustomSpell.MageHand && caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterPolyvalent))
        || (spellId == CustomSpell.IllusionMineure && caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree))
        || (caster.KnowsFeat((Feat)CustomSkill.DruideFormeDeLune) && Utils.In(spellId, CustomSpell.FormeSauvageAraignee, CustomSpell.FormeSauvageBlaireau, CustomSpell.FormeSauvageChat, CustomSpell.FormeSauvageDilophosaure, CustomSpell.FormeSauvageLoup, CustomSpell.FormeSauvageOursHibou, CustomSpell.FormeSauvagePanthere, CustomSpell.FormeSauvageRothe))
        || feat?.Id == CustomSkill.RangerVoileNaturel)
        return true;

      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAcceleration))
      {
        EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoAcceleration, EffectSystem.MetamagieEffectTag);
        return true;
      }

      return false;
    }
  }
}
