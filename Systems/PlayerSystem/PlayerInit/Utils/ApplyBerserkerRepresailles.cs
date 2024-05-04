using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBerserkerRepresailles()
      {
        oid.LoginCreature.OnDamaged -= BarbarianUtils.OnDamagedBerserkerRepresailles;

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerRepresailles))
          oid.LoginCreature.OnDamaged += BarbarianUtils.OnDamagedBerserkerRepresailles;
      }
    }
  }
}
