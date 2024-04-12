using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAsmodeusPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.ProduceFlame, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ProduceFlame], this)))
          learnableSkills[CustomSkill.ProduceFlame].LevelUp(this);

        learnableSkills[CustomSkill.ProduceFlame].source.Add(Category.Race);

        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      }
    }
  }
}
