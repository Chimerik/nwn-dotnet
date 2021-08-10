using System.Threading.Tasks;
using Discord.Commands;

namespace NWN.Systems
{
    public static partial class BotSystem
    {
        public static async Task ExecuteDisplayHealInfoCommand(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Avant de parler des soins, il faut parler de l'interprétation des points de vie et des blessures.\n\n" +
              "Les points de vie dans D&D ne correspondent pas au nombre de blessures qu'un personnage peut encaisser avant d'être K.O., mais à la résilience du personnage.\n\n" +
              "Perdre 1d8 + 4 points de vie ne signifie pas que vous venez de vous prendre un coup d'épée dans le ventre et que vous perdez votre sang, mais simplement que la résilience de votre personnage diminue, car l'attaque précédente l'a placé dans en mauvaise posture et qu'il n'a pas encore pu récupérer.\n\n" +
              "C'est la raison pour laquelle des points de vie sont régénérés après repos. Les blessures ne cicatrisent pas en une nuit, en revanche le personnage retrouve une partie de son aplomb perdu lors des combats.\n\n" +
              "C'est aussi la raison pour laquelle les PV doublent lors d'un passage de niveau 1 à 2.Le personnage ne devient pas subitement capable d'encaisser deux fois de plus de coups d'épées dans le ventre: son expérience lui permet en revanche de conserver son aplomb deux fois plus longtemps lors des combats.\n\n" +
              "C'est également la raison pour laquelle l'efficacité au combat du personnage est strictement la même qu'il lui reste 1 PV ou qu'il soit au maximum, alors que logiquement, un personnage très fortement blessé ('agonisant') devrait se déplacer moins vite, moins bien attaquer, avoir des difficultés à se concentrer, etc.\n\n" +
              "En revanche, lorsque les PV tombent à 0, la résilience du personnage est épuisée et celui-ci commet une erreur fatale qui le met hors de combat. C'est à ce moment que l'épée lui traverse le ventre / l'épaule / le genou, qu'il ne peut plus combattre et qu'il écope d'une véritable blessure pénalisante dont il lui faudra du temps pour se débarrasser.\n\n" +
              "Lorsque les PV tombent en dessous de - 11, la blessure infligée est considérée fatale.\n\n"
              );

            await context.Channel.SendMessageAsync("\n\nSur les Larmes des Erylies, nous considérons donc que les sorts de soins ne permettent pas de refermer des plaies ouvertes ni de ressouder les os. Ils permettent en revanche de rétablir la résilience et l'aplomb au combat d'un personnage.\n\n" +
              "L'idée étant d'adopter un concept plus 'low magic' et de rendre leur sens aux soins physiques tout en nerfant la puissance rp des soins magiques sans que ça ne change rien au système de combat.\n\n" +
              "De la même façon, les sorts comme restauration / délivrance des malédictions, etc, permettent de se débarrasser des statuts génériques 'de combat', comme ceux infligés par le sort malédiction.\n\n" +
              "En revanche, pour se débarrasser d'une malédiction ou d'un statut rp, il faudra remonter à la source de la malédiction et trouver la condition rp qui permet de la lever.\n\n");
        }
    }
}
