namespace Core.Dice
{
    public interface IDiceController
    {
        public void ThrowDice(int dice1Value, int dice2Value);
        public bool IsRolling { get; }
    }
}

