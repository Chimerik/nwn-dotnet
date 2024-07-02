using System.Linq;
using Anvil.API;
using static NWN.Systems.SpellConfig;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool ComputeCreatureAbilityDisadvantage(NwCreature creature, Ability ability, SpellEntry spellEntry = null, SpellEffectType effectType = SpellEffectType.Invalid, NwGameObject oCaster = null)
    {
      if (oCaster is NwCreature caster)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterMagicalAmbush) && !creature.IsCreatureSeen(caster))
          return true;

        if (spellEntry is not null)
        { 
          byte paladinSpellLevel = NwSpell.FromSpellId(spellEntry.RowIndex).GetSpellLevelForClass(ClassType.Paladin);

          if (paladinSpellLevel > 0 && paladinSpellLevel < 10 && creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChampionAntiqueEffectTag && e.Creator == caster))
            return true;

          if (spellEntry.RowIndex == CustomSpell.ConspuerLennemi && Utils.In(creature.Race.RacialType, RacialType.Undead, RacialType.Outsider))
            return true;
        }
      }

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
            else if (EffectSystem.MaledictionForceEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction force", LogUtils.LogType.Combat);
              return true;
            }
             
            break;

          case Ability.Dexterity:

            if (EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Bouclier ou Armure non maîtrisé", LogUtils.LogType.Combat);
              return true;
            }
            else if (EffectSystem.CourrouxDeLaNatureEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Courroux de la Nature", LogUtils.LogType.Combat);
              return true;
            }
            else if (EffectSystem.MaledictionDexteriteEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Dextérité", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case Ability.Constitution:

            if (EffectUtils.In(eff.Tag, EffectSystem.SaignementEffectTag, EffectSystem.MorsureInfectieuseEffectTag))
            {
              LogUtils.LogMessage("Désavantage - Saignement ou Morsure Infectieuse", LogUtils.LogType.Combat);
              return true;
            }
            else if (EffectSystem.MaledictionConstitutionEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Constitution", LogUtils.LogType.Combat);
              return true;
            }
            
            break;

          case Ability.Intelligence:

            if (EffectSystem.MaledictionIntelligenceEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Intelligence", LogUtils.LogType.Combat);
              return true;
            }

            break;

          case Ability.Wisdom:

            if (EffectSystem.MaledictionSagesseEffectTag == eff.Tag)
            {
              LogUtils.LogMessage("Désavantage - Malédiction Sagesse", LogUtils.LogType.Combat);
              return true;
            }

            break;
        }
      }

      return false;
    }
  }
}
