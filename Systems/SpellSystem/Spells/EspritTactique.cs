
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void EspritTactique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Esprit Tactique - Jet +{NwRandom.Roll(Utils.random, spellEntry.damageDice).ToString().ColorString(ColorConstants.White)}", StringUtils.gold, true, true);
    }
  }
}
