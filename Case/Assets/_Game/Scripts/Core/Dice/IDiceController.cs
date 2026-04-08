namespace Core.Dice
{
    public interface IDiceController
    {
        public void RollDice(int dice1Value, int dice2Value);
        public void RollDice(int[] diceValues);
    }
}