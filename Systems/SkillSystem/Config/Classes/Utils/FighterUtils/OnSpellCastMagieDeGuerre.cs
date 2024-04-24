using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnSpellCastMagieDeGuerre(OnSpellAction onSpellAction)
    {
      if((onSpellAction.Spell.GetSpellLevelForClass((ClassType)CustomClass.EldritchKnight) < 1 
        || onSpellAction.Caster.GetClassInfo(ClassType.Fighter).Level > 17)
        && onSpellAction.Caster.CurrentAction == Action.AttackObject
        && !CreatureUtils.HandleBonusActionUse(onSpellAction.Caster))
      {
        NwCreature caster = onSpellAction.Caster;
        SpellEntry spellEntry = Spells2da.spellTable[onSpellAction.Spell.Id];

        switch (NwSpell.FromSpellId(spellEntry.RowIndex).SpellType)
        {
          case Spell.AbilityBarbarianRage: break;

          default:

            foreach (var eff in caster.ActiveEffects)
              switch (eff.Tag)
              {
                case EffectSystem.ShieldArmorDisadvantageEffectTag:
                  caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort en étant équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas", ColorConstants.Red);
                  return;
                case EffectSystem.BarbarianRageEffectTag:
                  caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort sous l'effet de la rage", ColorConstants.Red);
                  return;
              }

            break;
        }

        NwItem offHand = caster.GetItemInSlot(InventorySlot.LeftHand);
        NwItem rightHand = caster.GetItemInSlot(InventorySlot.RightHand);

        if (spellEntry.requiresSomatic
          && !caster.KnowsFeat((Feat)CustomSkill.MageDeGuerre)
          && rightHand is not null && (offHand is not null && ItemUtils.IsTwoHandedWeapon(rightHand.BaseItem, caster.Size)))
        {
          caster.ControllingPlayer?.SendServerMessage("Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
          return;
        }
        if (!(onSpellAction.Spell.Id == CustomSpell.FlameBlade && caster.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString).Value == CustomSpell.FlameBlade) // TODO : Si on recast Flame Blade, alors on ne compte pas un nouvel emplacement de sort
        && spellEntry.requiresConcentration
        && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.ConcentrationEffectTag))
          SpellUtils.DispelConcentrationEffects(caster);

        LogUtils.LogMessage("Sort lancé en tant qu'action bonus", LogUtils.LogType.Combat);
        
        SpellUtils.SpellSwitch(caster, onSpellAction.Spell, spellEntry, onSpellAction.TargetObject, Location.Create(onSpellAction.Caster.Area, onSpellAction.Caster.Position, onSpellAction.Caster.Rotation), onSpellAction.Caster.Classes[onSpellAction.ClassIndex].Class);
        
        onSpellAction.PreventSpellCast = true;
      }
    }
  }
}
