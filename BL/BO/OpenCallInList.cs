using Helpers;

namespace BO;

public class OpenCallInList
{
    public int CallId { get; init; }
    public CallType Type { get; init; }
    public string ?VerbalDescription { get; init; }
    public string Address { get; init; }
    public DateTime OpeningTime { get; init; }
    public DateTime? MaxTime { get; init; }
    public double DistanceFromVolunteer { get; init; }
    public override string ToString() => this.ToStringProperty();

}
