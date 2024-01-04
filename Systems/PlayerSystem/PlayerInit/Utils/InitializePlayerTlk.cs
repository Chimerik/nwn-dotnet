using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializePlayerTlk()
      {
        if(learnableSkills.ContainsKey(CustomSkill.FighterArcaneArcher))
          new StrRef(8).SetPlayerOverride(oid, "Archer-Mage");
        else if (learnableSkills.ContainsKey(CustomSkill.FighterChampion))
          new StrRef(8).SetPlayerOverride(oid, "Champion");
        else if (learnableSkills.ContainsKey(CustomSkill.FighterWarMaster))
          new StrRef(8).SetPlayerOverride(oid, "Maître de Guerre");
      }
    }
  }
}
