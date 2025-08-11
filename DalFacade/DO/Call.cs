
namespace DO;
/// <summary>
/// entity calls
/// </summary>
/// <param name="Id">The id of the call.</param>
/// <param name="Type"> The type of service call.</param>
/// <param name="FullCallAddress">Full address of the call to service.</param>
/// <param name="Latitude">Latitude of the call to service.</param>
/// <param name="Longitude">Longitude of the call to service.</param>
/// <param name="OpeningTime">The opening time of the call to the service.</param>
/// <param name="VerbalDescription">Description of the service required.</param>
/// <param name="MaxTimeToFinish">Maximum time to end the call to the service.</param>
public record Call
(
    int Id,
    CallType Type,
    string FullCallAddress,
    double Latitude,
    double Longitude,
    DateTime OpeningTime,
    string? VerbalDescription = null,
    DateTime? MaxTimeToFinish = null
)
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Call() : this(0, CallType.FoodDelivery, "", 0, 0, DateTime.Now) { }

    
}

