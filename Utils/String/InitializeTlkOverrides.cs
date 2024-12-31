using System.Security.Cryptography;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class StringUtils
  {
    public static void InitializeTlkOverrides()
    {
      OverrideTlkEntry(190050, "Mélange");
      OverrideTlkEntry(190051, "Tranche-artère");
      OverrideTlkEntry(190052, "Adrénaline : 4\nAttaque à l'épée.\nInflige 5...21...25 secondes de saignement.");
      OverrideTlkEntry(190053, "Saignement");
      OverrideTlkEntry(190054, "Brûlure");
      OverrideTlkEntry(190055, "Infirmité");
      OverrideTlkEntry(190056, "Blessure profonde");
      OverrideTlkEntry(190057, "Maladie");
      OverrideTlkEntry(190058, "Empoisonnement");
      OverrideTlkEntry(190059, "Faiblesse");
      OverrideTlkEntry(190060, "Armure brisée");
      OverrideTlkEntry(190061, "Aveuglement");
      OverrideTlkEntry(190062, "Etourdissement");
      OverrideTlkEntry(190067, "Menacé");
      OverrideTlkEntry(190072, "Haut-Elfe");
      OverrideTlkEntry(190073, "Elfe des bois");
      OverrideTlkEntry(190074, "Drow");
      OverrideTlkEntry(190075, "Demi Haut-Elfe");
      OverrideTlkEntry(190076, "Demi Elfe des Bois");
      OverrideTlkEntry(190077, "Demi Drow");
      OverrideTlkEntry(190078, "Hyper-sensibilité lumineuse");
      OverrideTlkEntry(190079, "Demi Orc : Endurance Implacable");
      //OverrideTlkEntry(190080, "Nain d'or");
      OverrideTlkEntry(190081, "Aasimar");
      OverrideTlkEntry(190082, "Duergar");
      OverrideTlkEntry(190083, "Halfelin Pied-Léger");
      OverrideTlkEntry(190084, "Halfelin Coeur-Vaillant");
      OverrideTlkEntry(190085, "Gnome des profondeurs");
      OverrideTlkEntry(190086, "Gnome des forêts");
      OverrideTlkEntry(190087, "Gnome des roches");
      OverrideTlkEntry(190088, "Tieffelin - Héritage Infernal");
      OverrideTlkEntry(190089, "Tieffelin - Héritage Abyssal");
      OverrideTlkEntry(190090, "Tieffelin - Héritage Chtonique");
      OverrideTlkEntryAsync(190091, "1sUqHix9bOocebunN6481Ea9Qyka8cGrm9_jDneflSyo");
      OverrideTlkEntryAsync(190092, "1Eiq8eipvoCHp3gtynjwfzZhUUkQaaaJEE8bmmgBwuSs");
      OverrideTlkEntryAsync(190093, "1OPhIPceDp4BxYhHT9ewznlcWuN2svohIt_bB6RLMXQI");
      OverrideTlkEntryAsync(190094, "1Uegi0SIF00tLWQfB5kNvaNTBfoIq1Eo0vFktiAatiL0");
      OverrideTlkEntryAsync(190095, "1Zty6S_4FW7gprPWM9zyijAHD3xIeGKsUiRu9qkYSmFI");
      OverrideTlkEntryAsync(190096, "1yYb8KC3izRmxApxQj6BIQD8diUpunnzbKnpBNAitk-U");
      OverrideTlkEntryAsync(190097, "1h56nnvKQ7nZW240V-MEPkHkSd8hZ3amw7hnkKR_OHpk");
      OverrideTlkEntryAsync(190098, "12WVf_85P08bLsNlSyxSrFW24xrxjVaEm5_WRZ0w6o8I");
      OverrideTlkEntryAsync(190099, "1KylDTmjQm3I9DPHJQlN56NHunA3FyCj_JqvAvUdC2eM");
      OverrideTlkEntryAsync(190100, "19jne6SUXvG-iU20seboUSIQD8p-RFm2ZX56gQEqbt-s");
      OverrideTlkEntryAsync(190101, "1rC8V0TIIITtONIU7v3oibrCI0-sTXiMT21xBcyF21Ww");
      OverrideTlkEntryAsync(190102, "174R6FNW5dO_GxVQyqvndCm0vTnyZ6sTqlDtKVcFSklA");
      OverrideTlkEntryAsync(190103, "1m5JzqgayChh6D64b6L53AqqoJvyhjt0gLKdHhUIRh94");
      OverrideTlkEntryAsync(190104, "1Gtoy2ELVBlY1l8YaSqiFp4GUKgPbDqRyNZLFxD-7lYU");
      OverrideTlkEntryAsync(190105, "1HTCpVKh_FQqvdLfVWLy8P2--tucvL6k1ipUYngL1uto");
      OverrideTlkEntryAsync(190106, "17lww5fKedFXoigqIlyTBdyM_-Id9MRKp4qs-HkmYX-0");
      OverrideTlkEntryAsync(190107, "1lELBdXP7nR8oa_EWDh0DlMk9rtZUxtZ1vt-7Yuk8xFg");
      OverrideTlkEntryAsync(190108, "1ReBm1WQocMnbz0yKQV_cRtd73N2YwXQXvl0Iv44LAhg");
      OverrideTlkEntryAsync(190109, "1ll6A-ekXUXKj0crQrAYLRtn6HowvnXCo8JprxzKLpu8");
      OverrideTlkEntry(190110, "Protection contre les lames");
      OverrideTlkEntry(190116, "Concentration");
      OverrideTlkEntry(190119, "Nécrotique");
      OverrideTlkEntry(190120, "<CUSTOM0> Nécrotique");
      OverrideTlkEntry(190121, "Poison");
      OverrideTlkEntry(190122, "<CUSTOM0> Poison");
      OverrideTlkEntry(190123, "Psychique");
      OverrideTlkEntry(190124, "<CUSTOM0> Psychique");
      OverrideTlkEntry(190125, "Force");
      OverrideTlkEntry(190126, "<CUSTOM0> Force");
      OverrideTlkEntry(190127, "Radiant");
      OverrideTlkEntry(190128, "<CUSTOM0> Radiant");
      OverrideTlkEntry(190129, "Tonnerre");
      OverrideTlkEntry(190130, "<CUSTOM0> Tonnerre");
      OverrideTlkEntry(190153, "LISEZ-MOI");
      OverrideTlkEntry(190154, "Les choix de races, de classe et de caractéristiques de cette partie de l'interface sont inutiles.\n\nEn jeu, un miroir vous permettra d'accéder à des options de personnalisation approfondies.\n\nIl vous faudra alors valider toutes les étapes avant que le capitaine ne vous autorise à poursuivre votre voyage.");
      OverrideTlkEntry(190155, "Les Larmes des Erylies - Editeur de personnage");
      OverrideTlkEntry(190619, "Immunité aux dégâts (poison)");
      OverrideTlkEntry(190620, "Immunité aux dégâts (nécrotique)");
      OverrideTlkEntry(190621, "Immunité aux dégâts (psychique)");
      OverrideTlkEntry(190807, "Terrain Difficile");
      OverrideTlkEntry(191025, "Glacé");
      OverrideTlkEntry(191107, "Immunité aux dégâts (tranchant)");
      OverrideTlkEntry(191108, "Immunité aux dégâts (contondant)");
      OverrideTlkEntry(191109, "Immunité aux dégâts (perçant)");
      OverrideTlkEntry(191110, "Vulnérabilité aux dégâts (tranchant)");
      OverrideTlkEntry(191111, "Vulnérabilité aux dégâts (contondant)");
      OverrideTlkEntry(191112, "Vulnérabilité aux dégâts (perçant)");
      OverrideTlkEntry(191113, "Vulnérabilité aux dégâts (poison)");
      OverrideTlkEntry(191114, "Vulnérabilité aux dégâts (nécrotique)");
      OverrideTlkEntry(191115, "Vulnérabilité aux dégâts (psychique)");
      OverrideTlkEntry(191116, "Résistance aux dégâts (force)");
      OverrideTlkEntry(191117, "Résistance aux dégâts (acide)");
      OverrideTlkEntry(191118, "Résistance aux dégâts (froid)");
      OverrideTlkEntry(191119, "Résistance aux dégâts (radiant)");
      OverrideTlkEntry(191120, "Résistance aux dégâts (électricité)");
      OverrideTlkEntry(191121, "Résistance aux dégâts (feu)");
      OverrideTlkEntry(191122, "Résistance aux dégâts (tonnerre)");
      OverrideTlkEntry(191123, "Résistance aux dégâts (tranchant)");
      OverrideTlkEntry(191124, "Résistance aux dégâts (contondant)");
      OverrideTlkEntry(191125, "Résistance aux dégâts (perçant)");
      OverrideTlkEntry(191126, "Résistance aux dégâts (poison)");
      OverrideTlkEntry(191127, "Résistance aux dégâts (nécrotique)");
      OverrideTlkEntry(191128, "Résistance aux dégâts (psychique)");
      OverrideTlkEntry(191253, "Déstabilisé");
      OverrideTlkEntry(191259, "Arcane Mystique");

      foreach (var entry in Feats2da.featTable)
      {
        if ((entry.spellId < 0) && SkillSystem.learnableDictionary.TryGetValue(entry.RowIndex, out var learnable) && learnable is LearnableSkill)
        {
          StrRef name = entry.nameTlkEntry;
          name.Override = learnable.name;
          StrRef description = entry.descriptionTlkEntry;
          description.Override = learnable.description;
        }
      }

      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkEtreinteDeLenfer).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkEtreinteDeLenfer].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkFrissonDeLaMontagne).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkFrissonDeLaMontagne].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkPoingDesQuatreTonnerres).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkPoingDesQuatreTonnerres].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkRueeDesEspritsDuVent).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkRueeDesEspritsDuVent].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkSphereDequilibreElementaire).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkSphereDequilibreElementaire].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkFrappeDesCendres).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkFrappeDesCendres].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkFrappeDeLaTempete).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkFrappeDeLaTempete].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkPoigneDuVentDuNord).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkPoigneDuVentDuNord].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkGongDuSommet).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkGongDuSommet].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkFlammesDuPhenix).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkFlammesDuPhenix].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkPostureBrumeuse).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkPostureBrumeuse].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkDefenseDeLaMontagne).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkDefenseDeLaMontagne].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkTorrentDeFlammes).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkTorrentDeFlammes].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkVagueDeTerre).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkVagueDeTerre].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkSouffleDeLhiver).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkSouffleDeLhiver].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ThiefDiscretionSupreme).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.ThiefDiscretionSupreme].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ThiefDiscretionSupreme).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.ThiefDiscretionSupreme].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkLinceulDombre).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkLinceulDombre].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MonkLinceulDombre).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.MonkLinceulDombre].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ClercLinceulDombre).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.ClercLinceulDombre].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ClercLinceulDombre).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.ClercLinceulDombre].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.PacteDeLaChaine).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.PacteDeLaChaine].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.PacteDeLaChaine).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.PacteDeLaChaine].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeEnjoleuse).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeEnjoleuse].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeEnjoleuse).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeEnjoleuse].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeEvanescente).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeEvanescente].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeEvanescente).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeEvanescente].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeProvocatrice).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeProvocatrice].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeProvocatrice).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeProvocatrice].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeRafraichissante).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeRafraichissante].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeRafraichissante).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeRafraichissante].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeRedoutable).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeRedoutable].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.FouleeRedoutable).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.FouleeRedoutable].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ArmureDesOmbres).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.ArmureDesOmbres].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.ArmureDesOmbres).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.ArmureDesOmbres].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.VisionsDesRoyaumesLointains).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.VisionsDesRoyaumesLointains].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.VisionsDesRoyaumesLointains).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.VisionsDesRoyaumesLointains].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.VigueurDemoniaque).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.VigueurDemoniaque].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.VigueurDemoniaque).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.VigueurDemoniaque].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.PasAerien).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.PasAerien].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.PasAerien).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.PasAerien].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.DonPelagique).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.DonPelagique].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.DonPelagique).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.DonPelagique].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.SautDoutremonde).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.SautDoutremonde].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.SautDoutremonde).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.SautDoutremonde].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.UnParmiLesOmbres).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.UnParmiLesOmbres].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.UnParmiLesOmbres).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.UnParmiLesOmbres].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MasqueDesMilleVisages).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MasqueDesMilleVisages].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MasqueDesMilleVisages).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.MasqueDesMilleVisages].description);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MaitreDesFormes).Name.CustomId, SkillSystem.learnableDictionary[CustomSkill.MaitreDesFormes].name);
      OverrideTlkEntry(NwFeat.FromFeatId(CustomSkill.MaitreDesFormes).Description.CustomId, SkillSystem.learnableDictionary[CustomSkill.MaitreDesFormes].description);
    }

    private static void OverrideTlkEntry(uint entry, string text)
    {
      StrRef.FromCustomTlk(entry).Override = text;
    }
    private static async void OverrideTlkEntryAsync(uint entry, string text)
    {
      StrRef.FromCustomTlk(entry).Override = await DownloadGoogleDoc(text);
    }
  }
}
