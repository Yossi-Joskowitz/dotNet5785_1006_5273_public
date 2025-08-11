using BO;
using DalApi;
using static System.Net.Mime.MediaTypeNames;
namespace Helpers;

/// <summary>
/// A class that helps with the call details
/// </summary>
internal static class CallManager
{

    internal static ObserverManager Observers = new();


    /// <summary>
    /// A helper variable that gets all the details from the data layer so we can get to them
    /// </summary>
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Correctness check for times and address
    /// </summary>
    /// <param name="call">A variable with which I check format and correctness </param>
    /// <exception cref="BlDataOrFormatMismatchException">The details the user entered are incorrect</exception>
    internal static void ValidateCall(BO.Call call)
    {
        if (!DateTime.TryParse(call.OpeningTime.ToString(), out DateTime result1))
            throw new BlDataOrFormatMismatchException("The time is not valid.");

        if (call.MaxTime != null && !DateTime.TryParse(call.MaxTime.ToString(), out DateTime result2))
            throw new BlDataOrFormatMismatchException("The time is not valid.");

        if ((call.MaxTime != null && call.OpeningTime > call.MaxTime) || call.OpeningTime > AdminManager.Now || (call.MaxTime != null && call.MaxTime < AdminManager.Now))
            throw new BlDataOrFormatMismatchException("The time is not valid.");

        if (!Tools.IsValidAddress(call.FullAddress))
            throw new BlDataOrFormatMismatchException("The address is not valid.");
    }

    /// <summary>
    /// Conversion between 2 types of call
    /// </summary>
    /// <param name="boCall">Receiving a variable of a certain type for the purpose of converting it to another type</param>
    /// <returns>DO.Call</returns>
    internal static DO.Call ConversionBoTODo(BO.Call boCall)
    {
        return new DO.Call
        {
            Id = boCall.Id,
            FullCallAddress = boCall.FullAddress,
            Latitude = 0,
            Longitude = 0,
            OpeningTime = boCall.OpeningTime,
            MaxTimeToFinish = boCall.MaxTime,
            VerbalDescription = boCall.VerbalDescription,
            Type = (DO.CallType)boCall.Type
        };
    }

    /// <summary>
    /// Function to update longitude and latitude lines.
    /// </summary>
    internal static async Task UpdateCoordinatesForCallAddressAsync(DO.Call doCall)
    {
        if (doCall.FullCallAddress is not null)
        {
            (double? latitude, double? longitude) = await Tools.GetCoordinates((string)doCall.FullCallAddress);

            if (latitude != null && longitude != null)
            {
                doCall = doCall with { Latitude = (double)latitude, Longitude = (double)longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);
                Observers.NotifyListUpdated();
                Observers.NotifyItemUpdated(doCall.Id);
            }
            else
            {
                throw new BO.BlDoesNotExistException("The longitudes were not found.");
            }
        }
    }

    /// <summary>
    /// Conversion between 2 types of call
    /// </summary>
    /// <param name="doCall">Receiving a variable of a certain type for the purpose of converting it to another type</param>
    /// <returns>BO.Call</returns>
    internal static BO.Call ConversionDoTOBo(DO.Call doCall)
    {
        lock (AdminManager.BlMutex)
            return new BO.Call
            {
                Id = doCall.Id,
                FullAddress = doCall.FullCallAddress,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpeningTime = doCall.OpeningTime,
                MaxTime = doCall.MaxTimeToFinish,
                VerbalDescription = doCall.VerbalDescription,
                Type = (BO.CallType)doCall.Type,
                StatusType = Tools.GetCallStatus(doCall.Id),
                AssignInList = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id).Select(a => new BO.CallAssignInList
                {
                    VolunteerName = s_dal.Volunteer.Read(a.VolunteerId)?.Name ?? null,
                    VolunteerId = a.VolunteerId,
                    EnteringTime = a.EntryTime,
                    EndOfTreatment = a.ExitTime,
                    Status = (BO.AssignmentStatus?)a.TheEndType
                }).ToList()
            };
    }

    /// <summary>
    /// conversion function
    /// </summary>
    /// <param name="doCall"> The object to be converted</param>
    /// <returns> The function returns an object of the converted type</returns>
    internal static BO.CallInList ConversionDoTOCallInList(DO.Call doCall)
    {

        DO.Assignment? LastAssignment;
        lock (AdminManager.BlMutex)
            LastAssignment = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id).LastOrDefault();

        DateTime? dateTime = LastAssignment?.EntryTime;
        BO.CallStatus status = Tools.GetCallStatus(doCall.Id);
        TimeSpan? timeToEnd = null;

        if (doCall.MaxTimeToFinish != null)
        {
            if (status == BO.CallStatus.Close || status == BO.CallStatus.Expired)
                timeToEnd = TimeSpan.Zero;
            else
                timeToEnd = doCall.MaxTimeToFinish - AdminManager.Now;
        }

        string? lastVolunteer = LastAssignment == null ? null : s_dal.Volunteer.Read(v => v.Id == LastAssignment.VolunteerId)?.Name;
        int numberOfAssignmates = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id).Count();


        return new BO.CallInList
        {
            AssignmateId = LastAssignment != null ? LastAssignment.Id : null,
            CallId = doCall.Id,
            Type = (BO.CallType)doCall.Type,
            OpeningTime = doCall.OpeningTime,
            TimeToEnd = timeToEnd,
            TotalTreatmentTime = LastAssignment != null && LastAssignment.TheEndType == DO.TypeOfEnding.Treated ? LastAssignment.ExitTime - doCall.OpeningTime : null,
            LastVolunteer = lastVolunteer,
            Status = status,
            NumberOfAssignmates = numberOfAssignmates
        };

    }
}








