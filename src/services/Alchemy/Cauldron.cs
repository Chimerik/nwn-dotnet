using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static NWN.Systems.Craft.Collect.Config;

namespace NWN.Systems.Alchemy
{
  public class Cauldron
  {
    public int characterId { get; }
    public Vector2 tablePosition { get; set; }
    public List<Vector2> ingredientVector { get; set; }
    public List<Vector2> consumedGridEffects { get; set; }
    public List<string> effectList { get; set; }
    public List<Instruction> instructions { get; set; }
    public int nBrowsedCases{ get; set; }
    public List<AddedIngredient> addedIngredients { get; set; }

    public Cauldron(int characterId)
    {
      this.characterId = characterId;
      tablePosition = AlchemySystem.center;
      ingredientVector = new List<Vector2>();
      consumedGridEffects = new List<Vector2>();
      addedIngredients = new List<AddedIngredient>();
      effectList = new List<string>();
      instructions = new List<Instruction>();
      nBrowsedCases = 0;
    }
  }

  public class AddedIngredient
  {
    public Plant ingredient { get; set; }
    public int quantity { get; set; }

    public AddedIngredient(Plant ingredient, int quantity)
    {
      this.ingredient = ingredient;
      this.quantity = quantity;
    }
  }
  public class CurrentRecipe
  {
    public List<AddedIngredient> addedIngredients { get; }
    public List<string> effectList { get; }
    public List<Instruction> instructions { get; }

    public CurrentRecipe(List<AddedIngredient> addedIngredients, List<string> effectList, List<Instruction> instructions)
    {
      this.addedIngredients = addedIngredients;
      this.effectList = effectList;
      this.instructions = instructions;
    }
  }
  public class Instruction
  {
    public InstructionType instruction { get; }
    public int quantity { get; set; }

    public Instruction(InstructionType instruction, int quantity)
    {
      this.instruction = instruction;
      this.quantity = quantity;
    }
  }
  public enum InstructionType
  {
    Invalid,
    Mix,
    Distill
  }
}
