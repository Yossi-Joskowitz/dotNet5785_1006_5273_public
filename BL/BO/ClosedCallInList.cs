using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int CallId { get; init; }

    public CallType Type { get; init; }
    public string FullCallAddress { get; init; }
    public DateTime OpeningTime { get; init; }
    public DateTime EnteringTime { get; init; }
    public DateTime? EndOfTime { get; init; }
    public  AssignmentStatus ? Status { get; init; }
    public override string ToString() => this.ToStringProperty();

}
