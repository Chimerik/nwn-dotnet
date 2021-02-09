using NWN.API;
using NWN.Core.NWNX;

namespace NWN.Systems.Items.ItemUseHandlers
{
    public static class SkillBook
    {
        public static void HandleActivate(NwItem skillBook, NwPlayer oPC)
        {
            int FeatId = skillBook.GetLocalVariable<int>("_SKILL_ID").Value;

            if (CreaturePlugin.GetHighestLevelOfFeat(oPC, FeatId) == (int)Feat.Invalid)
            {
                if (PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
                {
                    if (!player.learnableSkills.ContainsKey(FeatId))
                    {
                        Systems.SkillBook.pipeline.Execute(new Systems.SkillBook.Context(
                          oItem: skillBook,
                          oActivator: player,
                          SkillId: FeatId
                        ));
                    }
                    else
                        oPC.SendServerMessage("Cette capacité se trouve déjà dans votre liste d'apprentissage.");
                }
            }
            else
                oPC.SendServerMessage("Vous connaissez déjà les bases d'entrainement de cette capacité.");
        }
    }
}
