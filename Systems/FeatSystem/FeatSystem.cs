using Anvil.Services;
using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  [ServiceBinding(typeof(FeatSystem))]
  public partial class FeatSystem
  {
    public static void OnUseFeatBefore(OnUseFeat onUseFeat)
    {
      if (!PlayerSystem.Players.TryGetValue(onUseFeat.Creature.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;

      switch (onUseFeat.Feat.Id)
      {
        case CustomSkill.EnlargeDuergar:
          onUseFeat.Creature.GetObjectVariable<LocalVariableInt>("_ENLARGE_DUERGAR").Value = 1;
          return;

        case CustomSkill.HellishRebuke:
          HellishRebuke(onUseFeat.Creature);
          onUseFeat.PreventFeatUse = true;
          return;

        case CustomSkill.MaitreBouclier:
          MaitreBouclier(onUseFeat.Creature, onUseFeat.TargetObject);
          onUseFeat.PreventFeatUse = true;
          return;

        //case CustomSkill.Sprint: Sprint(onUseFeat.Creature, player); return;
        //case CustomSkill.Disengage: Disengage(onUseFeat.Creature); return;
        //case CustomSkill.Dodge: Dodge(onUseFeat.Creature, player); return;
        case CustomSkill.FighterSecondWind: SecondWind(onUseFeat.Creature); return;
        case CustomSkill.FighterSurge: ActionSurge(onUseFeat.Creature); return;
        case CustomSkill.CogneurLourd: CogneurLourd(onUseFeat.Creature); return;
        case CustomSkill.TireurDelite: TireurDelite(onUseFeat.Creature); return;
        case CustomSkill.MageDeGuerre: MageDeGuerre(onUseFeat.Creature); return;
        case CustomSkill.FureurOrc: FureurOrc(onUseFeat.Creature); return;
        case CustomSkill.AgressionOrc: AgressionOrc(onUseFeat.Creature, onUseFeat.TargetObject); return;
        case CustomSkill.MeneurExaltant: MeneurExaltant(onUseFeat.Creature); return;
        case CustomSkill.Chanceux: Chanceux(onUseFeat.Creature); return;
      }

      int featId = onUseFeat.Feat.Id + 10000;

      /*SkillSystem.Attribut featAttribute = SkillSystem.learnableDictionary[featId].attribut;
      int attributeLevel = player.GetAttributeLevel(featAttribute);
      int bonusAttributeChance = 0;

      NwItem castItem = onUseFeat.Creature.GetItemInSlot(InventorySlot.RightHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      castItem = onUseFeat.Creature.GetItemInSlot(InventorySlot.LeftHand);

      if (castItem is not null)
      {
        if (castItem is not null && ItemUtils.GetItemAttribute(castItem) == featAttribute)
          for (int i = 0; i < castItem.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
            if (castItem.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == CustomInscription.Maîtrise)
              bonusAttributeChance += 3;
      }

      if (NwRandom.Roll(Utils.random, 100) < bonusAttributeChance)
        attributeLevel += 1;*/

      switch (featId)
      {
        case CustomSkill.SeverArtery:
          onUseFeat.Creature.GetObjectVariable<LocalVariableInt>("_NEXT_ATTACK").Value = featId;
          return;
      }

      switch (onUseFeat.Feat.FeatType)
      {
        case CustomFeats.CustomMenuUP:
        case CustomFeats.CustomMenuDOWN:
        case CustomFeats.CustomMenuSELECT:
        case CustomFeats.CustomMenuEXIT:

          onUseFeat.PreventFeatUse = true;
          player.EmitKeydown(new PlayerSystem.Player.MenuFeatEventArgs(onUseFeat.Feat.FeatType));
          break;
      }
    }
  }
}
