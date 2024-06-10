using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetSkillEffectBonus(NwCreature creature, int skill)
    {
      if (skill != CustomSkill.StealthProficiency) // Simplification parce que le cas n'existe que pour Stealth pour le moment, mais à retirer lorsqu'il y aura d'autres cas
        return 0;

      int bonusScore = 0;

      foreach (var eff in creature.ActiveEffects)
      {
        if(skill == CustomSkill.StealthProficiency)
        {
          if (eff.EffectType == EffectType.SkillIncrease && eff.IntParams[0] == 8) // 8 = Move silently skill
          {
            if (eff.Tag == EffectSystem.WolfAspectEffectTag)
            {
              bonusScore += creature.GetAbilityModifier(Ability.Dexterity);
              LogUtils.LogMessage($"Totem - Aspect du loup : {creature.GetAbilityModifier(Ability.Dexterity)}", LogUtils.LogType.Combat);
            }
            else
            {
              bonusScore += eff.IntParams[1];
              LogUtils.LogMessage($"Bonus d'effet d'augmentation de compétence : {eff.IntParams[1]}", LogUtils.LogType.Combat);
            }
          }
        }
      }

      return bonusScore;
    }
  }
}
