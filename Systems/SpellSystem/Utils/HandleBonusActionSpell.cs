﻿using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool HandleBonusActionSpells(NwCreature caster, SpellEntry spellEntry, SpellEvents.OnSpellCast spellCast)
    {
      if (!spellEntry.isBonusAction)
        return true;

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return false;

        switch(NwSpell.FromSpellId(spellEntry.RowIndex).SpellType)
        {
          case Spell.AbilityBarbarianRage: break;
          
          default:

          foreach (var eff in caster.ActiveEffects)
            switch(eff.Tag)
            {
              case EffectSystem.ShieldArmorDisadvantageEffectTag:
                caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort en étant équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas", ColorConstants.Red);
                return false;
              case EffectSystem.BarbarianRageEffectTag:
                caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort sous l'effet de la rage", ColorConstants.Red);
                return false;
            }

            break;
        }

        NwItem offHand = caster.GetItemInSlot(InventorySlot.LeftHand);
        NwItem rightHand = caster.GetItemInSlot(InventorySlot.RightHand);

        if (spellEntry.requiresSomatic
          && !caster.KnowsFeat((Feat)CustomSkill.MageDeGuerre)
          && rightHand is not null && (offHand is not null && ItemUtils.IsTwoHandedWeapon(rightHand.BaseItem, caster.Size)))
        {
          caster.ControllingPlayer?.SendServerMessage($"Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
          return false;
        }

      SpellSystem.OnSpellCastAbjurationWard(caster, spellCast);

      LogUtils.LogMessage($"Sort lancé en tant qu'action bonus", LogUtils.LogType.Combat);

      return true;
    }
  }
}
