using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnChanceDebordante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChanceDebordanteAuraEffectTag))
      {
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.chanceDebordanteAura(player.oid.LoginCreature));
        UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(10);
      }

      List<NuiComboEntry> abilities = new();

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Dexterity) < 20)
        abilities.Add(new("Dextérité", (int)Ability.Dexterity));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Constitution) < 20)
        abilities.Add(new("Constitution", (int)Ability.Constitution));

      if (player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma) < 20)
        abilities.Add(new("Charisme", (int)Ability.Charisma));

      if (abilities.Count > 0)
      {
        if (!player.windows.TryGetValue("abilityBonusChoice", out var value)) player.windows.Add("abilityBonusChoice", new AbilityBonusChoiceWindow(player, abilities));
        else ((AbilityBonusChoiceWindow)value).CreateWindow(abilities);
      }

      return true;
    }
  }
}
