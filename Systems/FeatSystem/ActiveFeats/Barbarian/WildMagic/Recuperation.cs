using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Recuperation(NwCreature caster, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      byte spellLevel = (byte)Utils.Roll(3);
      
      foreach (var classInfo in target.Classes)
      {
        if (classInfo.Class.IsSpellCaster)
        {
          for (byte i = spellLevel; i > 0; i--)
          {
            var remainingSlots = classInfo.GetRemainingSpellSlots(i);

            if (remainingSlots < classInfo.Class.SpellGainTable[classInfo.Level - 1][i])
            {
              classInfo.SetRemainingSpellSlots(i, (byte)(remainingSlots + 1));

              caster.DecrementRemainingFeatUses((Feat)CustomSkill.WildMagicMagieGalvanisanteBienfait);
              caster.DecrementRemainingFeatUses((Feat)CustomSkill.WildMagicMagieGalvanisanteRecuperation);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Magie Galvanisante - Récupération sort niveau {i} sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);

              return;
            }
          }
        }
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Magie Galvanisante - Récupération sur {target.Name.ColorString(ColorConstants.Cyan)} - Pas de sort à récupérer", StringUtils.gold, true, true);
    }
  }
}
