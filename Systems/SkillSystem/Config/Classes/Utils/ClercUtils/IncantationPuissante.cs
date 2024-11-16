using Anvil.API;

namespace NWN.Systems
{
  public partial class ClercUtils
  {
    public static void HandleIncantationPuissante(NwGameObject oCaster, NwGameObject oTarget, int damage, NwSpell spell)
    {
      if (damage < 1 || oTarget is not NwCreature target || oCaster is not NwCreature caster || !caster.KnowsFeat((Feat)CustomSkill.ClercIncantationPuissante)
        || spell.GetSpellLevelForClass(ClassType.Cleric) != 0 || caster.GetClassInfo(ClassType.Cleric)?.Level < 14)
        return;

      if (caster.GetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante) < 1)
        caster.SetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante, 1);
    }
  }
}
