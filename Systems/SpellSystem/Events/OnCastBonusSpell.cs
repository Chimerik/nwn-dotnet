using System.Linq;
using System.Numerics;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnCastBonusSpell(OnSpellAction onSpellAction)
    {
      SpellEntry spellEntry = Spells2da.spellTable[onSpellAction.Spell.Id];
      NwSpell spell = onSpellAction.Spell;
      NwCreature caster = onSpellAction.Caster;

      /*if (!spellEntry.isBonusAction
        && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.MetamagieEffectTag && e.IntParams[5] == CustomSkill.EnsoAllonge))
      {
        if (!onSpellAction.IsFake)
        {
          onSpellAction.PreventSpellCast = true;
          _ = caster.ActionCastFakeSpellAt(spell, caster);
          return;
        }
        else
        {
          if (onSpellAction.TargetObject is null)
            caster.GetObjectVariable<LocalVariableLocation>("_ENSO_ALLONGE_TARGET").Value = Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation);
          else
            caster.GetObjectVariable<LocalVariableObject<NwGameObject>>("_ENSO_ALLONGE_TARGET").Value = onSpellAction.TargetObject;

          caster.OnSpellCast -= EnsoUtils.CastAllongeSpell;
          caster.OnSpellCast += EnsoUtils.CastAllongeSpell;

          onSpellAction.PreventSpellCast = true;
          return;
        }
      }*/

      if(spell.Id == CustomSpell.AuraDeVitalite && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeVitaliteEffectTag)
        && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeVitaliteHealEffectTag))
      {
        NwClass castingClass = onSpellAction.ClassIndex < 255 ? onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class : NwClass.FromClassId(CustomClass.Adventurer);
        SpellUtils.SpellSwitch(caster, onSpellAction.Spell, onSpellAction.Feat, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation), castingClass);
        onSpellAction.PreventSpellCast = true;
        return;
      }
      
      if (SpellUtils.IsBonusActionSpell(caster, spell.Id, spellEntry, onSpellAction.Feat))
      {
        if(spellEntry.isReaction
          || (spell.Id == CustomSpell.MarqueDuChasseur && caster.GetClassInfo(ClassType.Ranger) is not null))
        {
          if (!CreatureUtils.HandleReactionUse(caster))
          {
            onSpellAction.PreventSpellCast = true;
            return;
          }
        } 
        else if (!CreatureUtils.HandleBonusActionUse(caster))
        {
          onSpellAction.PreventSpellCast = true;
          return;
        }

        if (!SpellUtils.CanCastSpell(caster, onSpellAction.TargetObject, spell, spellEntry))
        {
          onSpellAction.PreventSpellCast = true;
          return;
        }

        if (onSpellAction.TargetObject is not NwItem)
        {
          var targetPosition = onSpellAction.TargetObject is null ? onSpellAction.TargetPosition : onSpellAction.TargetObject.Position;

          if (Vector3.DistanceSquared(caster.Position, targetPosition) > SpellUtils.GetSpellRange(spell))
          {
            caster.LoginPlayer?.SendServerMessage("Impossible de lancer ce sort en action bonus instantanée : vous n'êtes pas à portée", ColorConstants.Orange);
            onSpellAction.PreventSpellCast = true;
            return;
          }
        }
        
        if (caster.GetObjectVariable<LocalVariableObject<NwGameObject>>(CreatureUtils.CurrentAttackTarget).HasValue)
          _ = caster.ActionAttackTarget(caster.GetObjectVariable<LocalVariableObject<NwGameObject>>(CreatureUtils.CurrentAttackTarget).Value);

        SpellUtils.CheckDispelConcentration(caster, spell, spellEntry);
        SpellUtils.HandlePhlegetos(caster, spellEntry);

        if (!caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree) && spell.SpellType != (Spell)CustomSpell.IllusionMineure)
          EffectUtils.RemoveEffectType(caster, EffectType.Invisibility, EffectType.ImprovedInvisibility);

        LogUtils.LogMessage($"{caster.Name} - Sort {spell.Name.ToString()} ({spell.Id}) lancé en action bonus", LogUtils.LogType.Combat);
        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - {spell.Name.ToString().ColorString(ColorConstants.Orange)} - Action Bonus", StringUtils.gold, true, true);

        NwClass castingClass = onSpellAction.ClassIndex < 255 ? onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class : NwClass.FromClassId(CustomClass.Adventurer);

        SpellUtils.SpellSwitch(caster, onSpellAction.Spell, onSpellAction.Feat, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation), castingClass);

        if (castingClass.Id != CustomClass.Adventurer)
        {
          switch(spell.Id)
          {
            case CustomSpell.SphereDeFeuDeplacement:
            case CustomSpell.NueeDeDaguesDeplacement:
            case CustomSpell.RayonDeLuneDeplacement: break;

            default:

              var classInfo = caster.GetClassInfo(castingClass);
              var spellLevel = spell.MasterSpell is null ? spell.GetSpellLevelForClass(castingClass) : spell.MasterSpell.GetSpellLevelForClass(castingClass);

              if (spellLevel > 0)
                classInfo.SetRemainingSpellSlots(spellLevel, (byte)(classInfo.GetRemainingSpellSlots(spellLevel) - 1));

              break;
          }
        }

        onSpellAction.PreventSpellCast = true;
      }
      /*else if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.LenteurEffectTag))
      {
        onSpellAction.PreventSpellCast = true;
        _ = caster.ClearActionQueue();

        NwClass castingClass = onSpellAction.ClassIndex < 255 ? onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class : NwClass.FromClassId(CustomClass.Adventurer);

        if (!onSpellAction.IsAreaTarget)
          _ = caster.ActionCastSpellAt(spell, onSpellAction.TargetObject, spellClass: castingClass);
        else
          _ = caster.ActionCastSpellAt(spell, Location.Create(onSpellAction.Caster.Area, onSpellAction.TargetPosition, onSpellAction.Caster.Rotation), spellClass: castingClass);
      }*/
    }
  }
}
