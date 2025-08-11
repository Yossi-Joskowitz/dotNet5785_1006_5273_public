using Helpers;

namespace BO;
public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ?Password { get; set; }
    public string ?FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Role RoleType { get; set; }
    public bool Active { get; set; }
    public Distance DistanceType { get; set; }
    public double? MaxDistance { get; set; }
    public int NumberOfCallsThatTreated { get; init; }
    public int NumberOfCallsThatSelfCanceled { get; init; }
    public int NumberOfCallsTakenAndExpired { get; init; }
    public CallInProgress? callInProgress{get; init; }

    public override string ToString() => this.ToStringProperty();


}
