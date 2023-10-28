using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyStrongHeartPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Halfelin, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Halfelin])))
          learnableSkills[CustomSkill.Halfelin].LevelUp(this);

        learnableSkills[CustomSkill.Halfelin].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);

        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)CustomItemPropertiesDamageType.Poison, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);

        // TODO : Penser à gérer le rejeu des jets de compétences en cas de 1 
      }
    }
  }
}
