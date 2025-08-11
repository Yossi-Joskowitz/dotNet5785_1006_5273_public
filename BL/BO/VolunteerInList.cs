using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public bool Active { get; set; }
    public int NumberOfCallsThatTreated { get; init; }
    public int NumberOfCallsThatSelfCanceled { get; init; }
    public int NumberOfCallsTakenAndExpired { get; init; }
    public int NumberOfCallsTakenAndCancelingAnAdministrator { get; init; }
    public int ?NumberOfIdCall { get; init; }
    public CallType Type { get; init; }
    public override string ToString() => this.ToStringProperty();
}
