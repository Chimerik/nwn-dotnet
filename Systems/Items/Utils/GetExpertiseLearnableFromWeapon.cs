using Anvil.API;

namespace NWN.Systems
{
  public static partial class ItemUtils
  {
    public static int GetExpertiseLearnableFromWeapon(BaseItemType itemType)
    {
      return itemType switch
      {
        BaseItemType.ShortSpear => CustomSkill.ExpertiseLance,
        BaseItemType.Longsword => CustomSkill.ExpertiseEpeeLongue,
        BaseItemType.Shortsword => CustomSkill.ExpertiseEpeeCourte,
        BaseItemType.Longbow => CustomSkill.ExpertiseArcLong,
        BaseItemType.Shortbow => CustomSkill.ExpertiseArcCourt,
        BaseItemType.Rapier => CustomSkill.ExpertiseRapiere,
        BaseItemType.LightHammer => CustomSkill.ExpertiseMarteauLeger,
        BaseItemType.Warhammer => CustomSkill.ExpertiseMarteauDeGuerre,
        BaseItemType.Handaxe => CustomSkill.ExpertiseHachette,
        BaseItemType.DwarvenWaraxe => CustomSkill.ExpertiseHacheNaine,
        BaseItemType.Shuriken => CustomSkill.ExpertiseShuriken,
        BaseItemType.TwoBladedSword => CustomSkill.ExpertiseLameDouble,
        BaseItemType.Club => CustomSkill.ExpertiseGourdin,
        BaseItemType.Dagger => CustomSkill.ExpertiseDague,
        BaseItemType.Doubleaxe => CustomSkill.ExpertiseHacheDouble,
        BaseItemType.Quarterstaff or BaseItemType.MagicStaff => CustomSkill.ExpertiseBaton,
        BaseItemType.LightMace => CustomSkill.ExpertiseMasseLegere,
        BaseItemType.Sickle => CustomSkill.ExpertiseSerpe,
        BaseItemType.LightCrossbow => CustomSkill.ExpertiseArbaleteLegere,
        BaseItemType.Dart => CustomSkill.ExpertiseDard,
        BaseItemType.LightFlail => CustomSkill.ExpertiseFleauLeger,
        BaseItemType.Morningstar => CustomSkill.ExpertiseMorgenstern,
        BaseItemType.Sling => CustomSkill.ExpertiseFronde,
        BaseItemType.Battleaxe => CustomSkill.ExpertiseHacheDeGuerre,
        BaseItemType.Greataxe => CustomSkill.ExpertiseHacheDarmes,
        BaseItemType.Greatsword => CustomSkill.ExpertiseEspadon,
        BaseItemType.Scimitar => CustomSkill.ExpertiseCimeterre,
        BaseItemType.Halberd => CustomSkill.ExpertiseHallebarde,
        BaseItemType.HeavyFlail => CustomSkill.ExpertiseFleauLourd,
        BaseItemType.ThrowingAxe => CustomSkill.ExpertiseHacheDeLancer,
        BaseItemType.Whip => CustomSkill.ExpertiseFouet,
        BaseItemType.HeavyCrossbow => CustomSkill.ExpertiseArbaleteLourde,
        BaseItemType.Bastardsword => CustomSkill.ExpertiseEpeeBatarde,
        BaseItemType.Scythe => CustomSkill.ExpertiseFaux,
        BaseItemType.DireMace => CustomSkill.ExpertiseMasseDouble,
        BaseItemType.Kama => CustomSkill.ExpertiseKama,
        BaseItemType.Katana => CustomSkill.ExpertiseKatana,
        BaseItemType.Kukri => CustomSkill.ExpertiseKukri,

        _ => 0,
      };
    }
    public static BaseItemType GeBaseWeaponFromLearnable(int learnable)
    {
      return learnable switch
      {
        CustomSkill.ExpertiseLance => BaseItemType.ShortSpear,
        CustomSkill.ExpertiseEpeeLongue => BaseItemType.Longsword,
        CustomSkill.ExpertiseEpeeCourte => BaseItemType.Shortsword,
        CustomSkill.ExpertiseArcLong => BaseItemType.Longbow,
        CustomSkill.ExpertiseArcCourt => BaseItemType.Shortbow,
        CustomSkill.ExpertiseRapiere => BaseItemType.Rapier,
        CustomSkill.ExpertiseMarteauLeger => BaseItemType.LightHammer,
        CustomSkill.ExpertiseMarteauDeGuerre => BaseItemType.Warhammer,
        CustomSkill.ExpertiseHachette => BaseItemType.Handaxe,
        CustomSkill.ExpertiseHacheNaine => BaseItemType.DwarvenWaraxe,
        CustomSkill.ExpertiseShuriken => BaseItemType.Shuriken,
        CustomSkill.ExpertiseLameDouble => BaseItemType.TwoBladedSword,
        CustomSkill.ExpertiseGourdin => BaseItemType.Club,
        CustomSkill.ExpertiseDague => BaseItemType.Dagger,
        CustomSkill.ExpertiseHacheDouble => BaseItemType.Doubleaxe,
        CustomSkill.ExpertiseBaton => BaseItemType.Quarterstaff,
        CustomSkill.ExpertiseMasseLegere => BaseItemType.LightMace,
        CustomSkill.ExpertiseSerpe => BaseItemType.Sickle,
        CustomSkill.ExpertiseArbaleteLegere => BaseItemType.LightCrossbow,
        CustomSkill.ExpertiseDard => BaseItemType.Dart,
        CustomSkill.ExpertiseFleauLeger => BaseItemType.LightFlail,
        CustomSkill.ExpertiseMorgenstern => BaseItemType.Morningstar,
        CustomSkill.ExpertiseFronde => BaseItemType.Sling,
        CustomSkill.ExpertiseHacheDeGuerre => BaseItemType.Battleaxe,
        CustomSkill.ExpertiseHacheDarmes => BaseItemType.Greataxe,
        CustomSkill.ExpertiseEspadon => BaseItemType.Greatsword,
        CustomSkill.ExpertiseCimeterre => BaseItemType.Scimitar,
        CustomSkill.ExpertiseHallebarde => BaseItemType.Halberd,
        CustomSkill.ExpertiseFleauLourd => BaseItemType.HeavyFlail,
        CustomSkill.ExpertiseHacheDeLancer => BaseItemType.ThrowingAxe,
        CustomSkill.ExpertiseFouet => BaseItemType.Whip,
        CustomSkill.ExpertiseArbaleteLourde => BaseItemType.HeavyCrossbow,
        CustomSkill.ExpertiseEpeeBatarde => BaseItemType.Bastardsword,
        CustomSkill.ExpertiseFaux => BaseItemType.Scythe,
        CustomSkill.ExpertiseMasseDouble => BaseItemType.DireMace,
        CustomSkill.ExpertiseKama => BaseItemType.Kama,
        CustomSkill.ExpertiseKatana => BaseItemType.Katana,
        CustomSkill.ExpertiseKukri => BaseItemType.Kukri,

        _ => BaseItemType.Invalid,
      };
    }
  }
}
