using Helpers;
using System.Data;
namespace BO;

public class CallInList
{
    public int? AssignmateId {  get; set; }
    public int CallId {  get; set; }
    public CallType Type { get; set; }
    public DateTime OpeningTime { get; set; }
    public TimeSpan ? TimeToEnd { get; set; }
    public TimeSpan? TotalTreatmentTime { get; set; }
    public string? LastVolunteer { get; set; }
    public CallStatus Status { get; set; }
    public int NumberOfAssignmates { get; set; }
    public override string ToString() => this.ToStringProperty();
}
