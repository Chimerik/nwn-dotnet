using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterAddCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1285, HandleHealthPoints },
            { 1286, HandleHealthPoints },
            { 1287, HandleHealthPoints },
            { 1288, HandleHealthPoints },
            { 1289, HandleHealthPoints },
    };

    public static Dictionary<int, Func<PlayerSystem.Player, int, int>> RegisterRemoveCustomFeatEffect = new Dictionary<int, Func<PlayerSystem.Player, int, int>>
    {
            { 1130, HandleRemoveStrengthMalusFeat },
    };

    private static int HandleHealthPoints(PlayerSystem.Player player, int feat)
    {
      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, CreaturePlugin.GetMaxHitPointsByLevel(player.oid, 1) + 1 + (3 * CreaturePlugin.GetRawAbilityScore(player.oid, NWScript.ABILITY_CONSTITUTION) + CreaturePlugin.GetKnowsFeat(player.oid, (int)Feat.Toughness)));
      return 0;
    }

    private static int HandleRemoveStrengthMalusFeat(PlayerSystem.Player player, int idMalusFeat)
    {
      player.removeableMalus.Remove(idMalusFeat);
      CreaturePlugin.SetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH, CreaturePlugin.GetRawAbilityScore(player.oid, NWScript.ABILITY_STRENGTH) + 2);

      return 0;
    }

    public static Feat[] forgeBasicSkillBooks = new Feat[] { Feat.Metallurgy, Feat.Miner, Feat.Prospection, Feat.StripMiner, Feat.Reprocessing, Feat.Forge, Feat.CraftScaleMail, Feat.CraftDagger, Feat.CraftLightMace, Feat.CraftMorningStar, Feat.CraftSickle, Feat.CraftShortSpear };
    public static Feat[] craftSkillBooks = new Feat[] { Feat.Metallurgy, Feat.AdvancedCraft, Feat.Miner, Feat.Geology, Feat.Prospection, Feat.VeldsparReprocessing, Feat.ScorditeReprocessing, Feat.PyroxeresReprocessing, Feat.StripMiner, Feat.Reprocessing, Feat.ReprocessingEfficiency, Feat.Connections, Feat.Forge };
    public static Feat[] languageSkillBooks = new Feat[] { Feat.LanguageAbyssal, Feat.LanguageCelestial, Feat.LanguageDeep, Feat.LanguageDraconic, Feat.LanguageDruidic, Feat.LanguageDwarf, Feat.LanguageElf, Feat.LanguageGiant, Feat.LanguageGoblin, Feat.LanguageHalfling, Feat.LanguageInfernal, Feat.LanguageOrc, Feat.LanguagePrimodial, Feat.LanguageSylvan, Feat.LanguageThieves };
  }
}
