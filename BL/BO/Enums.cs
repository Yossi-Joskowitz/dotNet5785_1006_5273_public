namespace BO;

/// <summary>
/// Represents the role of a volunteer.
/// </summary>
public enum Role { Volunteer, Management }
/// <summary>
/// Represents the type of distance calculation.
/// </summary>
public enum Distance { Aerial, Walking, Driving }

/// <summary>
/// Represents the type of the call.
/// </summary>
public enum CallType
{
    FoodDelivery,
    EquipmentDelivery,
    EquipmentRepair,
    Rides,
    Laundry,
    StorageOfBelongings,
    MedicalHelp,
    EventOrganization,
    PsychologicalAssistance,
    MotivationalTalks,
    None
};
/// <summary>
/// Treatment status.
/// </summary>
public enum VolunteerStatus {InTreatment , RiskTreatment }

/// <summary>
/// Call status.
/// </summary>
public enum CallStatus { Open, InTreatment,Close, Expired,OpenAtRisk, RiskTreatment }
/// <summary>
/// Assignment status
/// </summary>
public enum AssignmentStatus { Treated, SelfCancellation, CancelingAnAdministrator, CancellationHasExpired }

public enum TypeOfSortingVolunteer {Id,Name, NumberOfCallsThatTreated, NumberOfCallsThatSelfCanceled, NumberOfCallsTakenAndExpired }

public enum TypeOfFiltering { Type, Status }

public enum TypeOfSortingCall { AssignmateId, CallId, OpeningTime, NumberOfAssignmates, TotalTreatmentTime }

public enum TypeOfSortingClosedCalls { CallId, OpeningTime, EnteringTime, EndOfTime }

public enum TypeOfSortingOpenCalls { CallId, OpeningTime, DistanceFromVolunteer }

public enum TypeOfTime { Minute, Hour, Day, Month, Year }
