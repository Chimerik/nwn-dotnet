using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AspectSauvage(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.AspectSauvageEffectTag); 
      oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.AspectSauvageChouette(spell.Id));
    }
  }
}
