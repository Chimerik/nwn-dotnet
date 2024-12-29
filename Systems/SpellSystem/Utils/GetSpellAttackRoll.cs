using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

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
        Effect shieldEffect = targetObject.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.BouclierEffectTag);
        int shieldBonus = shieldEffect is null ? 0 : 5;
        bool shieldUsed = false;
        Effect inspirationEffect = oCaster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.InspirationBardiqueEffectTag);
        int inspirationBardique = inspirationEffect is not null ? inspirationEffect.CasterLevel : 0;
        bool inspirationUsed = false;
        int criticalRange = 20;
        int targetAC = target.AC;
        int totalAttack;

        if (oCaster is NwCreature caster)
        {
          int proficiencyBonus = NativeUtils.GetCreatureProficiencyBonus(caster);
          attackModifier = proficiencyBonus + caster.GetAbilityModifier(spellCastingAbility) + GetAttackBonus(target, caster);
          advantage = CreatureUtils.GetAdvantageAgainstTarget(caster, spell, isRangedSpell, target, spellCastingAbility);
          attackRoll = NativeUtils.GetAttackRoll(caster, advantage, spellCastingAbility);
          totalAttack = attackRoll + attackModifier + GetSpellAttackBonus(caster);
          criticalRange = GetSpellCriticalRange(caster);
          targetAC = target.GetArmorClassVersus(caster);

          if ((attackRoll >= criticalRange || criticalHit) && caster.KnowsFeat((Feat)CustomSkill.Pourfendeur))
            caster.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Value = 1;

          if ((attackRoll < 2 || totalAttack <= targetAC)
          && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoGuidage))
          {
            int tempAttackRoll = NativeUtils.GetAttackRoll(caster, advantage, spellCastingAbility);
            LogUtils.LogMessage($"Métamagie Guidage : Jet d'attaque {attackRoll} remplacé par {tempAttackRoll}", LogUtils.LogType.Combat);
            attackRoll = tempAttackRoll;
            totalAttack = attackRoll + attackModifier + GetSpellAttackBonus(caster);
            EffectUtils.RemoveTaggedParamEffect(caster, CustomSkill.EnsoGuidage, EffectSystem.MetamagieEffectTag);
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
            if (totalAttack + inspirationBardique <= targetAC + shieldBonus) // Rate alors qu'il aurait du toucher
            {
              result = TouchAttackResult.Miss;
              hitString = "manque".ColorString(ColorConstants.Red);
              rollString = rollString.StripColors().ColorString(ColorConstants.Red);

              if (inspirationBardique != 0)
              {
                inspirationUsed = true;
                LogUtils.LogMessage($"Activation {(inspirationBardique > 0 ? "Inspiration Bardique" : "Mots Cinglants")} : {inspirationBardique} BA", LogUtils.LogType.Combat);
              }

              if(shieldBonus > 0)
              {
                targetAC += shieldBonus;
                shieldUsed = true;
                LogUtils.LogMessage("Activation Bouclier : +5 CA", LogUtils.LogType.Combat);
              }

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
            if(inspirationBardique > 0 && totalAttack + inspirationBardique > targetAC + shieldBonus) // Touche alors qu'il aurait du rater
            {
              result = TouchAttackResult.Hit;
              inspirationUsed = true;
              LogUtils.LogMessage($"Activation inspiration bardique : +{inspirationBardique}", LogUtils.LogType.Combat);
              LogUtils.LogMessage($"Touché : {attackRoll} + {attackModifier + inspirationBardique} = {totalAttack + inspirationBardique} vs {targetAC}", LogUtils.LogType.Combat);

              if (shieldBonus > 0)
              {
                targetAC += shieldBonus;
                shieldUsed = true;
                LogUtils.LogMessage("Activation Bouclier : +5 CA", LogUtils.LogType.Combat);
              }
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

          if (shieldUsed)
            EffectUtils.RemoveTaggedEffect(casterCreature, EffectSystem.BouclierEffectTag);

          casterCreature.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));

          if (result != TouchAttackResult.Miss)
          {
            if (casterCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.TraverseeInfernaleBuffEffectTag))
            {
              EffectUtils.RemoveTaggedEffect(casterCreature, EffectSystem.TraverseeInfernaleBuffEffectTag);
              int spellDC = GetCasterSpellDC(casterCreature, Ability.Charisma);

              if (CreatureUtils.GetSavingThrow(casterCreature, target, Ability.Charisma, spellDC) == SavingThrowResult.Failure)
                ApplyTraverseeInfernale(casterCreature, target);
            }

            if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.MaleficeTag && e.Creator == oCaster))
            {
              NWScript.AssignCommand(casterCreature, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(Utils.Roll(6, result == TouchAttackResult.CriticalHit ? 2 : 1), CustomDamageType.Necrotic)));
              target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
            }
          }
        }

        target.LoginPlayer?.SendServerMessage($"{advantageString}{criticalString}{oCaster.Name} {hitString} {target.Name} {rollString}".ColorString(ColorConstants.Cyan));
      }

      return result;
    }

    private static async void ApplyTraverseeInfernale(NwCreature attacker, NwCreature target)
    {
      target.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonGate));

      await NwTask.NextFrame();
      NWScript.AssignCommand(attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.TraverseeInfernale(target), NwTimeSpan.FromRounds(1)));
    }
  }
}
