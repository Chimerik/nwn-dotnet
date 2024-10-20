using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeNewCharacter()
      {
        LogUtils.LogMessage($"{oid.PlayerName} vient de créer un nouveau personnage : {oid.LoginCreature.Name}", LogUtils.LogType.PlayerConnections);

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").Value = 1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_STATS").Value = 1;

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_REMAINING_ABILITY_POINTS").Value = 27;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_MAIN_BONUS").Value = -1;
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_CHARACTER_CHOSEN_SECONDARY_BONUS").Value = -1;

        for (int i = 0; i < 6; i++)
        {
          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>($"_CHARACTER_SET_ABILITY_{i}").Value = 8;
          oid.LoginCreature.SetsRawAbilityScore((Ability)i, 8);
        }

        Utils.DestroyInventory(oid.LoginCreature);
        ClearKnownSpells();
        CreateCharacterSkin();     
        CreateCharacterDB(CreateIntroScene(oid, areaSystem));
        InitializeStartingLearnables();

        oid.LoginCreature.SetQuickBarButton(0, new PlayerQuickBarButton { ObjectType = QuickBarButtonType.Empty });
      }
    }
  }
}
