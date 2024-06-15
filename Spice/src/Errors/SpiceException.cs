namespace Spice.Errors;

/// <summary>
/// Generic Spice SDK exception. 
/// </summary>
[Serializable]
public class SpiceException : Exception
{
    public SpiceStatus Status { get; private set; }

    internal SpiceException(SpiceStatus status) : this(status, status.ToString()) {}

    internal SpiceException(SpiceStatus status, string message) : base(message)
    {
        Status = status;
    }
}