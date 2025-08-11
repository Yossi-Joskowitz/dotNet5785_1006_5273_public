namespace BlImplementation;
using BlApi;

using Helpers;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

/// <summary>
/// Exercising all the functions of call
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// The observer manager for the call.    
    /// </summary>
    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer);

    /// <summary>
    /// Creating a variable through the factory for producing variables of the call type to reach a database through it
    /// </summary>
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// A function to add a call
    /// </summary>
    /// <param name="boCall">The call to add</param>
    /// <exception cref="BO.BlDoesNotExistException">Belting when the address is not found</exception>
    public void Add(BO.Call boCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Call docall;
        try
        {
            //function to check normality and format
            CallManager.ValidateCall(boCall);

            //Creating a new object of type DO.Call
            docall = CallManager.ConversionBoTODo(boCall);

            lock (AdminManager.BlMutex)
                _dal.Call.Create(docall);

            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }

        _ = CallManager.UpdateCoordinatesForCallAddressAsync(docall);
    }

    /// <summary>
    /// A function to select a call for treatment
    /// </summary>
    /// <param name="volunteerId">Voluntary ID </param>
    /// <param name="callId">Call ID </param>
    /// <exception cref="BO.BlDoesNotExistException"> An exception with no volunteer or a call with such an ID</exception>
    /// <exception cref="BO.BlAlreadyExistsException">An exception with the call is already being handled </exception>
    /// <exception cref="BO.BlXMLFileLoadCreateException">Exception if there is an error in the xml files </exception>
    public void ChoosingACallForTreatment(int volunteerId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException("The volunteer does not exist. ");

            DO.Call call;
            lock (AdminManager.BlMutex)
                call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException("The call does not exist. ");

            lock (AdminManager.BlMutex)
                if (_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.ExitTime == null && a.TheEndType == null).Any())
                    throw new BO.BlAttemptToPerformAnIllegalActionException("The volunteer has another open assignment.. ");

            BO.CallStatus Status = Tools.GetCallStatus(callId);

            if (Status != BO.CallStatus.Open && Status != BO.CallStatus.OpenAtRisk)
                throw new BO.BlAlreadyExistsException("The call is already being processed. ");

            if (volunteer.MaxDistance < Tools.GetDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude, call.Longitude, volunteer.DistanceType))
                throw new BO.BlAttemptToPerformAnIllegalActionException("The distance is too far. ");

            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(new DO.Assignment
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    EntryTime = AdminManager.Now,
                });

            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(callId);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("The data does not exist. ", ex);
        }
    }

    /// <summary>
    /// A function that checks how many calls there are in each status and arranges them in an array
    /// </summary>
    /// <returns>An array with a number of calls of each status</returns>
    /// <exception cref="Exception"></exception>
    public int[] CountCalls()
    {
        try
        {
            IEnumerable<BO.Call> calls;
            lock (AdminManager.BlMutex)
                calls = _dal.Call.ReadAll().Select(c => CallManager.ConversionDoTOBo(c));
            var grouped = calls
            .GroupBy(call => (int)call.StatusType)
            .ToDictionary(group => group.Key, group => group.Count());

            int[] result = new int[Enum.GetValues(typeof(BO.CallStatus)).Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = grouped.ContainsKey(i) ? grouped[i] : 0;
            }
            return result;

        }
        catch (Exception ex)
        {
            throw new Exception("An exception in the function of the number of calls by type.");
        }
    }

    /// <summary>
    /// A function to delete a call based on an ID 
    /// </summary>
    /// <param name="id"> Call id</param>
    /// <exception cref="BO.BlDeletionImpossible">Exception if cannot be deleted</exception>
    /// <exception cref="BO.BlDoesNotExistException">Exception if no such call exists</exception>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            lock (AdminManager.BlMutex)
                if (_dal.Assignment.Read(c => c.CallId == id) == null && Tools.GetCallStatus(id) == BO.CallStatus.Open)
                    _dal.Call.Delete(id);
                else
                    throw new BO.BlDeletionImpossible("Deletion is impossible");

            CallManager.Observers.NotifyListUpdated();
            CallManager.Observers.NotifyItemUpdated(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
    }

    /// <summary>
    /// A function that returns a list of calls according to a filter
    /// </summary>
    /// <param name="filter">the selected filter</param>
    /// <param name="ob">The variable selected for filtering</param>
    /// <param name="sort">The variable selected for sorting</param>
    /// <returns>The function returns a collection of the calls according to the user's request</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public IEnumerable<BO.CallInList> GetCalls(BO.TypeOfFiltering? filter, object? ob, BO.TypeOfSortingCall? sort)
    {
        try
        {

            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex)
                calls = _dal.Call.ReadAll();

            calls = filter switch
            {
                BO.TypeOfFiltering.Type when ob is BO.CallType typeFilter =>
                calls.Where(call => call.Type == (DO.CallType)typeFilter),

                BO.TypeOfFiltering.Status when ob is BO.CallStatus statusFilter =>
                calls.Where(call => Tools.GetCallStatus(call.Id) == statusFilter),

                _ => calls
            };


            IEnumerable<BO.CallInList> callInList;
            lock (AdminManager.BlMutex)
                callInList = calls.Select(c => CallManager.ConversionDoTOCallInList(_dal.Call.Read(c.Id)!));

            //Sorting the list according to the user's request
            return sort switch
            {
                BO.TypeOfSortingCall.AssignmateId => callInList.OrderBy(c => c.AssignmateId),
                BO.TypeOfSortingCall.OpeningTime => callInList.OrderBy(c => c.OpeningTime),
                BO.TypeOfSortingCall.NumberOfAssignmates => callInList.OrderBy(c => c.NumberOfAssignmates),
                BO.TypeOfSortingCall.TotalTreatmentTime => callInList.OrderBy(c => c.TotalTreatmentTime),
                _ => callInList.OrderBy(c => c.CallId)
            };

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
    }

    /// <summary>
    /// A function to get all closed calls according to a filter
    /// </summary>
    /// <param name="volunteerId">Call for a specific volunteer</param>
    /// <param name="filter">Filter for filtering</param>
    /// <param name="sort">Filter for sorting</param>
    /// <returns>The function returns a list of closed calls</returns>
    /// <exception cref="BO.BlDoesNotExistException">Exception when no list of calls was found</exception>
    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volunteerId, BO.CallType? filter, BO.TypeOfSortingClosedCalls? sort)
    {
        try
        {

            lock (AdminManager.BlMutex)
                if (_dal.Volunteer.Read(volunteerId) == null)
                    throw new BO.BlDoesNotExistException("No such volunteer was found or no closed calls. ");
           
            lock (AdminManager.BlMutex)
                if (!_dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType != null).Any())
                    throw new BO.BlDoesNotExistException("The volunteer does'nt have any closed calls. ");

            IEnumerable<BO.ClosedCallInList> closedCalls;
            List<DO.Assignment> assignments;

            lock (AdminManager.BlMutex)
                assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.TheEndType != null).ToList();

            lock (AdminManager.BlMutex)
                closedCalls = from assignment in assignments
                              let entryTime = assignment.EntryTime
                              let exitTime = assignment.ExitTime
                              let call = _dal.Call.Read(assignment.CallId)
                              where filter == null || call.Type == (DO.CallType)filter
                              select new BO.ClosedCallInList
                              {
                                  CallId = call.Id,
                                  Type = (BO.CallType)call.Type,
                                  FullCallAddress = call.FullCallAddress,
                                  OpeningTime = call.OpeningTime,
                                  EnteringTime = entryTime,
                                  EndOfTime = exitTime,
                                  Status = (BO.AssignmentStatus)assignment.TheEndType!
                              };

            //Sorting the list according to the user's request
            return sort switch
            {
                BO.TypeOfSortingClosedCalls.OpeningTime => closedCalls.OrderBy(c => c.OpeningTime),
                BO.TypeOfSortingClosedCalls.EnteringTime => closedCalls.OrderBy(c => c.EnteringTime),
                BO.TypeOfSortingClosedCalls.EndOfTime => closedCalls.OrderBy(c => c.EndOfTime),
                _ => closedCalls.OrderBy(c => c.CallId)
            };

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No list of call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }
    }

    /// <summary>
    /// A function that returns open calls
    /// </summary>
    /// <param name="id">Call for a specific volunteer</param>
    /// <param name="filter">Filter for filtering</param>
    /// <param name="sort">Filter for sorting</param>
    /// <returns>returns open calls</returns>   
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public IEnumerable<BO.OpenCallInList> GetOpenCalls(int id, BO.CallType? filter, BO.TypeOfSortingOpenCalls? sort)
    {
        try
        {

            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(volunteer => volunteer.Id == id && volunteer.Active == true) ?? throw new BO.BlDoesNotExistException("The volunteer does not exist or is inactive. ");

            List<DO.Call> calls;
            lock (AdminManager.BlMutex)
                calls = _dal.Call.ReadAll(call => Tools.GetCallStatus(call.Id) == BO.CallStatus.Open || Tools.GetCallStatus(call.Id) == BO.CallStatus.OpenAtRisk).ToList();

            IEnumerable<BO.OpenCallInList> opendCalls =
            (from call in calls
            let DistanceFromVolunteerToCall = Tools.GetDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude, call.Longitude, volunteer.DistanceType)
            where filter == null || call.Type == (DO.CallType)filter
            where volunteer.MaxDistance > DistanceFromVolunteerToCall
            select new BO.OpenCallInList
            {
                CallId = call.Id,
                Type = (BO.CallType)call.Type,
                VerbalDescription = call.VerbalDescription,
                Address = call.FullCallAddress,
                OpeningTime = call.OpeningTime,
                MaxTime = call.MaxTimeToFinish,
                DistanceFromVolunteer = DistanceFromVolunteerToCall
            }).ToList();

            //Sorting the list according to the user's request
            return sort switch
            {
                BO.TypeOfSortingOpenCalls.OpeningTime => opendCalls.OrderBy(c => c.OpeningTime),
                BO.TypeOfSortingOpenCalls.DistanceFromVolunteer => opendCalls.OrderBy(c => c.DistanceFromVolunteer),
                _ => opendCalls.OrderBy(c => c.CallId)
            };

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }

    }
    /// <summary>
    /// A function to read a call from the data layer
    /// </summary>
    /// <param name="callId">id to select the desired qira</param>
    /// <returns> Returns the desired call </returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Call Read(int callId)
    {
        try
        {

            {
                //Checking if such a call exists
                DO.Call call;
                lock (AdminManager.BlMutex)
                    call = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"The call with the ID {callId} does not exist.");
                //Converts and returns
                return CallManager.ConversionDoTOBo(call);
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("No call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }

    }

    /// <summary>
    /// A function to update a calling object
    /// </summary>
    /// <param name="call"> An object of type call</param>
    /// <exception cref="BO.BlDoesNotExistException"> Exception if no call with the same ID number is found</exception>
    /// <exception cref="BO.BlXMLFileLoadCreateException">Exception if there was a problem in the xml layer </exception>
    public void Update(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Call docall;
        try
        {
            //Checking if such an object exists
            lock (AdminManager.BlMutex)
                if (Read(call.Id) == null)
                    throw new BO.BlDoesNotExistException("No call with this id found.");

            //Validity and format check
            CallManager.ValidateCall(call);

            //Creating a new object of type DO.Call
            docall = CallManager.ConversionBoTODo(call);

            lock (AdminManager.BlMutex)
                _dal.Call.Update(docall);



            CallManager.Observers.NotifyListUpdated();
            //CallManager.Observers.NotifyItemUpdated(call.Id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }
        DO.Call newDocall;
        lock (AdminManager.BlMutex)
            newDocall = _dal.Call.Read(call.Id)!;
        _ = CallManager.UpdateCoordinatesForCallAddressAsync(newDocall);
    }

    /// <summary>
    /// Function to update deassignment
    /// </summary>
    /// <param name="volunteerId">Identification number of the cancellation requester</param>
    /// <param name="assignmentId">Identification number of the allocation</param>
    /// <exception cref="BO.BlDoesNotExistException">Exception if no volunteer or assignment with the given identification number is found</exception>
    /// <exception cref="BO.BlAttemptToPerformAnIllegalActionException">Exception if cannot be deallocated</exception>
    /// <exception cref="BO.BlXMLFileLoadCreateException"></exception>
    public void UpdateCancellationOfTreatment(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            int tempCallId;
            int tempVolunteerId;

            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException("The volunteer does not exist. ");

            //Check from the end because maybe the volunteer took the same call after canceling
            DO.Assignment assignment;
            lock (AdminManager.BlMutex)
                assignment = _dal.Assignment.Read(a => a.Id == assignmentId && (a.VolunteerId == volunteerId || volunteer.RoleType == DO.Role.Management)) ?? throw new BO.BlDoesNotExistException("The assignment does not exist. ");

            if (volunteer.Id != volunteerId && volunteer.RoleType != DO.Role.Management)
                throw new BO.BlAttemptToPerformAnIllegalActionException("You are not allowed to perform the action. ");

            if (assignment.TheEndType != null || assignment.ExitTime != null)
                throw new BO.BlAttemptToPerformAnIllegalActionException("The assignment is already closed. ");
            DO.Assignment newAssignment = new DO.Assignment
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                VolunteerId = assignment.VolunteerId,
                EntryTime = assignment.EntryTime,
                ExitTime = AdminManager.Now,
                TheEndType = volunteer.RoleType == DO.Role.Management ? DO.TypeOfEnding.CancelingAnAdministrator : DO.TypeOfEnding.SelfCancellation
            };

            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(newAssignment);

            CallManager.Observers.NotifyItemUpdated(assignment.CallId);
            VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }
    }
    /// <summary>
    /// A function to update the end of treatment
    /// </summary>
    /// <param name="volunteerId">id of the person requesting the request to end treatment</param>
    /// <param name="assignmentId">id of the assignment to update treatment completion</param>
    /// <exception cref="BO.BlDoesNotExistException">Exception if no volunteer or assignment with the given identification number is found</exception>
    /// <exception cref="BO.BlAlreadyExistsException">Exception if the assignment already exists and is closed</exception>
    /// <exception cref="BO.BlXMLFileLoadCreateException"></exception>
    public void UpdateEndOfTreatment(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            int tempCallId;


            //Check from the end because maybe the volunteer took the same call after canceling
            DO.Assignment temp;
            lock (AdminManager.BlMutex)
                temp = _dal.Assignment.Read(a => a.Id == assignmentId && a.VolunteerId == volunteerId) ?? throw new BO.BlDoesNotExistException("No such assignment exists. ");

            if (temp.TheEndType != null && temp.ExitTime != null)
                throw new BO.BlAlreadyExistsException("The assignment already exists and is closed. ");

            DO.Assignment assignment = new DO.Assignment
            {
                Id = temp.Id,
                CallId = temp.CallId,
                VolunteerId = temp.VolunteerId,
                EntryTime = temp.EntryTime,
                ExitTime = AdminManager.Now,
                TheEndType = DO.TypeOfEnding.Treated
            };
            //Update to the data layer
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(assignment);


            CallManager.Observers.NotifyItemUpdated(temp.CallId);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"No call found.", ex);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);

        }

    }

}
