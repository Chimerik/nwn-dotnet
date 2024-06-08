using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (oCaster is NwCreature caster && caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterMagicalAmbush) && !creature.IsCreatureSeen(caster))
        return true;

      foreach (var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {

          case EffectSystem.FrightenedEffectTag:
            LogUtils.LogMessage("Désavantage - Effroi", LogUtils.LogType.Combat);
            return true;

          case EffectSystem.PoisonEffectTag:
            LogUtils.LogMessage("Désavantage - Poison", LogUtils.LogType.Combat);
            return true;

          case EffectSystem.FrappeOcculteEffectTag:

            if (spellEntry is not null && oCaster is not null && eff.Creator == oCaster)
            {
              creature.RemoveEffect(eff);
              LogUtils.LogMessage("Désavantage - Chevalier Occulte : Frappe Occulte", LogUtils.LogType.Combat);
              return true;
            }
            break;
        }

        switch (ability)
        {
          case Ability.Strength:

            if (EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Bouclier ou Armure non maîtrisé", LogUtils.LogType.Combat);
              return true;
            }
            break;

          case Ability.Dexterity:

            if (EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Bouclier ou Armure non maîtrisé", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case Ability.Constitution:

            if (EffectUtils.In(eff.Tag, EffectSystem.SaignementEffectTag, EffectSystem.MorsureInfectieuseEffectTag))
            {
              LogUtils.LogMessage("Désavantage - Saignement ou Morsure Infectieuse", LogUtils.LogType.Combat);
              return true;
            }

            break;
        }
      }

      return false;
    }
  }
}
