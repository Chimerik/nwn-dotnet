using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static bool IsCreatureWeaponExpert(NwCreature creature, NwItem item)
    {
      if (item is null)
        return false;

      return item.BaseItem.ItemType switch
      {
        BaseItemType.Shortsword => creature.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeCourte),
        BaseItemType.Sling => creature.KnowsFeat((Feat)CustomSkill.ExpertiseFronde),
        BaseItemType.ThrowingAxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDeLancer),
        BaseItemType.Longbow => creature.KnowsFeat((Feat)CustomSkill.ExpertiseArcLong),
        BaseItemType.Shortbow => creature.KnowsFeat((Feat)CustomSkill.ExpertiseArcCourt),
        BaseItemType.LightCrossbow => creature.KnowsFeat((Feat)CustomSkill.ExpertiseArbaleteLegere),
        BaseItemType.HeavyCrossbow => creature.KnowsFeat((Feat)CustomSkill.ExpertiseArbaleteLourde),
        BaseItemType.Dart => creature.KnowsFeat((Feat)CustomSkill.ExpertiseDard),
        BaseItemType.Scythe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseFaux),
        BaseItemType.Whip => creature.KnowsFeat((Feat)CustomSkill.ExpertiseFouet),
        BaseItemType.Sickle => creature.KnowsFeat((Feat)CustomSkill.ExpertiseSerpe),
        BaseItemType.Kukri => creature.KnowsFeat((Feat)CustomSkill.ExpertiseKukri),
        BaseItemType.Kama => creature.KnowsFeat((Feat)CustomSkill.ExpertiseKama),
        BaseItemType.Dagger => creature.KnowsFeat((Feat)CustomSkill.ExpertiseDague),
        BaseItemType.Rapier => creature.KnowsFeat((Feat)CustomSkill.ExpertiseRapiere),
        BaseItemType.LightHammer => creature.KnowsFeat((Feat)CustomSkill.ExpertiseMarteauLeger),
        BaseItemType.Warhammer => creature.KnowsFeat((Feat)CustomSkill.ExpertiseMarteauDeGuerre),
        BaseItemType.Morningstar => creature.KnowsFeat((Feat)CustomSkill.ExpertiseMorgenstern),
        BaseItemType.LightFlail => creature.KnowsFeat((Feat)CustomSkill.ExpertiseFleauLeger),
        BaseItemType.HeavyFlail => creature.KnowsFeat((Feat)CustomSkill.ExpertiseFleauLourd),
        BaseItemType.LightMace => creature.KnowsFeat((Feat)CustomSkill.ExpertiseMasseLegere),
        BaseItemType.DireMace => creature.KnowsFeat((Feat)CustomSkill.ExpertiseMasseDouble),
        BaseItemType.Club => creature.KnowsFeat((Feat)CustomSkill.ExpertiseGourdin),
        BaseItemType.Quarterstaff or BaseItemType.MagicStaff => creature.KnowsFeat((Feat)CustomSkill.ExpertiseBaton),
        BaseItemType.ShortSpear => creature.KnowsFeat((Feat)CustomSkill.ExpertiseLance),
        BaseItemType.Halberd => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHallebarde),
        BaseItemType.Greataxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDarmes),
        BaseItemType.Handaxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHachette),
        BaseItemType.Battleaxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDeGuerre),
        BaseItemType.DwarvenWaraxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHacheNaine),
        BaseItemType.Doubleaxe => creature.KnowsFeat((Feat)CustomSkill.ExpertiseHacheDouble),
        BaseItemType.Scimitar => creature.KnowsFeat((Feat)CustomSkill.ExpertiseCimeterre),
        BaseItemType.Longsword => creature.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeLongue),
        BaseItemType.Greatsword => creature.KnowsFeat((Feat)CustomSkill.ExpertiseEspadon),
        BaseItemType.Bastardsword => creature.KnowsFeat((Feat)CustomSkill.ExpertiseEpeeBatarde),
        BaseItemType.TwoBladedSword => creature.KnowsFeat((Feat)CustomSkill.ExpertiseLameDouble),
        _ => false,
      };
    }

    public static bool IsCreatureWeaponExpert(Native.API.CNWSCreature creature, NwBaseItem item)
    {
      if (item is null)
        return false;

      return item.ItemType switch
      {
        BaseItemType.Shortsword => creature.m_pStats.HasFeat(CustomSkill.ExpertiseEpeeCourte).ToBool(),
        BaseItemType.Sling => creature.m_pStats.HasFeat(CustomSkill.ExpertiseFronde).ToBool(),
        BaseItemType.ThrowingAxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHacheDeLancer).ToBool(),
        BaseItemType.Longbow => creature.m_pStats.HasFeat(CustomSkill.ExpertiseArcLong).ToBool(),
        BaseItemType.Shortbow => creature.m_pStats.HasFeat(CustomSkill.ExpertiseArcCourt).ToBool(),
        BaseItemType.LightCrossbow => creature.m_pStats.HasFeat(CustomSkill.ExpertiseArbaleteLegere).ToBool(),
        BaseItemType.HeavyCrossbow => creature.m_pStats.HasFeat(CustomSkill.ExpertiseArbaleteLourde).ToBool(),
        BaseItemType.Dart => creature.m_pStats.HasFeat(CustomSkill.ExpertiseDard).ToBool(),
        BaseItemType.Scythe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseFaux).ToBool(),
        BaseItemType.Whip => creature.m_pStats.HasFeat(CustomSkill.ExpertiseFouet).ToBool(),
        BaseItemType.Sickle => creature.m_pStats.HasFeat(CustomSkill.ExpertiseSerpe).ToBool(),
        BaseItemType.Kukri => creature.m_pStats.HasFeat(CustomSkill.ExpertiseKukri).ToBool(),
        BaseItemType.Kama => creature.m_pStats.HasFeat(CustomSkill.ExpertiseKama).ToBool(),
        BaseItemType.Dagger => creature.m_pStats.HasFeat(CustomSkill.ExpertiseDague).ToBool(),
        BaseItemType.Rapier => creature.m_pStats.HasFeat(CustomSkill.ExpertiseRapiere).ToBool(),
        BaseItemType.LightHammer => creature.m_pStats.HasFeat(CustomSkill.ExpertiseMarteauLeger).ToBool(),
        BaseItemType.Warhammer => creature.m_pStats.HasFeat(CustomSkill.ExpertiseMarteauDeGuerre).ToBool(),
        BaseItemType.Morningstar => creature.m_pStats.HasFeat(CustomSkill.ExpertiseMorgenstern).ToBool(),
        BaseItemType.LightFlail => creature.m_pStats.HasFeat(CustomSkill.ExpertiseFleauLeger).ToBool(),
        BaseItemType.HeavyFlail => creature.m_pStats.HasFeat(CustomSkill.ExpertiseFleauLourd).ToBool(),
        BaseItemType.LightMace => creature.m_pStats.HasFeat(CustomSkill.ExpertiseMasseLegere).ToBool(),
        BaseItemType.DireMace => creature.m_pStats.HasFeat(CustomSkill.ExpertiseMasseDouble).ToBool(),
        BaseItemType.Club => creature.m_pStats.HasFeat(CustomSkill.ExpertiseGourdin).ToBool(),
        BaseItemType.Quarterstaff or BaseItemType.MagicStaff => creature.m_pStats.HasFeat(CustomSkill.ExpertiseBaton).ToBool(),
        BaseItemType.ShortSpear => creature.m_pStats.HasFeat(CustomSkill.ExpertiseLance).ToBool(),
        BaseItemType.Halberd => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHallebarde).ToBool(),
        BaseItemType.Greataxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHacheDarmes).ToBool(),
        BaseItemType.Handaxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHachette).ToBool(),
        BaseItemType.Battleaxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHacheDeGuerre).ToBool(),
        BaseItemType.DwarvenWaraxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHacheNaine).ToBool(),
        BaseItemType.Doubleaxe => creature.m_pStats.HasFeat(CustomSkill.ExpertiseHacheDouble).ToBool(),
        BaseItemType.Scimitar => creature.m_pStats.HasFeat(CustomSkill.ExpertiseCimeterre).ToBool(),
        BaseItemType.Longsword => creature.m_pStats.HasFeat(CustomSkill.ExpertiseEpeeLongue).ToBool(),
        BaseItemType.Greatsword => creature.m_pStats.HasFeat(CustomSkill.ExpertiseEspadon).ToBool(),
        BaseItemType.Bastardsword => creature.m_pStats.HasFeat(CustomSkill.ExpertiseEpeeBatarde).ToBool(),
        BaseItemType.TwoBladedSword => creature.m_pStats.HasFeat(CustomSkill.ExpertiseLameDouble).ToBool(),
        _ => false,
      };
    }
  }
}
