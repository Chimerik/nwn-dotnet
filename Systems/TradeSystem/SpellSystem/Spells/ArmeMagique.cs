using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ArmeMagique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);

      if (oTarget is not NwItem target || !ItemUtils.IsWeapon(target.BaseItem))
        return new List<NwGameObject>();

      var damageType = target.BaseItem.WeaponType.FirstOrDefault() switch
      {
        DamageType.Piercing => IPDamageType.Piercing,
        DamageType.Bludgeoning => IPDamageType.Bludgeoning,
        _ => IPDamageType.Slashing,
      };

      NWScript.AssignCommand(oCaster, () => target.AddItemProperty(ItemProperty.AttackBonus(1), EffectDuration.Temporary, NwTimeSpan.FromRounds(spellEntry.duration)));
      NWScript.AssignCommand(oCaster, () => target.AddItemProperty(ItemProperty.DamageBonus(damageType,IPDamageBonus.Plus1), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
         
      return new List<NwGameObject>() { oTarget };
    }
  }
}
