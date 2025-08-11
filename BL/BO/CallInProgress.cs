using Helpers;
using System.Security.Cryptography.X509Certificates;

namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType Type { get; init; }
    public string ?VerbalDescription { get; init; }
    public string Address { get; init; }
    public DateTime OpeningTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public DateTime EnteringTime { get; init; }
    public double DistanceFromVolunteer { get; init; }
    public CallStatus StatusType { get; init; }
    public override string ToString() => this.ToStringProperty();


}
