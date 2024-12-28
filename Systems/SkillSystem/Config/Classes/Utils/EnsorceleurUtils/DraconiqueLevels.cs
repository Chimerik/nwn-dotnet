using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static void HandleDraconiqueLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(9).SetPlayerOverride(player.oid, "Lignée Draconique");
          player.oid.SetTextureOverride("ensorceleur", "enso_draconique");

          player.LearnClassSkill(CustomSkill.EnsoResistanceDraconique);
          player.LearnAlwaysPreparedSpell(CustomSpell.ChangementDapparence, CustomClass.Ensorceleur);
          player.LearnAlwaysPreparedSpell(CustomSpell.OrbeChromatique, CustomClass.Ensorceleur);
          player.LearnAlwaysPreparedSpell(CustomSpell.Injonction, CustomClass.Ensorceleur);

          foreach (var ensoLevel in player.oid.LoginCreature.LevelInfo)
            if (ensoLevel.ClassInfo.Class.ClassType == ClassType.Sorcerer)
              ensoLevel.HitDie += 1;

          break;

        case 5:

          player.LearnAlwaysPreparedSpell((int)Spell.Fear, CustomClass.Ensorceleur);
          player.LearnAlwaysPreparedSpell(CustomSpell.Vol, CustomClass.Ensorceleur);

          break;

        case 6:

          if (!player.windows.TryGetValue("ensoDracoAffiniteElementaireSelection", out var style)) player.windows.Add("ensoDracoAffiniteElementaireSelection", new EnsoDracoAffiniteElementaireSelectionWindow(player));
          else ((EnsoDracoAffiniteElementaireSelectionWindow)style).CreateWindow();

          break;

        case 7:
          
          player.LearnAlwaysPreparedSpell(CustomSpell.OeilMagique, CustomClass.Ensorceleur);
          player.LearnAlwaysPreparedSpell((int)Spell.CharmMonster, CustomClass.Ensorceleur);

          break;

        case 9:
          player.LearnAlwaysPreparedSpell((int)Spell.EpicDragonKnight, CustomClass.Ensorceleur);
          player.LearnAlwaysPreparedSpell((int)Spell.LegendLore, CustomClass.Ensorceleur);
          break;

        case 11: player.LearnClassSkill(CustomSkill.EnsoDracoWings); break;
        case 18: player.LearnClassSkill(CustomSkill.EnsoCompagnonDraconique); break;
      }
    }
  }
}
