using NWN.API;

namespace NWN.Systems
{
  class SurchargeArcanique
  {
    public SurchargeArcanique(NwPlayer oPC, NwGameObject oTarget)
    {
      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      if (!(oTarget is NwItem))
      {
        oPC.SendServerMessage($"{oTarget.Name.ColorString(Color.WHITE)} n'est pas un objet et ne peut donc pas être surchargé.", Color.RED);
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
        item.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
        oPC.SendServerMessage($"En forçant à l'aide de votre puissance brute, vous parvenez à ajouter un emplacement de sort supplémentaire à votre {item.Name.ColorString(Color.WHITE)} !", Color.NAVY);
      }
      else if (dice > surchargeLevel + controlLevel)
      {
        item.Destroy();
        oPC.SendServerMessage($"Vous forcez, forcez, et votre {item.Name.ColorString(Color.WHITE)} se brise sous l'excès infligé.", Color.PURPLE);
      }
    }
  }
}
