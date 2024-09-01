namespace Ruby.Commands;

public struct InvalidParameterValue
{
    internal InvalidParameterValue(string value)
    {
        this.value = value;
    }

    internal string value;
}