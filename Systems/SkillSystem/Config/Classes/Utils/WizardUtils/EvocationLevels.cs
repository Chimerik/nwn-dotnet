using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Wizard
  {
    public static void HandleEvocationLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(20).SetPlayerOverride(player.oid, "Evocateur");
          player.oid.SetTextureOverride("wizard", "evocation");

          player.LearnClassSkill(CustomSkill.EvocateurFaconneurDeSorts);

          if (!player.windows.TryGetValue("spellSelection", out var spell1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 2, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Wizard, 2, SpellSchool.Evocation);

          break;

        case 4:

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell4).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 5:

          if (!player.windows.TryGetValue("spellSelection", out var spell5)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell5).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.EvocateurToursPuissants);
          
          if (!player.windows.TryGetValue("spellSelection", out var spell6)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell6).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);
          
          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell7).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 8:

          if (!player.windows.TryGetValue("spellSelection", out var spell8)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell8).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 9:

          if (!player.windows.TryGetValue("spellSelection", out var spell9)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell9).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 10:

          player.LearnClassSkill(CustomSkill.EvocateurSuperieur);

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell10).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell11).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 12:

          if (!player.windows.TryGetValue("spellSelection", out var spell12)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell12).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell13).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 14:

          player.LearnClassSkill(CustomSkill.EvocateurSurcharge);

          if (!player.windows.TryGetValue("spellSelection", out var spell14)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell14).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 15:

          if (!player.windows.TryGetValue("spellSelection", out var spell15)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell15).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 16:

          if (!player.windows.TryGetValue("spellSelection", out var spell16)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell16).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 17:

          if (!player.windows.TryGetValue("spellSelection", out var spell17)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell17).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 18:

          if (!player.windows.TryGetValue("spellSelection", out var spell18)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell18).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 19:

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell19).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;

        case 20:

          if (!player.windows.TryGetValue("spellSelection", out var spell20)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Wizard, 1, SpellSchool.Evocation));
          else ((SpellSelectionWindow)spell20).CreateWindow(ClassType.Wizard, 1, SpellSchool.Evocation);

          break;
      }
    }
  }
}
