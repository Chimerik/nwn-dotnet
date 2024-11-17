using Anvil.API;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static async void RestoreKi(NwCreature creature)
    {
      byte? level = creature.GetClassInfo((ClassType)CustomClass.Monk)?.Level;

      if (!level.HasValue)
        return;

      byte featUse = level.Value < 20 ? level.Value : (byte)20;

      await NwTask.NextFrame();

      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPatience, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDelugeDeCoups, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkExplosionKi, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSilence, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkCrochetsDuSerpentDeFeu, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrissonDeLaMontagne, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDagueDeGivre, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDeLair, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDesQuatreTonnerres, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkRueeDesEspritsDuVent, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSphereDequilibreElementaire, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDesCendres, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDeLaTempete, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFouetDeLonde, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFaconnageDeLaRiviere, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkEtreinteDeLenfer, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkGongDuSommet, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFlammesDuPhenix, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPostureBrumeuse, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPorteParLeVent, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDefenseDeLaMontagne, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTorrentDeFlammes, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkVagueDeTerre, featUse);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSouffleDeLhiver, featUse);
    }
  }
}
