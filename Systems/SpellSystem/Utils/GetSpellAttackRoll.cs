using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static TouchAttackResult GetSpellAttackRoll(NwGameObject targetObject, NwGameObject oCaster, NwSpell spell, Ability spellCastingAbility, int isRangedSpell = 1)
    {
      TouchAttackResult result = TouchAttackResult.Hit;

      if (targetObject is not NwCreature target)
        return result;

      bool criticalHit = IsSpellAttackAutoCrit(target);
      if (!criticalHit)
      {
        int attackRoll = 10;
        int attackModifier = 0;
        int advantage = 0;
        int inspirationBardique = 0;
        Effect inspirationEffect = null;
        bool inspirationUsed = false;
        int criticalRange = 20;
        int targetAC = target.AC;
        int totalAttack;

        if (oCaster is NwCreature caster)
        {
          int proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(caster);
          attackModifier = proficiencyBonus + caster.GetAbilityModifier(spellCastingAbility);
          advantage = CreatureUtils.GetAdvantageAgainstTarget(caster, spell, isRangedSpell, target, spellCastingAbility);
          attackRoll = NativeUtils.GetAttackRoll(caster, advantage, spellCastingAbility);
          totalAttack = attackRoll + attackModifier + GetSpellAttackBonus(caster);
          criticalRange = GetSpellCriticalRange(caster);
          targetAC = target.GetArmorClassVersus(caster);

          if ((attackRoll >= criticalRange || criticalHit) && caster.KnowsFeat((Feat)CustomSkill.Pourfendeur))
            caster.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Value = 1;

          foreach (var eff in caster.ActiveEffects)
            if (eff.Tag == EffectSystem.InspirationBardiqueEffectTag)
            {
              inspirationBardique += eff.CasterLevel;
              inspirationEffect = eff;
              break;
            }

          LogUtils.LogMessage($"Bonus d'attaque contre la cible {attackModifier} dont {caster.GetAbilityModifier(spellCastingAbility)} du modificateur de {spellCastingAbility.ToString()} et {proficiencyBonus} du bonus de maîtrise", LogUtils.LogType.Combat);
          LogUtils.LogMessage($"CA de la cible : {targetAC}", LogUtils.LogType.Combat);
        }
        else
          totalAttack = (int)Math.Floor((double)(spell.InnateSpellLevel * 2) - 1);

        string hitString = "touche".ColorString(new Color(32, 255, 32));
        string rollString = $"{attackRoll} + {attackModifier} = {totalAttack}".ColorString(new Color(32, 255, 32));
        string criticalString = "";
        string advantageString = advantage == 0 ? "" : advantage > 0 ? "Avantage - ".ColorString(StringUtils.gold) : "Désavantage - ".ColorString(ColorConstants.Red);

        if (attackRoll >= criticalRange || criticalHit)
        {
          result = TouchAttackResult.CriticalHit;
          criticalString = "CRITIQUE - ".ColorString(StringUtils.gold);
          LogUtils.LogMessage("Coup critique", LogUtils.LogType.Combat);
        }
        else if (attackRoll > 1)
        {
          if(totalAttack > targetAC)
          {
            if(inspirationBardique < 0 && totalAttack + inspirationBardique <= targetAC) // Rate alors qu'il aurait du toucher
            {
              result = TouchAttackResult.Miss;
              hitString = "manque".ColorString(ColorConstants.Red);
              rollString = rollString.StripColors().ColorString(ColorConstants.Red);
              inspirationUsed = true;
              LogUtils.LogMessage($"Activation mots cinglants : {inspirationBardique}", LogUtils.LogType.Combat);
              LogUtils.LogMessage($"Manqué : {attackRoll} + {attackModifier + inspirationBardique} = {totalAttack + inspirationBardique} vs {targetAC}", LogUtils.LogType.Combat);
            }
            else // Touche, cas normal
            {
              result = TouchAttackResult.Hit;
              LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
            }
          }
          else
          {
            if(inspirationBardique > 0 && totalAttack + inspirationBardique > targetAC) // Touche alors qu'il aurait du rater
            {
              result = TouchAttackResult.Hit;
              inspirationUsed = true;
              LogUtils.LogMessage($"Activation inspiration bardique : +{inspirationBardique}", LogUtils.LogType.Combat);
              LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier + inspirationBardique} = {totalAttack + inspirationBardique} vs {targetAC}", LogUtils.LogType.Combat);
            }
            else // Rate, cas normal
            {
              result = TouchAttackResult.Miss;
              hitString = "manque".ColorString(ColorConstants.Red);
              rollString = rollString.StripColors().ColorString(ColorConstants.Red);
              LogUtils.LogMessage($"Manqué : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
            }
          }
        }
        else // Roll = 1 : echec auto
        {
          result = TouchAttackResult.Miss;
          hitString = "manque".ColorString(ColorConstants.Red);
          rollString = rollString.StripColors().ColorString(ColorConstants.Red);
          LogUtils.LogMessage($"Manqué : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
        }

        if (oCaster is NwCreature casterCreature)
        {
          if (inspirationUsed)
          {
            string inspirationType = inspirationBardique > 0 ? "Inspiration Bardique +" : "Mots Cinglants ";
            StringUtils.DisplayStringToAllPlayersNearTarget(casterCreature, $"{inspirationType}{StringUtils.ToWhitecolor(inspirationBardique)}".ColorString(StringUtils.gold), StringUtils.gold, true, true);
            casterCreature.RemoveEffect(inspirationEffect);
          }

          casterCreature.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));
        }

        target.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));
      }

      return result;
    }
  }
}
