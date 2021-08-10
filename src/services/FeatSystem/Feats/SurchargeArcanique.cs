using Anvil.API;

namespace NWN.Systems
{
  class SurchargeArcanique
  {
    public SurchargeArcanique(NwPlayer oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
        return;

      if (!(oTarget is NwItem))
      {
        oPC.SendServerMessage($"{oTarget.Name.ColorString(ColorConstants.White)} n'est pas un objet et ne peut donc pas être surchargé.", ColorConstants.Red);
        return;
      }

      NwItem item = (NwItem)oTarget;

      int surchargeLevel = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.SurchargeArcanique))
        surchargeLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SurchargeArcanique, player.learntCustomFeats[CustomFeats.SurchargeArcanique]);

      int controlLevel = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.SurchargeControlee))
        controlLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.SurchargeControlee, player.learntCustomFeats[CustomFeats.SurchargeControlee]);


      int dice = NwRandom.Roll(Utils.random, 100);

      if(dice <= surchargeLevel)
      {
        item.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
        oPC.SendServerMessage($"En forçant à l'aide de votre puissance brute, vous parvenez à ajouter un emplacement de sort supplémentaire à votre {item.Name.ColorString(ColorConstants.White)} !", ColorConstants.Navy);
      }
      else if (dice > surchargeLevel + controlLevel)
      {
        item.Destroy();
        oPC.SendServerMessage($"Vous forcez, forcez, et votre {item.Name.ColorString(ColorConstants.White)} se brise sous l'excès infligé.", ColorConstants.Purple);
      }
    }
  }
}
