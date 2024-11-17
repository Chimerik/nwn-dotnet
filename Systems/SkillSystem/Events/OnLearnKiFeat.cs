using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnKiFeat(PlayerSystem.Player player, int customSkillId)
    {
      NwCreature creature = player.oid.LoginCreature;

      if (!creature.KnowsFeat((Feat)customSkillId))
        creature.AddFeat((Feat)customSkillId);

      if(customSkillId == CustomSkill.MonkPatience)
        creature.SetFeatRemainingUses((Feat)customSkillId, 2);
      else
        creature.SetFeatRemainingUses((Feat)customSkillId, creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPatience));

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkDesertion) < 4)
      {
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkDesertion, 0);
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

      if (creature.GetFeatRemainingUses((Feat)CustomSkill.MonkEtreinteDeLenfer) < 3)
        creature.SetFeatRemainingUses((Feat)CustomSkill.MonkEtreinteDeLenfer, 0);

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

      return true;
    }
  }
}
