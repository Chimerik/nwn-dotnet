using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Recuperation(NwCreature caster, NwGameObject target)
    {
      if (target is not NwCreature creature)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.WildMagicMagieGalvanisanteBienfait);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.WildMagicMagieGalvanisanteRecuperation);

      byte spellLevel = (byte)Utils.Roll(3);
      StringUtils.DisplayStringToAllPlayersNearTarget(creature, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Magie Galvanisante - Récupération sort niveau {spellLevel} sur {creature.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

      foreach (var classInfo in caster.Classes)
      {
        if (classInfo.Class.IsSpellCaster)
        {
          var remainingSlots = classInfo.GetRemainingSpellSlots(spellLevel);
          if (remainingSlots < classInfo.Class.SpellGainTable[spellLevel - 1].Count)
            classInfo.SetRemainingSpellSlots(spellLevel, (byte)(remainingSlots + 1));
        }
      }
    }
  }
}
