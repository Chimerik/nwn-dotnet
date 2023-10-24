using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class StringUtils
  {
    public static async void InitializeTlkOverrides()
    {
      StrRef tlkEntry = StrRef.FromCustomTlk(190050);
      tlkEntry.Override = "Mélange";

      tlkEntry = StrRef.FromCustomTlk(190051);
      tlkEntry.Override = "Tranche-artère";

      tlkEntry = StrRef.FromCustomTlk(190052);
      tlkEntry.Override = "Adrénaline : 4\nAttaque à l'épée.\nInflige 5...21...25 secondes de saignement.";

      tlkEntry = StrRef.FromCustomTlk(190053);
      tlkEntry.Override = "Saignement";

      tlkEntry = StrRef.FromCustomTlk(190054);
      tlkEntry.Override = "Brûlure";

      tlkEntry = StrRef.FromCustomTlk(190055);
      tlkEntry.Override = "Infirmité";

      tlkEntry = StrRef.FromCustomTlk(190056);
      tlkEntry.Override = "Blessure profonde";

      tlkEntry = StrRef.FromCustomTlk(190057);
      tlkEntry.Override = "Maladie";

      tlkEntry = StrRef.FromCustomTlk(190058);
      tlkEntry.Override = "Empoisonnement";

      tlkEntry = StrRef.FromCustomTlk(190059);
      tlkEntry.Override = "Faiblesse";

      tlkEntry = StrRef.FromCustomTlk(190060);
      tlkEntry.Override = "Armure brisée";

      tlkEntry = StrRef.FromCustomTlk(190061);
      tlkEntry.Override = "Aveuglement";

      tlkEntry = StrRef.FromCustomTlk(190062);
      tlkEntry.Override = "Etourdissement";

      tlkEntry = StrRef.FromCustomTlk(1382);
      tlkEntry.Override = "Souffle de guérison";

      tlkEntry = StrRef.FromCustomTlk(190063);
      tlkEntry.Override = "Sprint";

      tlkEntry = StrRef.FromCustomTlk(190064);
      tlkEntry.Override = "Action qui permet de doubler votre vitesse de déplacement pendant 1 round.";

      tlkEntry = StrRef.FromCustomTlk(190065);
      tlkEntry.Override = "Désengagement";

      tlkEntry = StrRef.FromCustomTlk(190066);
      tlkEntry.Override = "Se désengager permet de se déplacer sans subir d'attaque d'opportunité pendant 1 round.";

      tlkEntry = StrRef.FromCustomTlk(190067);
      tlkEntry.Override = "Menacé";

      tlkEntry = StrRef.FromCustomTlk(190068);
      tlkEntry.Override = "Second Souffle";

      tlkEntry = StrRef.FromCustomTlk(190069);
      tlkEntry.Override = "Action bonus\nVous soigne de 1d10 + votre niveau de guerrier\nRécupération : repos court";

      tlkEntry = StrRef.FromCustomTlk(190070);
      tlkEntry.Override = "Fougue";

      tlkEntry = StrRef.FromCustomTlk(190071);
      tlkEntry.Override = "Action gratuite\nVous bénéficiez d'une attaque supplémentaire\nDurée & Cooldown : 10 rounds\nRécupération : repos court";

      tlkEntry = StrRef.FromCustomTlk(190072);
      tlkEntry.Override = "Haut-Elfe";

      tlkEntry = StrRef.FromCustomTlk(190073);
      tlkEntry.Override = "Elfe des bois";

      tlkEntry = StrRef.FromCustomTlk(190074);
      tlkEntry.Override = "Drow";

      tlkEntry = StrRef.FromCustomTlk(190075);
      tlkEntry.Override = "Demi Haut-Elfe";

      tlkEntry = StrRef.FromCustomTlk(190076);
      tlkEntry.Override = "Demi Elfe des bois";

      tlkEntry = StrRef.FromCustomTlk(190077);
      tlkEntry.Override = "Demi Drow";

      tlkEntry = StrRef.FromCustomTlk(190078);
      tlkEntry.Override = "Hyper-sensibilité lumineuse";

      tlkEntry = StrRef.FromCustomTlk(190079);
      tlkEntry.Override = "Demi Orc : Endurance Implacable";

      tlkEntry = StrRef.FromCustomTlk(190080);
      tlkEntry.Override = "Nain d'or";

      tlkEntry = StrRef.FromCustomTlk(190081);
      tlkEntry.Override = "Nain d'écu";

      tlkEntry = StrRef.FromCustomTlk(190082);
      tlkEntry.Override = "Duergar";

      tlkEntry = StrRef.FromCustomTlk(190083);
      tlkEntry.Override = "Halfelin Pied-Léger";

      tlkEntry = StrRef.FromCustomTlk(190084);
      tlkEntry.Override = "Halfelin Coeur-Vaillant";

      tlkEntry = StrRef.FromCustomTlk(190085);
      tlkEntry.Override = "Gnome des profondeurs";

      tlkEntry = StrRef.FromCustomTlk(190086);
      tlkEntry.Override = "Gnome des forêts";

      tlkEntry = StrRef.FromCustomTlk(190087);
      tlkEntry.Override = "Gnome des roches";

      tlkEntry = StrRef.FromCustomTlk(190088);
      tlkEntry.Override = "Tieffelin - Asmodeus";

      tlkEntry = StrRef.FromCustomTlk(190089);
      tlkEntry.Override = "Tieffelin - Méphisto";

      tlkEntry = StrRef.FromCustomTlk(190090);
      tlkEntry.Override = "Tieffelin - Zariel";

      tlkEntry = StrRef.FromCustomTlk(190091);
      tlkEntry.Override = await DownloadGoogleDoc("1sUqHix9bOocebunN6481Ea9Qyka8cGrm9_jDneflSyo");

      tlkEntry = StrRef.FromCustomTlk(190092);
      tlkEntry.Override = await DownloadGoogleDoc("1Eiq8eipvoCHp3gtynjwfzZhUUkQaaaJEE8bmmgBwuSs");
      
      tlkEntry = StrRef.FromCustomTlk(190093);
      tlkEntry.Override = await DownloadGoogleDoc("1OPhIPceDp4BxYhHT9ewznlcWuN2svohIt_bB6RLMXQI");
      
      tlkEntry = StrRef.FromCustomTlk(190094);
      tlkEntry.Override = await DownloadGoogleDoc("1Uegi0SIF00tLWQfB5kNvaNTBfoIq1Eo0vFktiAatiL0");
      
      tlkEntry = StrRef.FromCustomTlk(190095);
      tlkEntry.Override = await DownloadGoogleDoc("1Zty6S_4FW7gprPWM9zyijAHD3xIeGKsUiRu9qkYSmFI");

      tlkEntry = StrRef.FromCustomTlk(190096);
      tlkEntry.Override = await DownloadGoogleDoc("1yYb8KC3izRmxApxQj6BIQD8diUpunnzbKnpBNAitk-U");

      tlkEntry = StrRef.FromCustomTlk(190097);
      tlkEntry.Override = await DownloadGoogleDoc("1h56nnvKQ7nZW240V-MEPkHkSd8hZ3amw7hnkKR_OHpk");

      tlkEntry = StrRef.FromCustomTlk(190098);
      tlkEntry.Override = await DownloadGoogleDoc("12WVf_85P08bLsNlSyxSrFW24xrxjVaEm5_WRZ0w6o8I");

      tlkEntry = StrRef.FromCustomTlk(190099);
      tlkEntry.Override = await DownloadGoogleDoc("1MQS77YVL1AFYDoeKZbTMax64AlKkt_ro5SpcFqLEDeM");

      tlkEntry = StrRef.FromCustomTlk(190100);
      tlkEntry.Override = await DownloadGoogleDoc("19jne6SUXvG-iU20seboUSIQD8p-RFm2ZX56gQEqbt-s");

      tlkEntry = StrRef.FromCustomTlk(190101);
      tlkEntry.Override = await DownloadGoogleDoc("1rC8V0TIIITtONIU7v3oibrCI0-sTXiMT21xBcyF21Ww");

      tlkEntry = StrRef.FromCustomTlk(190102);
      tlkEntry.Override = await DownloadGoogleDoc("174R6FNW5dO_GxVQyqvndCm0vTnyZ6sTqlDtKVcFSklA");

      tlkEntry = StrRef.FromCustomTlk(190103);
      tlkEntry.Override = await DownloadGoogleDoc("1m5JzqgayChh6D64b6L53AqqoJvyhjt0gLKdHhUIRh94");

      tlkEntry = StrRef.FromCustomTlk(190104);
      tlkEntry.Override = await DownloadGoogleDoc("1Gtoy2ELVBlY1l8YaSqiFp4GUKgPbDqRyNZLFxD-7lYU");

      tlkEntry = StrRef.FromCustomTlk(190105);
      tlkEntry.Override = await DownloadGoogleDoc("1HTCpVKh_FQqvdLfVWLy8P2--tucvL6k1ipUYngL1uto");

      tlkEntry = StrRef.FromCustomTlk(190106);
      tlkEntry.Override = await DownloadGoogleDoc("17lww5fKedFXoigqIlyTBdyM_-Id9MRKp4qs-HkmYX-0");

      tlkEntry = StrRef.FromCustomTlk(190107);
      tlkEntry.Override = await DownloadGoogleDoc("1lELBdXP7nR8oa_EWDh0DlMk9rtZUxtZ1vt-7Yuk8xFg");

      tlkEntry = StrRef.FromCustomTlk(190108);
      tlkEntry.Override = await DownloadGoogleDoc("1ReBm1WQocMnbz0yKQV_cRtd73N2YwXQXvl0Iv44LAhg");

      tlkEntry = StrRef.FromCustomTlk(190109);
      tlkEntry.Override = await DownloadGoogleDoc("1ll6A-ekXUXKj0crQrAYLRtn6HowvnXCo8JprxzKLpu8");
    }
  }
}
