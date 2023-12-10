using System.Formats.Tar;
using Anvil.API;

namespace NWN
{
  public static partial class StringUtils
  {
    public static async void InitializeTlkOverrides()
    {
      OverrideTlkEntry(1382, "Souffle de guérison");
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
      OverrideTlkEntry(190063, "Sprint");
      OverrideTlkEntry(190064, "Action qui permet de doubler votre vitesse de déplacement pendant 1 round.");
      OverrideTlkEntry(190065, "Désengagement");
      OverrideTlkEntry(190066, "Se désengager permet de se déplacer sans subir d'attaque d'opportunité pendant 1 round.");
      OverrideTlkEntry(190067, "Menacé");
      OverrideTlkEntry(190068, "Second Souffle");
      OverrideTlkEntry(190069, "Action bonus\nVous soigne de 1d10 + votre niveau de guerrier\nRécupération : repos court");
      OverrideTlkEntry(190070, "Fougue");
      OverrideTlkEntry(190071, "Action gratuite\nVous bénéficiez d'une attaque supplémentaire\nDurée & Cooldown : 10 rounds\nRécupération : repos court");
      OverrideTlkEntry(190072, "Haut-Elfe");
      OverrideTlkEntry(190073, "Elfe des bois");
      OverrideTlkEntry(190074, "Drow");
      OverrideTlkEntry(190075, "Demi Haut-Elfe");
      OverrideTlkEntry(190076, "Demi Drow");
      OverrideTlkEntry(190077, "Hyper-sensibilité lumineuse");
      OverrideTlkEntry(190078, "Demi Orc : Endurance Implacable");
      OverrideTlkEntry(190079, "Nain d'or");
      OverrideTlkEntry(190080, "Nain d'écu");
      OverrideTlkEntry(190081, "Duergar");
      OverrideTlkEntry(190082, "Demi Elfe des bois");
      OverrideTlkEntry(190083, "Halfelin Pied-Léger");
      OverrideTlkEntry(190084, "Halfelin Coeur-Vaillant");
      OverrideTlkEntry(190085, "Gnome des profondeurs");
      OverrideTlkEntry(190086, "Gnome des forêts");
      OverrideTlkEntry(190088, "Tieffelin - Asmodeus");
      OverrideTlkEntry(190089, "Tieffelin - Méphisto");
      OverrideTlkEntry(190090, "Tieffelin - Zariel");
      OverrideTlkEntryAsync(190091, "1sUqHix9bOocebunN6481Ea9Qyka8cGrm9_jDneflSyo");
      OverrideTlkEntryAsync(190092, "1Eiq8eipvoCHp3gtynjwfzZhUUkQaaaJEE8bmmgBwuSs");
      OverrideTlkEntryAsync(190093, "1OPhIPceDp4BxYhHT9ewznlcWuN2svohIt_bB6RLMXQI");
      OverrideTlkEntryAsync(190094, "1Uegi0SIF00tLWQfB5kNvaNTBfoIq1Eo0vFktiAatiL0");
      OverrideTlkEntryAsync(190095, "1Zty6S_4FW7gprPWM9zyijAHD3xIeGKsUiRu9qkYSmFI");
      OverrideTlkEntryAsync(190096, "1yYb8KC3izRmxApxQj6BIQD8diUpunnzbKnpBNAitk-U");
      OverrideTlkEntryAsync(190097, "1h56nnvKQ7nZW240V-MEPkHkSd8hZ3amw7hnkKR_OHpk");
      OverrideTlkEntryAsync(190098, "12WVf_85P08bLsNlSyxSrFW24xrxjVaEm5_WRZ0w6o8I");
      OverrideTlkEntryAsync(190099, "1MQS77YVL1AFYDoeKZbTMax64AlKkt_ro5SpcFqLEDeM");
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
      OverrideTlkEntry(190129, "<Tonnerre");
      OverrideTlkEntry(190130, "<CUSTOM0> Tonnerre");
      OverrideTlkEntry(190131, "<CUSTOM0> Force");
      OverrideTlkEntry(190132, "<CUSTOM0> Force");
      OverrideTlkEntry(190133, "<CUSTOM0> Force");
      OverrideTlkEntry(190136, "Communication Animale");
      OverrideTlkEntry(190151, "Mode Esquive");
      OverrideTlkEntry(190152, "Vous vous concentrer uniquement sur l'esquive au détriment de toute attaque.\n\nLes jets d'attaque qui vous ciblent subissent un désavantage, vos jets de sauvegarde de dextérité ont un avantage.\n\nVous perdez cet effet si vous êtes incapable d'agir, que votre capacité de déplacement tombe à 0, que vous attaquez ou lancez un sort.");
      OverrideTlkEntry(190153, "LISEZ-MOI");
      OverrideTlkEntry(190154, "Les choix de races, de classe et de caractéristiques de cette partie de l'interface sont inutiles.\n\nEn jeu, un miroir vous permettra d'accéder à des options de personnalisation approfondies.\n\nIl vous faudra alors valider toutes les étapes avant que le capitaine ne vous autorise à poursuivre votre voyage.");
      OverrideTlkEntry(190155, "Les Larmes des Erylies - Editeur de personnage");
      OverrideTlkEntry(190156, "Style de combat - Protection");
      OverrideTlkEntry(190157, "Maître Ambidextre");
      OverrideTlkEntry(190158, "Cogneur Lourd");
      OverrideTlkEntry(190162, "Maître des boucliers");
      OverrideTlkEntry(190164, "Tueur de mage");
      OverrideTlkEntry(190165, "Mobile");
      OverrideTlkEntry(190167, "Agresseur Sauvage");
      OverrideTlkEntry(190169, "Maître d'Hast");
      OverrideTlkEntry(190175, "Tireur d'Elite");
      OverrideTlkEntry(190177, "Bagarreur de taverne");
      OverrideTlkEntry(190179, "Mage de guerre");
      OverrideTlkEntry(190187, "Pourfendeur");
      OverrideTlkEntry(190188, "Pourfendeur - Infirmité");
      OverrideTlkEntry(190189, "Pourfendeur - Désavantage");
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
