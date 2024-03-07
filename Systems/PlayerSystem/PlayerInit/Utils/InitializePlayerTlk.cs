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
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianBerseker))
          new StrRef(5213).SetPlayerOverride(oid, "Berseker");
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianTotem))
          new StrRef(5213).SetPlayerOverride(oid, "Voie du Totem");
        else if (learnableSkills.ContainsKey(CustomSkill.BarbarianWildMagic))
          new StrRef(5213).SetPlayerOverride(oid, "Voie de la Magie Sauvage");
        else if (learnableSkills.ContainsKey(CustomSkill.RogueThief))
          new StrRef(16).SetPlayerOverride(oid, "Voleur");
      }
    }
  }
}
