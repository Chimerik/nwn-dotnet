namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeWarMasterImplacable()
      {
        if (learnableSkills.TryGetValue(CustomSkill.FighterWarMaster, out LearnableSkill learnable) && learnable.currentLevel > 14)
        {
          oid.OnCombatStatusChange -= FighterUtils.OnCombatWarMasterRecoverManoeuvre;
          oid.OnCombatStatusChange += FighterUtils.OnCombatWarMasterRecoverManoeuvre;
        }
      }
    }
  }
}
