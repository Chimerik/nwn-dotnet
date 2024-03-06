using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Parade(NwCreature caster)
    {
      FeatUtils.ClearPreviousManoeuvre(caster);

      int warMasterLevel = caster.GetClassInfo(NwClass.FromClassId(CustomClass.Fighter)).Level;
      int superiorityDice = warMasterLevel > 17 ? 12 : warMasterLevel > 9 ? 10 : 8;

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreTypeVariable).Value = CustomSkill.WarMasterParade;
      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.ManoeuvreDiceVariable).Value = superiorityDice;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Parade", StringUtils.gold);
      FeatUtils.DecrementManoeuvre(caster);
    }
  }
}
