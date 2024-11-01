using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDefensesEnjoleuses()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DefensesEnjoleuses) && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.DefensesEnjoleusesEffectTag))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.DefensesEnjoleuses));
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.CharmeImmunite));
        }
      }
    }
  }
}
