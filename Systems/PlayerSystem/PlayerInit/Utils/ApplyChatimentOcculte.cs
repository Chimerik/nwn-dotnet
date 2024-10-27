using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyChatimentOcculte()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentOcculte))
          oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, (byte)(oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).GetRemainingSpellSlots(1) - 1));
      }
    }
  }
}
