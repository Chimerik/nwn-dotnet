using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> LameArdente(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return null;

      if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.LameArdenteEffectTag))
      {
        // Le sort est gratuit et ne consomme pas d'emplacement
        caster.GetObjectVariable<LocalVariableInt>("_FREE_SPELL").Value = 1;

        // si la lame n'est pas est équipée on créé la lame
        NwItem lameArdente = caster.GetItemInSlot(InventorySlot.RightHand);

        if (lameArdente is null || lameArdente.Tag != "_TEMP_FLAME_BLADE")
        {
          CreateFlameBlade(caster, spell.SpellType);
        }    
        else if (oTarget == oCaster)
        {
          // si la lame est équipée, on fait disparaître la lame
          caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseFire));
          lameArdente.Destroy();
          caster.OnItemUnequip -= OnUnequipFlameBlade;
        }
        else
        {
          // On inflige les dégâts
          SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
          int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

          switch (SpellUtils.GetSpellAttackRoll(oTarget, oCaster, spell, casterClass.SpellCastingAbility, 0))
          {
            case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
            case TouchAttackResult.Hit: break;
            default: return new List<NwGameObject> { caster };
          }

          oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));
          SpellUtils.DealSpellDamage(oTarget, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));
        }
      }
      else
      {
        // on créé la lame et on applique l'effet
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.LameArdente, SpellUtils.GetSpellDuration(caster, spellEntry));
        CreateFlameBlade(caster, spell.SpellType);
      }
      
      return new List<NwGameObject> { caster };
    }

    private static async void CreateFlameBlade(NwCreature caster, Spell spellType)
    {
      SpellUtils.SignalEventSpellCast(caster, caster, spellType);
      NwItem blade = await NwItem.Create(BaseItems2da.baseItemTable[(int)BaseItemType.Scimitar].craftedItem, caster, 1, "_TEMP_FLAME_BLADE");
      blade.AddItemProperty(ItemProperty.NoDamage(), EffectDuration.Permanent);
      blade.AddItemProperty(ItemProperty.Light(IPLightBrightness.Low, IPLightColor.Orange), EffectDuration.Permanent);
      caster.RunEquip(blade, InventorySlot.RightHand);
      caster.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseFire));
      caster.OnItemUnequip -= OnUnequipFlameBlade;
      caster.OnItemUnequip += OnUnequipFlameBlade;
    }

    private static void OnUnequipFlameBlade(OnItemUnequip onUnEquip)
    {
      if (!onUnEquip.Item.IsValid || onUnEquip.Item.Tag != "_TEMP_FLAME_BLADE")
        return;

      onUnEquip.Item.Destroy();
    }
  }
}
