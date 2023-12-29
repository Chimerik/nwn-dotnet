using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static TouchAttackResult GetSpellAttackRoll(NwGameObject targetObject, NwCreature caster, NwSpell spell, Ability spellCastingAbility, int isRangedSpell = 1)
    {
      TouchAttackResult result = TouchAttackResult.Hit;

      if (targetObject is not NwCreature target)
        return result;

        bool criticalHit = IsSpellAttackAutoCrit(target);
        if (!criticalHit)
        {
          int proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(caster);
          int attackModifier = caster.GetAttackBonus(false, true, false, false) + proficiencyBonus
          - caster.GetAbilityModifier(Ability.Dexterity);

          attackModifier += caster.GetAbilityModifier(spellCastingAbility);

          int advantage = CreatureUtils.GetSpellAttackAdvantageAgainstTarget(caster, spell, isRangedSpell, target, spellCastingAbility);
          int attackRoll = NativeUtils.GetAttackRoll(caster, advantage, spellCastingAbility);
          int targetAC = target.GetArmorClassVersus(caster);
          int totalAttack = attackRoll + attackModifier;
          int criticalRange = GetSpellCriticalRange(caster);

          LogUtils.LogMessage($"Bonus d'attaque contre la cible {attackModifier} dont {caster.GetAbilityModifier(spellCastingAbility)} du modificateur de {spellCastingAbility.ToString()} et {proficiencyBonus} du bonus de maîtrise", LogUtils.LogType.Combat);
          LogUtils.LogMessage($"CA de la cible : {targetAC}", LogUtils.LogType.Combat);

          string hitString = "touchez".ColorString(new Color(32, 255, 32));
          string rollString = $"{attackRoll} + {attackModifier} = {totalAttack}".ColorString(new Color(32, 255, 32));
          string criticalString = "";
          string advantageString = advantage == 0 ? "" : advantage > 0 ? "Avantage - ".ColorString(StringUtils.gold) : "Désavantage - ".ColorString(ColorConstants.Red);

          if (attackRoll >= criticalRange || criticalHit)
          {
            result = TouchAttackResult.CriticalHit;
            criticalString = "CRITIQUE - ".ColorString(StringUtils.gold);
            LogUtils.LogMessage("Coup critique", LogUtils.LogType.Combat);

            if(caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Pourfendeur)))
              caster.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Value = 1;
        }
          else if (attackRoll > 1 && totalAttack > targetAC)
          {
            result = TouchAttackResult.Hit;
            LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
          }
          else
          {
            result = TouchAttackResult.Miss;
            hitString = "manquez".ColorString(ColorConstants.Red);
            rollString = rollString.StripColors().ColorString(ColorConstants.Red);
            LogUtils.LogMessage($"Manqué : {attackRoll} + {attackModifier} = {totalAttack} vs {targetAC}", LogUtils.LogType.Combat);
          }

          caster.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}Vous {hitString} {target.Name.ColorString(ColorConstants.Cyan)} {rollString}".ColorString(ColorConstants.Cyan));
        }

      return result;
    }
  }
}
