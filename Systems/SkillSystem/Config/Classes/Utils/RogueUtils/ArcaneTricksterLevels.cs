using Anvil.API;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleArcaneTricksterLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Escroc Arcanique");
          player.oid.SetTextureOverride("rogue", "arcane_trickster");

          int classPosition;
          for (classPosition = 0; classPosition < player.oid.LoginCreature.Classes.Count; classPosition++)
            if (player.oid.LoginCreature.Classes[classPosition].Class.ClassType == ClassType.Rogue)
              break;

          CreaturePlugin.SetClassByPosition(player.oid.LoginCreature, classPosition, CustomClass.RogueArcaneTrickster);

          player.LearnAlwaysPreparedSpell(CustomSpell.MageHand, CustomClass.RogueArcaneTrickster);

          if (!player.windows.TryGetValue("spellSelection", out var cantrip1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 2, 3));
          else ((SpellSelectionWindow)cantrip1).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 2, 3);

          break;

        case 4:

          if (!player.windows.TryGetValue("spellSelection", out var spell4)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell4).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 7:

          if (!player.windows.TryGetValue("spellSelection", out var spell7)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell7).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 8:

          if (!player.windows.TryGetValue("spellSelection", out var spell8)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell8).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 9: player.LearnClassSkill(CustomSkill.ArcaneTricksterMagicalAmbush); break;

        case 10:

          if (!player.windows.TryGetValue("spellSelection", out var spell10)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 1, 1));
          else ((SpellSelectionWindow)spell10).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 1, 1);

          break;

        case 11:

          if (!player.windows.TryGetValue("spellSelection", out var spell11)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell11).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 13:

          if (!player.windows.TryGetValue("spellSelection", out var spell13)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell13).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          player.LearnClassSkill(CustomSkill.ArcaneTricksterPolyvalent);

          break;

        case 14:

          if (!player.windows.TryGetValue("spellSelection", out var spell14)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell14).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 16:

          if (!player.windows.TryGetValue("spellSelection", out var spell16)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell16).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;


        case 19:

          if (!player.windows.TryGetValue("spellSelection", out var spell19)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell19).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;

        case 20:

          if (!player.windows.TryGetValue("spellSelection", out var spell20)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, (ClassType)CustomClass.RogueArcaneTrickster, 0, 1));
          else ((SpellSelectionWindow)spell20).CreateWindow((ClassType)CustomClass.RogueArcaneTrickster, 0, 1);

          break;
      }
    }
  }
}
