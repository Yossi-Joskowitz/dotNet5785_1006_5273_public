namespace BlImplementation; 
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

internal class AdminImplementation : IAdmin
{

    /// <summary>
    /// Add a clock observer to monitor clock updates.
    /// </summary>
    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;

    /// <summary>
    /// Remove a clock observer.
    /// </summary>
    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;

    /// <summary>
    /// Add a configuration observer to monitor config updates.
    /// </summary>
    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;

    /// <summary>
    /// Remove a configuration observer.
    /// </summary>
    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;

    // Reference to the DAL (Data Access Layer) implementation.
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Initializes the admin system by resetting the database and updating the clock.
    /// </summary>
    public void AdminInitialize()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7 
        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// Resets the admin system and database.
    /// </summary>
    public void AdminReset()
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            AdminManager.ResetDB(); //stage 7                    
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (BO.BLTemporaryNotAvailableException ex)
        {
            throw new BO.BLTemporaryNotAvailableException(ex.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception("The reset failed. ", ex);
        }
        
    }

    /// <summary>
    /// Advances the clock by the specified time unit.
    /// </summary>
    /// <param name="type">The unit of time to advance.</param>
    public void AdvanceTheClock(TypeOfTime type)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        switch (type) {
            case TypeOfTime.Minute:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;
            case TypeOfTime.Hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case TypeOfTime.Day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case TypeOfTime.Month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case TypeOfTime.Year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;
            default:
                throw new BO.BlAttemptToPerformAnIllegalActionException("No promotion type selected.");
        }
       
    }

    // <summary>
    /// Gets the current clock time.
    /// </summary>
    /// <returns>Current clock time as a DateTime object.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Clock;
    }

    /// <summary>
    /// Gets the risk time range.
    /// </summary>
    /// <returns>Risk range as a TimeSpan.</returns>
    public TimeSpan GetRiskTime()
    {
        return AdminManager.RiskRange;
    }

    /// <summary>
    /// Sets the risk time range.
    /// </summary>
    /// <param name="time">The new risk time range.</param
    public void SetRiskTime(TimeSpan time)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.RiskRange = time;
        CallManager.Observers.NotifyListUpdated();
        foreach (var call in _dal.Call.ReadAll())
        {
            CallManager.Observers.NotifyItemUpdated(call.Id);
        }
        foreach (var volunteer in _dal.Volunteer.ReadAll())
        {
            VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
        }
    }


    /// <summary>
    /// Initializing the simulator
    /// </summary>
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    /// <summary>
    /// stops the simulator
    /// </summary>
    public void StopSimulator()
            => AdminManager.Stop(); //stage 7

    public void IsSimulatorIsRonning()
        => AdminManager.IsSimulationRunning();


    //The function is only needed for tests of bl and dal
    /// <summary>
    /// Reads and displays all data from the database (Volunteers, Calls, and Assignments).
    /// </summary>
    public void ReadAll()
    {
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
        foreach (DO.Volunteer volunteer in volunteers)
        {
            Console.WriteLine(volunteer);
        }

        IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
        foreach (var call in calls)
        {
            Console.WriteLine(call);
        }

        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
        foreach (var assignment in assignments)
        {
            Console.WriteLine(assignment);
        }

    }
}
