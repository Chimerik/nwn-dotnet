using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAdepteAlpin()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AdepteAlpin))
        {
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Escalade(true));
        }
      }
    }
  }
}
