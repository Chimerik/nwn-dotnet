using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool HandleBonusActionSpells(NwCreature caster, SpellEntry spellEntry)
    {
      if (!spellEntry.isBonusAction)
        return true;

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return false!;

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
          && !caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MageDeGuerre))
          && rightHand is not null && (offHand is not null && ItemUtils.IsTwoHandedWeapon(rightHand.BaseItem, caster.Size)))
        {
          caster.ControllingPlayer?.SendServerMessage($"Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
          return false;
        }

        LogUtils.LogMessage($"Sort lancé en tant qu'action bonus", LogUtils.LogType.Combat);

      return true;
    }
  }
}
