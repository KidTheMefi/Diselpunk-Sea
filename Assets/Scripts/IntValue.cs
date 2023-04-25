

public struct IntValue 
{
    public int MaxValue { get; private set; }
    public int MinValue { get; private set; }
    public int CurrentValue { get; private set; }

    public bool FullValue => CurrentValue == MaxValue;

    public IntValue(int maxValue, int minValue = 0)
    {
        MaxValue = maxValue;
        MinValue = minValue;
        CurrentValue = maxValue;
    }

    public void ChangeValueFor(int value)
    {
        var newValue = CurrentValue + value;
        CurrentValue = newValue < MinValue ? MinValue : newValue > MaxValue ? MaxValue : newValue;
    }

    public void SetValueTo(int value)
    {
        CurrentValue = value < MinValue ? MinValue : value > MaxValue ? MaxValue : value;
    }
    
}
