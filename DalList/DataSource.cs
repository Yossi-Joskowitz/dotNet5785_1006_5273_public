

namespace Dal;

internal static  class DataSource
{
    /// <summary>
    /// A list of all the volunteers.
    /// </summary>
    internal static List<DO.Volunteer>? Volunteers { get; } = new();
    /// <summary>
    /// A list of all the calls.
    /// </summary>
    internal static List<DO.Call> ?Calls { get; } = new();
    /// <summary>
    /// A list of all the assignments.
    /// </summary>
    internal static List<DO.Assignment>? Assignments { get; } = new();
}
