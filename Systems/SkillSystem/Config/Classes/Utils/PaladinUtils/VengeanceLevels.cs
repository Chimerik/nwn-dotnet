using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

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

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.MarqueDuChasseur, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Bane, CustomClass.Paladin);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FouleeBrumeuse, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldPerson, CustomClass.Paladin);

          break;


        case 7:

          player.LearnClassSkill(CustomSkill.PaladinVengeurImplacable);

          player.oid.LoginCreature.OnCreatureAttack -= PaladinUtils.OnAttackVengeurImplacable;
          player.oid.LoginCreature.OnCreatureAttack += PaladinUtils.OnAttackVengeurImplacable;

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Haste, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ProtectionFromElements, CustomClass.Paladin);

          break;

        case 13:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Bannissement, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.PorteDimensionnelle, CustomClass.Paladin);

          break;

        case 17:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldMonster, CustomClass.Paladin);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Scrutation, CustomClass.Paladin);

          break;

        case 20:

          player.LearnClassSkill(CustomSkill.AngeDeLaVengeance);

          break;
      }
    }
  }
}
