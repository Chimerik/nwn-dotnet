using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void OnSpellCastCancelStealth(OnSpellBroadcast onCast)
    {
      if (onCast.Caster.GetActionMode(ActionMode.Stealth))
      {
        if (Spells2da.spellTable.GetRow(onCast.Spell.Id).requiresVerbal
          || onCast.Spell.SpellType == Spell.AbilityBarbarianRage
          || onCast.Spell.Id == 411) // chant du barde
        {
          onCast.Caster.SetActionMode(ActionMode.Stealth, false);
          onCast.Caster.OnSpellBroadcast -= OnSpellCastCancelStealth;
        }
      }
      else
        onCast.Caster.OnSpellBroadcast -= OnSpellCastCancelStealth;
    }
  }
}
