using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyConstitutionInfernale()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ConstitutionInfernale))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ConstitutionInfernale));
      }
    }
  }
}
