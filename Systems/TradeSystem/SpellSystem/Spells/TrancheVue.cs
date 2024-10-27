using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void TrancheVue(NwGameObject oCaster)
    {
      if(oCaster is NwCreature caster)
        NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.TrancheVue(caster), NwTimeSpan.FromRounds(1)));
    }
  }
}
