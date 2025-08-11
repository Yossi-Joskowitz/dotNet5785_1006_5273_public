namespace Dal;
/// <summary>
/// The interface for the configuration.
/// </summary>
internal static class Config
{

    internal const int callId = 1;
    private static int nextCallId = callId;
    internal static int NextCallId { get => nextCallId++; }//Running ID number of the call


    internal const int assignmentId = 1;
    private static int nextAssignmentId = assignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }//Running ID number of the assignment



    internal static DateTime Clock { get; set; } = DateTime.Now;//the system clock

    internal static TimeSpan RiskRange { get; set; } = new TimeSpan(0, 30, 0);//the range of time for the risk

    /// <summary>
    /// The function resets the system clock and the risk range and the next call and assignment id.
    /// </summary>
    internal static void Reset()
    {
        nextCallId = callId;
        nextAssignmentId = assignmentId;
        Clock = DateTime.Now;
        RiskRange = new TimeSpan(0, 30, 0);
    }

}
