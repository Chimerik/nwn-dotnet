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
        int criticalRange = 20;
        int targetAC = target.AC;
        int totalAttack;

        if (oCaster is NwCreature caster)
        {
          int proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(caster);
          attackModifier = proficiencyBonus + caster.GetAbilityModifier(spellCastingAbility);
          advantage = CreatureUtils.GetAdvantageAgainstTarget(caster, spell, isRangedSpell, target, spellCastingAbility);
          attackRoll = NativeUtils.GetAttackRoll(caster, advantage, spellCastingAbility);
          totalAttack = attackRoll + attackModifier;
          criticalRange = GetSpellCriticalRange(caster);
          targetAC = target.GetArmorClassVersus(caster);

          if ((attackRoll >= criticalRange || criticalHit) && caster.KnowsFeat((Feat)CustomSkill.Pourfendeur))
            caster.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Value = 1;

          foreach(var eff in caster.ActiveEffects)
            if(eff.Tag == EffectSystem.InspirationBardiqueEffectTag)
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
        else if (attackRoll > 1 && totalAttack > targetAC)
        {
          result = TouchAttackResult.Hit;
          LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
        }
        else if(attackRoll > 1 && totalAttack + inspirationBardique > targetAC)
        {
          if (inspirationEffect is not null)
            oCaster.RemoveEffect(inspirationEffect);

          attackModifier += inspirationBardique;
          totalAttack += inspirationBardique;

          result = TouchAttackResult.Hit;
          LogUtils.LogMessage($"Activation inspiration bardique : +{inspirationBardique}", LogUtils.LogType.Combat);
          LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
          
          if(oCaster is NwCreature castingCreature)
            StringUtils.DisplayStringToAllPlayersNearTarget(castingCreature, $"Inspiration Bardique (+{StringUtils.ToWhitecolor(inspirationBardique)})".ColorString(StringUtils.gold), StringUtils.gold, true, true);
        }
        else
        {
          result = TouchAttackResult.Miss;
          hitString = "manque".ColorString(ColorConstants.Red);
          rollString = rollString.StripColors().ColorString(ColorConstants.Red);
          LogUtils.LogMessage($"Manqué : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
        }

        if (oCaster is NwCreature casterCreature)
          casterCreature.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));
        
        target.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));
      }

      return result;
    }
  }
}
