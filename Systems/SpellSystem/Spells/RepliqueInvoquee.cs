using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RepliqueInvoquee(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      foreach (var associate in caster.Associates.Where(e => e.Tag == EffectSystem.repliqueTag))
          associate.Unsummon();
        
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      EffectSystem.CreateRepliqueInvoquee(caster, targetLocation);

      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
