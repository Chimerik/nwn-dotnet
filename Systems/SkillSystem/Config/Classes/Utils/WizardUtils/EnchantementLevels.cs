using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleEnchantementLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Enchanteur");
          player.oid.SetTextureOverride("wizard", "enchantement");

          player.LearnClassSkill(CustomSkill.EnchantementRegardHypnotique);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell1)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 2, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Wizard, 2, SpellSchool.Enchantment);

          break;

        case 4:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell4)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 5:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell5)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.EnchantementCharmeInstinctif);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell6)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell6).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 7:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell7)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 8:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell8)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell8).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 9:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell9)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 10:

          player.LearnClassSkill(CustomSkill.EnchantementPartage);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell10)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 11:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell11)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 12:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell12)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell12).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 13:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell13)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 14:

          player.LearnClassSkill(CustomSkill.EnchantementAlterationMemorielle);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell14)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell14).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 15:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell15)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 16:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell16)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell16).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 17:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell17)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 18:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell18)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell18).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 19:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell19)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;

        case 20:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell20)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Enchantment));
          else ((SpellSelectionWindow)spell20).CreateWindow(ClassType.Wizard, 1, SpellSchool.Enchantment);

          break;
      }
    }
  }
}
