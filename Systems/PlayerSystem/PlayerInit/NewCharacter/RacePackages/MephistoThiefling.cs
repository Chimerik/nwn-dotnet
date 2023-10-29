using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMephistoPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.FlameBlade, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FlameBlade])))
          learnableSkills[CustomSkill.FlameBlade].LevelUp(this);

        learnableSkills[CustomSkill.FlameBlade].source.Add(Category.Race);

        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      
        // TODO : Penser à gérer les sorts 
        // Level 1 : Mage Hand
        // Level 3 : Mains brûlantes
        // Level 5 : Flame Blade
      }
    }
  }
}
