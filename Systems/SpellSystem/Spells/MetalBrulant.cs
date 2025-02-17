using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MetalBrulant(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
        
        foreach (var target in targets)
        {
          if (target is not NwCreature targetCreature)
            continue;

          var targetItem = targetCreature.GetItemInSlot(InventorySlot.RightHand);
          bool droppable = false;

          if(targetItem is not null && ItemUtils.IsMetalWeapon(targetItem.BaseItem.ItemType))
            droppable = true;
          else
          {
            targetItem = targetCreature.GetItemInSlot(InventorySlot.LeftHand);

            if (targetItem is not null && (ItemUtils.IsMetalWeapon(targetItem.BaseItem.ItemType) || ItemUtils.IsMetalShield(targetItem.BaseItem.ItemType)))
              droppable = true;
            else
            {
              targetItem = targetCreature.GetItemInSlot(InventorySlot.Chest);

              if (targetItem is not null && targetItem.BaseACValue > 3)
                droppable = false;
              else
              {
                caster.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor(target.Name)} ne porte pas d'objet métallique", ColorConstants.Orange);
                continue;
              }
            }
          }

          SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, spellEntry.numDice, oCaster, spell.GetSpellLevelForClass(casterClass));
          targetCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));

          int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

          if (droppable && CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          {
            // Si le jet est raté, l'objet est déséquipé et ne peut pas être réquipé jusqu'à la fin du sort
            targetItem.GetObjectVariable<LocalVariableObject<NwGameObject>>("_METAL_BRULANT").Value = oCaster;

            targetCreature.RunUnequip(targetItem);

            targetCreature.OnItemEquip -= OnEquipMetalBrulant;
            targetCreature.OnItemEquip += OnEquipMetalBrulant;

            EffectUtils.RemoveTaggedEffect(targetCreature, oCaster, EffectSystem.MetalBrulantEffectTag);

            NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.MetalBrulantDesarmement, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          }
          else
          {
            // Si l jet est réussi on réapplique les dégâts et le jet chaque round
            NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.MetalBrulant(spell, casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          }
        }
      }

      return targets;
    }

    public static void OnEquipMetalBrulant(OnItemEquip onEquip)
    {
      if (onEquip.Item.GetObjectVariable<LocalVariableObject<NwGameObject>>("_METAL_BRULANT").HasValue)
      {
        onEquip.EquippedBy.ControllingPlayer?.SendServerMessage("Métal Brûlant : Impossible de réequiper pour le moment", ColorConstants.Red);
        onEquip.PreventEquip = true;
      }
    }
  }
}
