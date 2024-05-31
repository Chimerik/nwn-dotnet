using Anvil.API;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static void RestoreInspirationBardique(NwCreature creature)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.BardInspiration))
      {
        int chaMod = creature.GetAbilityModifier(Ability.Charisma) > 0 ? creature.GetAbilityModifier(Ability.Charisma) : 1;
        creature.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, (byte)chaMod);

        if (creature.KnowsFeat((Feat)CustomSkill.DefenseVaillante))
          creature.SetFeatRemainingUses((Feat)CustomSkill.DefenseVaillante, (byte)chaMod);

        if (creature.KnowsFeat((Feat)CustomSkill.DegatsVaillants))
          creature.SetFeatRemainingUses((Feat)CustomSkill.DegatsVaillants, (byte)chaMod);

        if (creature.KnowsFeat((Feat)CustomSkill.BotteDefensive))
          creature.SetFeatRemainingUses((Feat)CustomSkill.BotteDefensive, (byte)chaMod);

        if (creature.KnowsFeat((Feat)CustomSkill.BotteTranchante))
          creature.SetFeatRemainingUses((Feat)CustomSkill.BotteTranchante, (byte)chaMod);

        foreach (var target in NwObject.FindObjectsOfType<NwCreature>())
          EffectUtils.RemoveTaggedEffect(target, creature, EffectSystem.InspirationBardiqueEffectTag);
      }
    }
  }
}
