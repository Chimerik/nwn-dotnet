using Anvil.API;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void ConsumeOathCharge(NwCreature creature)
    { 
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionArmeSacree, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionSaintesRepresailles, 0);
      creature.SetFeatRemainingUses((Feat)CustomSkill.DevotionRenvoiDesImpies, 0);
    }
  }
}
