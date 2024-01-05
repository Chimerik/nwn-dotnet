using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyZarielPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Thaumaturgy, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Thaumaturgy], this)))
          learnableSkills[CustomSkill.Thaumaturgy].LevelUp(this);

        learnableSkills[CustomSkill.Thaumaturgy].source.Add(Category.Race);

        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      }
    }
  }
}
