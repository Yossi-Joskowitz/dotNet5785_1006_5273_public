using BlApi;
using BO;
using DalApi;
using DO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Helpers;


/// <summary>
/// A helper class that manages volunteer-related operations.
/// Includes methods for converting between BO and DO objects, validation, and utility functions.
/// </summary>
internal static class VolunteerManager
{
    internal static ObserverManager Observers = new();

    /// <summary>
    /// A helper variable that gets all the details from the data layer so we can get to them
    /// </summary>
    private static IDal s_dal = DalApi.Factory.Get;


    /// <summary>
    /// Converts a BO.Volunteer object to a DO.Volunteer object.
    /// Includes validation and geolocation lookup for the address.
    /// </summary>
    /// <param name="boVolunteer">The BO.Volunteer object to convert.</param>
    /// <returns>A DO.Volunteer object representing the same volunteer.</returns>
    internal static DO.Volunteer ConvertFromBOVolunteerToDOVolunteer(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer
        {
            Id = boVolunteer.Id,
            Name = boVolunteer.FullName,
            PhoneNumber = boVolunteer.PhoneNumber,
            Email = boVolunteer.Email,
            FullAddress = boVolunteer.FullAddress,
            Latitude = 0,
            Longitude = 0,
            Password = boVolunteer.Password,
            RoleType = (DO.Role)boVolunteer.RoleType,
            Active = boVolunteer.Active,
            MaxDistance = boVolunteer.MaxDistance ?? null,
            DistanceType = (DO.Distance)boVolunteer.DistanceType
        };
    }

