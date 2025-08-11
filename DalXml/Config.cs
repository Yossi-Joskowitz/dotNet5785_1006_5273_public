namespace Dal;
/// <summary>
/// System data to an xml file
/// </summary>
internal static class Config
{
    /// <summary>
    /// Constant variables that accept certain strings for comparison
    /// </summary>
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_assignments_xml = "assignments.xml";


    /// <summary>
    /// A running variable for assignment counting
    /// </summary>
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    /// <summary>
    /// A running variable for call count
    /// </summary>
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);

    }
    /// <summary>
    /// System clock type variable
    /// </summary>
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    /// <summary>
    /// A risk range type variable
    /// </summary>
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }


    /// <summary>
    /// Reset all system fields
    /// </summary>
    internal static void Reset()
    {
        NextAssignmentId = 1;
        NextCallId = 1;
        Clock = DateTime.Now;
        RiskRange = new TimeSpan(0, 30, 0);

    }
}
