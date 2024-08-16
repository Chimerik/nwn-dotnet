using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCoeurDeLaTempete()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoCoeurDeLaTempete) 
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CoeurDeLaTempeteEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.CoeurDeLaTempete));
      }
    }
  }
}