    /// <summary>
    /// Function to update longitude and latitude lines.
    /// </summary>
    internal static async Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer)
    {

        (double? latitude, double? longitude) = await Tools.GetCoordinates((string)doVolunteer.FullAddress!);

        if (latitude != null && longitude != null)
        {
            doVolunteer = doVolunteer with { Latitude = latitude, Longitude = longitude };
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(doVolunteer);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(doVolunteer.Id);
        }
        else
        {
            throw new BO.BlDoesNotExistException("The longitudes were not found.");
        }

    }



    /// <summary>
    /// Converts a DO.Volunteer object to a BO.Volunteer object.
    /// Includes additional processing to enrich the BO object with calculated fields.
    /// </summary>
    /// <param name="doVolunteer">The DO.Volunteer object to convert.</param>
    /// <returns>A BO.Volunteer object representing the same volunteer.</returns>
    internal static BO.Volunteer ConvertFromDOVolunteerToBOVolunteer(DO.Volunteer doVolunteer)
    {
        return new BO.Volunteer
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            FullAddress = doVolunteer.FullAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            RoleType = (BO.Role)doVolunteer.RoleType,
            Active = doVolunteer.Active,
            DistanceType = (BO.Distance)doVolunteer.DistanceType,
            MaxDistance = doVolunteer.MaxDistance,
            NumberOfCallsThatTreated = GetNumberOfCallsThatTreated(doVolunteer.Id),
            NumberOfCallsThatSelfCanceled = GetNumberOfCallsThatSelfCanceled(doVolunteer.Id),
            NumberOfCallsTakenAndExpired = GetNumberOfCallsTakenAndExpired(doVolunteer.Id),
            callInProgress = GetCallInProgress(doVolunteer)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doVolunteer"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    internal static BO.CallInProgress? GetCallInProgress(DO.Volunteer doVolunteer)
    {
        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.ExitTime == null).LastOrDefault();

        if (assignment == null)
            return null;

        BO.CallStatus status = Tools.GetCallStatus(assignment.CallId);

        DO.Call? call;
        lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(call => call.Id == assignment.CallId && (status == BO.CallStatus.InTreatment || status == BO.CallStatus.RiskTreatment));

        if (call == null)
            return null;

        return new BO.CallInProgress
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            Type = (BO.CallType)call.Type,
            VerbalDescription = call.VerbalDescription ?? null,
            Address = call.FullCallAddress,
            OpeningTime = call.OpeningTime,
            MaxTime = call.MaxTimeToFinish ?? null,
            EnteringTime = assignment.EntryTime,
            DistanceFromVolunteer = Tools.GetDistance(doVolunteer.Latitude, doVolunteer.Longitude, call.Latitude, call.Longitude, doVolunteer.DistanceType),
            StatusType = status
        };

    }


    /// <summary>
    /// Converts a DO.Volunteer object to a BO.VolunteerInList object.
    /// </summary>
    /// <param name="doVolunteer">The DO.Volunteer object to convert.</param>
    /// <returns>A BO.VolunteerInList object representing the volunteer.</returns>
    internal static BO.VolunteerInList ConvertFronDOVolunteerToBOVolunteerInList(DO.Volunteer doVolunteer)
    {
        int? numberOfIdCall = GetIdOfCallThatStillOpen(doVolunteer.Id);

        BO.CallType callType;
        lock (AdminManager.BlMutex)
            callType = numberOfIdCall == null ? BO.CallType.None : (BO.CallType)s_dal.Call.Read(numberOfIdCall.Value).Type;



        return new BO.VolunteerInList
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.Name,
            Active = doVolunteer.Active,
            NumberOfCallsThatTreated = GetNumberOfCallsThatTreated(doVolunteer.Id),
            NumberOfCallsThatSelfCanceled = GetNumberOfCallsThatSelfCanceled(doVolunteer.Id),
            NumberOfCallsTakenAndExpired = GetNumberOfCallsTakenAndExpired(doVolunteer.Id),
            NumberOfCallsTakenAndCancelingAnAdministrator = GetNumberOfCallsTakenAndCancelingAnAdministrator(doVolunteer.Id),
            NumberOfIdCall = numberOfIdCall,
            Type = callType
        };
    }
    /// <summary>
    /// Sorts a list of BO.VolunteerInList objects based on the specified sorting criteria.
    /// </summary>
    /// <param name="volunteers">The list of volunteers to sort.</param>
    /// <param name="sorting">The sorting criteria.</param>
    /// <returns>A sorted IEnumerable of BO.VolunteerInList objects.</returns>
    internal static IEnumerable<BO.VolunteerInList> SortList(IEnumerable<BO.VolunteerInList> volunteers, BO.TypeOfSortingVolunteer? sorting)
    {
        return sorting switch
        {
            BO.TypeOfSortingVolunteer.Name => volunteers.OrderBy(v => v.FullName),
            BO.TypeOfSortingVolunteer.NumberOfCallsThatTreated => volunteers.OrderBy(v => v.NumberOfCallsThatTreated),
            BO.TypeOfSortingVolunteer.NumberOfCallsThatSelfCanceled => volunteers.OrderBy(v => v.NumberOfCallsThatSelfCanceled),
            BO.TypeOfSortingVolunteer.NumberOfCallsTakenAndExpired => volunteers.OrderBy(v => v.NumberOfCallsTakenAndExpired),
            _ => volunteers.OrderBy(v => v.Id)
        };

    }



    /// <summary>
    /// Gets the number of calls treated by the specified volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The number of treated calls.</returns>
    internal static int GetNumberOfCallsThatTreated(int volunteerId)
    {
        int temp = 0;       
        lock (AdminManager.BlMutex)
           temp = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType == DO.TypeOfEnding.Treated).Count();
        return temp;
    }



    /// <summary>
    /// Gets the number of calls self-canceled by the specified volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The number of self-canceled calls.</returns>
    internal static int GetNumberOfCallsThatSelfCanceled(int volunteerId)
    {
        int temp = 0;
        lock (AdminManager.BlMutex)
            temp = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType == DO.TypeOfEnding.SelfCancellation).Count();
        return temp;
    }

    /// <summary>
    /// Gets the number of calls taken and expired by the specified volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The number of calls taken and expired.</returns>
    internal static int GetNumberOfCallsTakenAndExpired(int volunteerId)
    {
        int temp = 0;
        lock (AdminManager.BlMutex)
             temp = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType == DO.TypeOfEnding.CancellationHasExpired).Count();
        return temp;
    }

    /// <summary>
    /// Gets the number of calls taken and Canceling by the manager by the specified volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The number of calls taken and expired.</returns>
    internal static int GetNumberOfCallsTakenAndCancelingAnAdministrator(int volunteerId)
    {
        int temp = 0;
        lock (AdminManager.BlMutex)
            temp = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType == DO.TypeOfEnding.CancelingAnAdministrator).Count();
        return temp;
    }

    /// <summary>
    /// Gets the ID of the call that is still open for the specified volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The ID of the open call, or null if none exist.</returns>
    internal static int? GetIdOfCallThatStillOpen(int volunteerId)
    {
        int ?temp = 0;
        lock (AdminManager.BlMutex)
            temp = s_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.ExitTime == null && (Tools.GetCallStatus(a.CallId) == BO.CallStatus.InTreatment || Tools.GetCallStatus(a.CallId) == BO.CallStatus.RiskTreatment)).FirstOrDefault()?.CallId;
        return temp;
    }


    /// <summary>
    /// Checks if the requester has permission to update a volunteer's details.
    /// </summary>
    /// <param name="requesterId">The ID of the requester.</param>
    /// <param name="volunteerId">The ID of the volunteer whose details are being updated.</param>
    /// <returns>True if the requester is an administrator or the volunteer themselves; otherwise, false.</returns>
    internal static bool CanUpdate(int requesterId, int volunteerId)
    {

        DO.Volunteer ?volunteer;
        lock (AdminManager.BlMutex)
            volunteer = s_dal.Volunteer.Read(requesterId);
        if(volunteer==null)
            throw new BO.BlDoesNotExistException("The volunteer requesting to update does not exist.");
        return volunteer.RoleType == DO.Role.Management || requesterId == volunteerId;

    }


    /// <summary>
    /// Validates the integrity of a volunteer's data, ensuring the phone number, email, and password meet the required criteria.
    /// </summary>
    /// <param name="volunteer">The volunteer object to validate.</param>
    /// <exception cref="Exception">Thrown when:
    /// - The phone number is invalid (must contain 10 digits and start with '0').
    /// - The email address is improperly formatted.
    /// - The password does not meet the strength requirements (must be at least 8 characters long and include uppercase, lowercase, numeric, and special characters).</exception>
    internal static void IntegrityCheck(BO.Volunteer volunteer)
    {
        if (!ValidateId(volunteer.Id))
            throw new BO.BlDataOrFormatMismatchException("Invalid id ");

        if (!ValidatePhoneNumber(volunteer.PhoneNumber))
            throw new BO.BlDataOrFormatMismatchException("Invalid Phone number ");

        if (!ValidateEmail(volunteer.Email))
            throw new BO.BlDataOrFormatMismatchException("Invalid Email");

        if (!IsStrongPassword(volunteer.Password))
            throw new BO.BlDataOrFormatMismatchException("Password not strong enough ");

        if (volunteer.FullAddress == null || !Tools.IsValidAddress(volunteer.FullAddress))
            throw new BO.BlDataOrFormatMismatchException("The address is invalid or does not exist.");

    }


    /// <summary>
    /// Validates an ID number using a checksum algorithm.
    /// </summary>
    /// <param name="id">The ID number to validate.</param>
    /// <returns>True if the ID is valid; otherwise, false.</returns>
    public static bool ValidateId(int id)
    {

        if (id < 100000000 || id > 999999999)
            return false;
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = id % 10;
            id /= 10;
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }
            sum += digit;
        }
        return sum % 10 == 0;
    }

    /// <summary>
    /// Checks if the provided password is strong based on predefined criteria.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if strong, false otherwise.</returns>
    internal static bool IsStrongPassword(string ?password)
    {

        if (password == null || password.Length < 8)
            return false;

        bool hasUpperChar = password.Any(char.IsUpper);
        bool hasLowerChar = password.Any(char.IsLower);
        bool hasNumber = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(ch => "!@#$%^&*()_+-=[]|,.<>?".Contains(ch));

        return hasUpperChar && hasLowerChar && hasNumber && hasSpecialChar;
    }


    /// <summary>
    /// Validates the phone number format.
    /// Ensures it is a 10-digit number starting with '0'.
    /// </summary>
    /// <param name="phone">The phone number to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    internal static bool ValidatePhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return false;
        return phone.All(char.IsDigit) && phone.Length == 10 && phone.StartsWith("0");
    }




    /// <summary>
    /// Validates the email format using basic checks for structure and content.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    internal static bool ValidateEmail(string email)
    {
        // Check if the email is null, empty, or contains spaces
        if (string.IsNullOrEmpty(email) || email.Contains(" "))
            return false;

        // Find the index of the '@' character
        int atIndex = email.IndexOf('@');
        // Find the index of the last '.' character
        int dotIndex = email.LastIndexOf('.');

        // Check if there is exactly one '@' character
        if (atIndex == -1 || atIndex != email.LastIndexOf('@'))
            return false;

        // Check if the email starts with a dot, or if there is a dot immediately after the '@' or at the end of the email
        if (email[0] == '.' || email[atIndex - 1] == '.' || email[dotIndex + 1] == '.')
            return false;

        // Check if the '@' is not at the start and the dot is after '@', and that the dot is not the last character
        return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < email.Length - 1;
    }

    // Represents the BL (Business Logic) of the system
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public static Random random = new Random();
    /// <summary>
    /// A function that simulates a simulator
    /// </summary>
    internal static void Simulation()
    {
        List<BO.VolunteerInList> volunteers;
        List<int> volunteersId = new();
        List<int> callsId = new();

        volunteers = s_bl.Volunteer.ReadVolunteers(true, null).ToList();

        foreach (BO.VolunteerInList volunteer in volunteers)
        {
            if (volunteer.NumberOfIdCall == null)
            {
                List<BO.OpenCallInList> openCalls = s_bl.Call.GetOpenCalls(volunteer.Id, null, null).ToList();

                if (openCalls.Count() == 0)
                    continue;

                if (random.Next(1, 101) <= 50)
                {
                    int callId = openCalls.ElementAt(random.Next(openCalls.Count())).CallId;
                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Create(new DO.Assignment(0, callId, volunteer.Id, AdminManager.Now));

                    volunteersId.Add(volunteer.Id);
                    callsId.Add(callId);             
                }
            }
            else
            {
                DO.Assignment assignment;
                lock (AdminManager.BlMutex)
                    assignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteer.Id && a.CallId == volunteer.NumberOfIdCall && a.ExitTime == null && a.TheEndType == null)!;

                if (assignment.EntryTime < (AdminManager.Now).AddDays(-30))
                {
                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(assignment with { ExitTime = AdminManager.Now, TheEndType = DO.TypeOfEnding.Treated });
                    volunteersId.Add(volunteer.Id);
                    callsId.Add((int)volunteer.NumberOfIdCall);
                }
                else
                {
                    if (random.Next(1, 101) <= 40)
                    {
                        lock (AdminManager.BlMutex)
                            s_dal.Assignment.Update(assignment with { ExitTime = AdminManager.Now, TheEndType = DO.TypeOfEnding.SelfCancellation });
                        volunteersId.Add(volunteer.Id);
                        callsId.Add((int)volunteer.NumberOfIdCall);
                    }
                }
            }
        }

        CallManager.Observers.NotifyListUpdated();
        Observers.NotifyListUpdated();

        foreach (int id in volunteersId)
        {
            Observers.NotifyItemUpdated(id);
        }

        foreach (int id in callsId)
        {
            CallManager.Observers.NotifyItemUpdated(id);
        }
    }
}



