using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static async void RestoreBarbarianRage(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Barbarian)?.Level;

      if (!level.HasValue)
        return;

      await NwTask.NextFrame();

      if(level < 3)
        creature.SetFeatRemainingUses(Feat.BarbarianRage, 2);
      else if(level < 6)
        creature.SetFeatRemainingUses(Feat.BarbarianRage, 3);
      else if (level < 12)
        creature.SetFeatRemainingUses(Feat.BarbarianRage, 4);
      else if (level < 17)
        creature.SetFeatRemainingUses(Feat.BarbarianRage, 5);
      else
        creature.SetFeatRemainingUses(Feat.BarbarianRage, 6);

      creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
    }
  }
}
