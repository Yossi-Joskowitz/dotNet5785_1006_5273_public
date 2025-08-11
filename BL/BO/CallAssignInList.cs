using Helpers;

namespace BO;

public class CallAssignInList
{

    public int ?VolunteerId {  get; set; }
    public string ?VolunteerName { get; set; }
    public DateTime EnteringTime { get; set; }
    public DateTime ?EndOfTreatment { get; set; }
    public AssignmentStatus? Status { get; set; }
    public override string ToString() => this.ToStringProperty();

}
