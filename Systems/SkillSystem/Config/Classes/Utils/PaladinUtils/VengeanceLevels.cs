using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Paladin
  {
    public static void HandleVengeanceLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Serment de Vengeance");
          player.oid.SetTextureOverride("paladin", "vengeance");

          player.LearnClassSkill(CustomSkill.PaladinVoeuHostile);
          player.LearnClassSkill(CustomSkill.PaladinPuissanceInquisitrice);

          player.LearnAlwaysPreparedSpell(CustomSpell.MarqueDuChasseur, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell((int)Spell.Bane, CustomClass.Paladin);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.FouleeBrumeuse, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell((int)Spell.HoldPerson, CustomClass.Paladin);

          break;


        case 7:

          player.LearnClassSkill(CustomSkill.PaladinVengeurImplacable);

          player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.Haste, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell((int)Spell.ProtectionFromElements, CustomClass.Paladin);

          break;

        case 13:

          player.LearnAlwaysPreparedSpell(CustomSpell.Bannissement, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.PorteDimensionnelle, CustomClass.Paladin);

          break;

        case 17:

          player.LearnAlwaysPreparedSpell((int)Spell.HoldMonster, CustomClass.Paladin);
          player.LearnAlwaysPreparedSpell(CustomSpell.Scrutation, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.AngeDeLaVengeance);

          break;
      }
    }
  }
}
