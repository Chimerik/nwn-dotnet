using Anvil.API;

namespace NWN.Systems
{
  public static partial class OccultisteUtils
  {
    public static async void RestoreLueurDeGuerison(NwCreature creature)
    {
      await NwTask.NextFrame();

      byte maxUses = (byte)(creature.GetClassInfo((ClassType)CustomClass.Occultiste).Level + 1);
      creature.SetFeatRemainingUses((Feat)CustomSkill.LueurDeGuérison, maxUses);
    }
  }
}
