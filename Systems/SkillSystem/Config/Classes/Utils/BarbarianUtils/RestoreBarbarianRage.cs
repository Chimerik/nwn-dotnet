using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static async void RestoreBarbarianRage(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian))?.Level;

      if (!level.HasValue)
        return;

      await NwTask.NextFrame();

      if(level < 3)
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 2);
      else if(level < 6)
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 2);
      else if (level < 12)
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 4);
      else if (level < 17)
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 5);
      else if (level < 20)
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 6);
      else
        creature.SetFeatRemainingUses(NwFeat.FromFeatType(Feat.BarbarianRage), 1);

      creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
    }
  }
}
