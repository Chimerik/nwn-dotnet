using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MetalBrulantEffectTag = "_METAL_BRULANT_EFFECT";
    private static ScriptCallbackHandle onIntervalMetalBrulantCallback;
    public static Effect MetalBrulant(Ability castAbility)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.MetalBrulant), Effect.RunAction(onIntervalHandle: onIntervalMetalBrulantCallback, interval:NwTimeSpan.FromRounds(1)));
      eff.Tag = MetalBrulantEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = (int)castAbility;
      return eff;
      
    }
    private static ScriptHandleResult OnIntervalMetalBrulant(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature && eventData.Effect.Creator is NwCreature caster
        && creature.Area == caster.Area && caster.DistanceSquared(creature) < 325
        && CreatureUtils.HandleBonusActionUse(caster))
      {
        var targetItem = creature.GetItemInSlot(InventorySlot.RightHand);
        bool droppable = false;

        if (targetItem is not null && ItemUtils.IsMetalWeapon(targetItem.BaseItem.ItemType))
          droppable = true;
        else
        {
          targetItem = creature.GetItemInSlot(InventorySlot.LeftHand);

          if (targetItem is not null && (ItemUtils.IsMetalWeapon(targetItem.BaseItem.ItemType) || ItemUtils.IsMetalShield(targetItem.BaseItem.ItemType)))
            droppable = true;
          else
          {
            targetItem = creature.GetItemInSlot(InventorySlot.Chest);

            if (targetItem is not null && targetItem.BaseACValue > 3)
              droppable = false;
            else
            {
              caster.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor(creature.Name)} ne porte pas d'objet métallique", ColorConstants.Orange);
              EffectUtils.RemoveTaggedEffect(creature, caster, MetalBrulantEffectTag);
              return ScriptHandleResult.Handled;
            }
          }
        }

        SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.MetalBrulant];

        SpellUtils.DealSpellDamage(creature, caster.CasterLevel, spellEntry, spellEntry.numDice, caster, 2);
        creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));

        int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.MetalBrulant), (Ability)eventData.Effect.IntParams[5]);

        if (droppable && CreatureUtils.GetSavingThrow(caster, creature, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
        {
          // Si le jet est raté, l'objet est déséquipé et ne peut pas être réquipé jusqu'à la fin du sort
          targetItem.GetObjectVariable<LocalVariableObject<NwGameObject>>("_METAL_BRULANT").Value = caster;

          creature.RunUnequip(targetItem);

          creature.OnItemEquip -= SpellSystem.OnEquipMetalBrulant;
          creature.OnItemEquip += SpellSystem.OnEquipMetalBrulant;

          EffectUtils.RemoveTaggedEffect(creature, caster, MetalBrulantEffectTag);

          NWScript.AssignCommand(caster, () => creature.ApplyEffect(EffectDuration.Temporary, MetalBrulantDesarmement, TimeSpan.FromSeconds(eventData.Effect.DurationRemaining)));
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
