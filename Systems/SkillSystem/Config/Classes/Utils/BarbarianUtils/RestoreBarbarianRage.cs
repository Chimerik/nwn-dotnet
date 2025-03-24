using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static async void RestoreBarbarianRage(NwCreature creature, bool shortRest = false)
    {
      byte? level = creature.GetClassInfo(ClassType.Barbarian)?.Level;

      if (!level.HasValue)
        return;

      await NwTask.NextFrame();

      

      byte maxUse = (byte)(level > 16 ? 6 : level > 11 ? 5 : level > 5 ? 3 : 2);

      Feat rageToRestore = GetRageToRestore(creature);

      if (shortRest)
      {
        byte remainingUse = creature.GetFeatRemainingUses(rageToRestore);
        
        if(remainingUse < maxUse)
          creature.SetFeatRemainingUses(rageToRestore, (byte)(remainingUse + 1));
      }
      else
      {
        creature.SetFeatRemainingUses(rageToRestore, maxUse);
        creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
        creature.SetFeatRemainingUses((Feat)CustomSkill.BersekerPresenceIntimidante, 1);
        creature.SetFeatRemainingUses((Feat)CustomSkill.BersekerRestorePresenceIntimidante, 0);
      }



      if (creature.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        byte proficiencyBonus = (byte)NativeUtils.GetCreatureProficiencyBonus(creature);

        creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicSense, proficiencyBonus);
        creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicSense, proficiencyBonus);
        creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicTeleportation, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicMagieGalvanisanteBienfait, proficiencyBonus);
        creature.SetFeatRemainingUses((Feat)CustomSkill.WildMagicMagieGalvanisanteRecuperation, proficiencyBonus);
      }
    }
    private static Feat GetRageToRestore(NwCreature creature)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.TotemRage))
        return (Feat)CustomSkill.TotemRage;
      else
        return Feat.BarbarianRage;
    }
  }
}
