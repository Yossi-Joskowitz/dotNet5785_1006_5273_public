namespace DO;

/// <summary>
/// Represents the role of a volunteer.
/// </summary>
public enum Role{Volunteer,Management}

/// <summary>
/// Represents the type of distance calculation.
/// </summary>
public enum Distance { Aerial , Walking , Driving  }


/// <summary>
/// Represents the type of the call.
/// </summary>
public enum CallType {
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
/// Represents the type of the type of ending of the assignment.
/// </summary>
public enum TypeOfEnding { Treated, SelfCancellation, CancelingAnAdministrator, CancellationHasExpired }
