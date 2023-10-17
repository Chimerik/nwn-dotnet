using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anvil.API;

namespace NWN
{
  public static partial class StringUtils
  {
    public static void InitializeTlkOverrides()
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
    }
  }
}
