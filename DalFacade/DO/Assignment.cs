namespace DO;
/// <summary>
/// entity that represents the Assignment
/// </summary>
/// <param name="Id"> The id of the assignment. </param>
/// <param name="CallId"> The id of the call.</param>
/// <param name="VolunteerId"> The id of the volunteerr.</param>
/// <param name="EntryTime"> The time the assignment statrted.</param>
/// <param name="ExitTime">The time the assignment ended.</param>
/// <param name="TheEndType">The reason the assignment has closed.</param>
public record Assignment
(

    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTime,
    DateTime? ExitTime = null,
    TypeOfEnding? TheEndType = null
)
{
    /// <summary>
    /// A empty default constructor.
    /// </summary>
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}