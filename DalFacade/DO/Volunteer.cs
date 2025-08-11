namespace DO;

/// <summary>
/// A volunteer entity
/// </summary>
/// <param name="Id">The unique identifier for the volunteer. </param>
/// <param name="Name"> The name of the volunteer.</param>
/// <param name="PhoneNumber"> The contact phone number of the volunteer.</param>
/// <param name="Email"> The contact email address of the volunteer. </param>
/// <param name="RoleType"> The role of the volunteer, Is the volunteer a volunteer or a manager? </param>
/// <param name="Active">Indicates whether the volunteer is active. </param>
/// <param name="DistanceType">The type of distance calculation.</param>
/// <param name="Password">The optional password of the volunteer.</param>
/// <param name="FullAddress">The optional full address of the volunteer.</param>
/// <param name="Latitude">The optional latitude coordinate of the volunteer.</param>
/// <param name="Longitude">The optional longitude coordinate of the volunteer.</param>
/// <param name="MaxDistance">The optional maximum distance the volunteer is willing to travel.</param>
public record Volunteer
(
    
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    Role RoleType,
    bool Active,
    Distance DistanceType,
    string? Password ,
    string? FullAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    double? MaxDistance = null
)
{
    /// <summary>
    /// A empty default constructor.
    /// </summary>
    public Volunteer(): this(0,"","","",Role.Volunteer,false, Distance.Aerial,"") { }
}
