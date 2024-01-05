using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyGoldDwarfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.LightHammerProficiency], this)))
          learnableSkills[CustomSkill.LightHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.LightHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarHammerProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarHammerProficiency], this)))
          learnableSkills[CustomSkill.WarHammerProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarHammerProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.HandAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HandAxeProficiency], this)))
          learnableSkills[CustomSkill.HandAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.HandAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.WarAxeProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.WarAxeProficiency], this)))
          learnableSkills[CustomSkill.WarAxeProficiency].LevelUp(this);

        learnableSkills[CustomSkill.WarAxeProficiency].source.Add(Category.Race);

        if (learnableSkills.TryAdd(CustomSkill.Nain, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Nain], this)))
          learnableSkills[CustomSkill.Nain].LevelUp(this);

        learnableSkills[CustomSkill.Nain].source.Add(Category.Race);

        oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.dwarfSlow);
        oid.LoginCreature.AddFeat(NwFeat.FromFeatType(Feat.Toughness));
        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity((IPDamageType)CustomItemPropertiesDamageType.Poison, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      }
    }
  }
}
