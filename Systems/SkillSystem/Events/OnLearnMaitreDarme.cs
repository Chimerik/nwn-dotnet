using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMaitreDarme(PlayerSystem.Player player, int customSkillId)
    {
      foreach (var learnable in player.learnableSkills.Values.Where(s => s.category == Category.WeaponProficiency && s.currentLevel < 1))
        learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

      List<int> weaponProficiencies = new();

      if (!player.oid.LoginCreature.KnowsFeat(Feat.WeaponProficiencySimple))
      {
        if (!player.learnableSkills.ContainsKey(CustomSkill.ClubProficiency))
          weaponProficiencies.Add(CustomSkill.ClubProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.DaggerProficiency))
          weaponProficiencies.Add(CustomSkill.DaggerProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.HandAxeProficiency))
          weaponProficiencies.Add(CustomSkill.HandAxeProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.LightHammerProficiency))
          weaponProficiencies.Add(CustomSkill.LightHammerProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.LightMaceProficiency))
          weaponProficiencies.Add(CustomSkill.LightMaceProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.QuarterstaffProficiency))
          weaponProficiencies.Add(CustomSkill.QuarterstaffProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.SickleProficiency))
          weaponProficiencies.Add(CustomSkill.SickleProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.LightCrossbowProficiency))
          weaponProficiencies.Add(CustomSkill.LightCrossbowProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.DartProficiency))
          weaponProficiencies.Add(CustomSkill.DartProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.MagicStaffProficiency))
          weaponProficiencies.Add(CustomSkill.MagicStaffProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.SlingProficiency))
          weaponProficiencies.Add(CustomSkill.SlingProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.SpearProficiency))
          weaponProficiencies.Add(CustomSkill.SpearProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.ShortBowProficiency))
          weaponProficiencies.Add(CustomSkill.ShortBowProficiency);
      }

      if (!player.oid.LoginCreature.KnowsFeat(Feat.WeaponProficiencyMartial))
      {
        if (!player.learnableSkills.ContainsKey(CustomSkill.LightFlailProficiency))
          weaponProficiencies.Add(CustomSkill.LightFlailProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.MorningstarProficiency))
          weaponProficiencies.Add(CustomSkill.MorningstarProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.BattleaxeProficiency))
          weaponProficiencies.Add(CustomSkill.BattleaxeProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.GreataxeProficiency))
          weaponProficiencies.Add(CustomSkill.GreataxeProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.GreatswordProficiency))
          weaponProficiencies.Add(CustomSkill.GreatswordProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.ScimitarProficiency))
          weaponProficiencies.Add(CustomSkill.ScimitarProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.HalberdProficiency))
          weaponProficiencies.Add(CustomSkill.HalberdProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.HeavyFlailProficiency))
          weaponProficiencies.Add(CustomSkill.HeavyFlailProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.ThrowingAxeProficiency))
          weaponProficiencies.Add(CustomSkill.ThrowingAxeProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.TridentProficiency))
          weaponProficiencies.Add(CustomSkill.TridentProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.WarHammerProficiency))
          weaponProficiencies.Add(CustomSkill.WarHammerProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.HeavyCrossbowProficiency))
          weaponProficiencies.Add(CustomSkill.HeavyCrossbowProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.RapierProficiency))
          weaponProficiencies.Add(CustomSkill.RapierProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.ShortSwordProficiency))
          weaponProficiencies.Add(CustomSkill.ShortSwordProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.LongSwordProficiency))
          weaponProficiencies.Add(CustomSkill.LongSwordProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.LongBowProficiency))
          weaponProficiencies.Add(CustomSkill.LongBowProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.ShurikenProficiency))
          weaponProficiencies.Add(CustomSkill.ShurikenProficiency);

        if (!player.learnableSkills.ContainsKey(CustomSkill.WhipProficiency))
          weaponProficiencies.Add(CustomSkill.WhipProficiency);
      }

      if (weaponProficiencies.Count > 0)
      {
        if (!player.windows.TryGetValue("weaponBonusChoice", out var value)) player.windows.Add("weaponBonusChoice", new WeaponBonusChoiceWindow(player, weaponProficiencies));
        else ((WeaponBonusChoiceWindow)value).CreateWindow(weaponProficiencies);
      }

      List<Ability> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Strength) < 20)
        abilities.Add(Ability.Strength);

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(Ability.Dexterity);

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
