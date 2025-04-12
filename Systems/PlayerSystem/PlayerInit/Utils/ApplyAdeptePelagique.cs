using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAdeptePelagique()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AdeptePelagique))
        {
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Nage(true));
        }
      }
    }
  }
}
