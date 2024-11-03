using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FurieElementaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1)
        return;

      var damageType = spell.Id switch
      {
        CustomSpell.FurieElementaireFroid => DamageType.Cold,
        CustomSpell.FurieElementaireFoudre => DamageType.Electrical,
        _ => DamageType.Fire,
      };

      DamageBonus damage = clerc.Level > 13 ? DamageBonus.Plus2d8 : DamageBonus.Plus1d8;
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FurieElementaire(damage, damageType));
    }
  }
}
