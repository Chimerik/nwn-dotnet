﻿using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleInvocationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Invocateur");
          player.oid.SetTextureOverride("wizard", "invocation");

          player.LearnClassSkill(CustomSkill.EvocateurFaconneurDeSorts);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell1)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 2, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Wizard, 2, SpellSchool.Conjuration);

          break;

        case 4:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell4)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 5:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell5)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.InvocationPermutation);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell6)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell6).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 7:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell7)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 8:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell8)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell8).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 9:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell9)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 10:

          player.LearnClassSkill(CustomSkill.InvocationConcentration);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell10)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 11:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell11)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 12:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell12)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell12).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 13:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell13)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 14:

          player.LearnClassSkill(CustomSkill.InvocationSupreme);

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell14)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell14).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 15:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell15)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 16:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell16)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell16).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 17:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell17)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 18:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell18)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell18).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 19:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell19)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;

        case 20:

          if (!player.windows.TryGetValue("schoolSpellSelection", out var spell20)) player.windows.Add("schoolSpellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Conjuration));
          else ((SpellSelectionWindow)spell20).CreateWindow(ClassType.Wizard, 1, SpellSchool.Conjuration);

          break;
      }
    }
  }
}
