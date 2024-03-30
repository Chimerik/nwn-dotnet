using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMobile()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Mobile))
        {
          if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.mobileEffectTag))
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.mobile);

          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackMobile;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackMobile;
        }
      }
    }
  }
}
