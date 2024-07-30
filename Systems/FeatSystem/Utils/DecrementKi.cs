using Anvil.API;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static void DecrementKi(NwCreature creature, byte nbCharge = 1)
    {
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPatience, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDelugeDeCoups, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkStunStrike, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDesertion, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkExplosionKi, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPaumeVibratoire, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkDarkVision, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkTenebres, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkPassageSansTrace, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkSilence, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkFrappeDombre, nbCharge);
      creature.DecrementRemainingFeatUses((Feat)CustomSkill.MonkCrochetsDuSerpentDeFeu, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDeLair, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDagueDeGivre, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrissonDeLaMontagne, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDesQuatreTonnerres, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkRueeDesEspritsDuVent, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSphereDequilibreElementaire, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDesCendres, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDeLaTempete, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFouetDeLonde, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFaconnageDeLaRiviere, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkEtreinteDeLenfer, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkGongDuSommet, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFlammesDuPhenix, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPostureBrumeuse, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPorteParLeVent, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDefenseDeLaMontagne, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTorrentDeFlammes, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkVagueDeTerre, nbCharge);
      creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSouffleDeLhiver, nbCharge);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkDesertion) < 4)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, 0);
        //creature.SetFeatRemainingUses((Feat)CustomSkill.ImpositionDesMainsGuerison, 0);
      }

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkStunStrike) < 3)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDombre, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPaumeVibratoire, 0);
      }

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkDefenseDeLaMontagne) < 5)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDefenseDeLaMontagne, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkTorrentDeFlammes) < 5)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTorrentDeFlammes, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkSouffleDeLhiver) < 6)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSouffleDeLhiver, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkVagueDeTerre) < 6)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkVagueDeTerre, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPorteParLeVent) < 4)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPorteParLeVent, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkFlammesDuPhenix) < 4)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFlammesDuPhenix, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPostureBrumeuse) < 4)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPostureBrumeuse, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkGongDuSommet) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkGongDuSommet, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoigneDuVentDuNord, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPoingDeLair) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDeLair, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDesCendres) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDesCendres, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDeLaTempete) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFrappeDeLaTempete, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkFouetDeLonde) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkFouetDeLonde, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkSphereDequilibreElementaire) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSphereDequilibreElementaire, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPoingDesQuatreTonnerres) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPoingDesQuatreTonnerres, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkRueeDesEspritsDuVent) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkRueeDesEspritsDuVent, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkDagueDeGivre) < 2)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDagueDeGivre, 0);

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkTenebres) < 2)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkTenebres, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkSilence, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkPassageSansTrace, 0);
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDarkVision, 0);
      }
    }
  }
}
