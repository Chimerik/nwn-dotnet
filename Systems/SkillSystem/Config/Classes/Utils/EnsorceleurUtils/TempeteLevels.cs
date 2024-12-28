using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Ensorceleur
  {
    public static void HandleTempeteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(9).SetPlayerOverride(player.oid, "Sorcellerie de la Tempête");
          player.oid.SetTextureOverride("ensorceleur", "enso_draconique");

          player.LearnClassSkill(CustomSkill.EnsoMagieTempetueuse);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.EnsoCoeurDeLaTempete);
          player.LearnClassSkill(CustomSkill.EnsoGuideTempete);

          EnsoUtils.LearnSorcerySpell(player, (int)Spell.Balagarnsironhorn);
          EnsoUtils.LearnSorcerySpell(player, (int)Spell.GustOfWind);
          EnsoUtils.LearnSorcerySpell(player, (int)Spell.CallLightning);
          EnsoUtils.LearnSorcerySpell(player, CustomSpell.TempeteDeNeige);
          EnsoUtils.LearnSorcerySpell(player, CustomSpell.CreationOuDestructionDeau);

          break;

        case 11: player.LearnClassSkill(CustomSkill.EnsoFureurTempete); break;
        case 18: player.LearnClassSkill(CustomSkill.EnsoAmeDesVents); break;
      }
    }
  }
}
