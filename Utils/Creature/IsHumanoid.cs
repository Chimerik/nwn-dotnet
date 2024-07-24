using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool IsHumanoid(NwCreature creature)
    {
      if(creature.IsPlayableRace  || Utils.In(creature.Race.RacialType, RacialType.HumanoidReptilian, RacialType.HumanoidMonstrous, RacialType.HumanoidGoblinoid, RacialType.HumanoidOrc))
        return true;
      else return false;
    }
  }
}
