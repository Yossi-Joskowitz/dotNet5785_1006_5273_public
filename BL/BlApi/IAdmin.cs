using BO;

namespace BlApi;
/// <summary>
/// Interface defining administrative actions and configurations.
/// </summary>
public interface IAdmin
{

    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);



    /// <summary>
    /// Retrieves the current system clock value.
    /// </summary>
    /// <returns>The current DateTime of the system clock.</returns>
    public DateTime GetClock();

    /// <summary>
    /// Advances the system clock by a specified time unit.
    /// </summary>
    /// <param name="tpe">The Type of time to advance.</param>
    public void AdvanceTheClock(TypeOfTime type);

    /// <summary>
    /// Retrieves the configured risk time span.
    /// </summary>
    /// <returns>The TimeSpan representing the risk duration.</returns>
    public TimeSpan GetRiskTime();


    /// <summary>
    /// Sets the risk time span to a specified value.
    /// </summary>
    /// <param name="time">The TimeSpan value to set for the risk duration.</param>
    public void SetRiskTime(TimeSpan time);

    /// <summary>
    /// Resets the database to its initial state, clearing all data and configurations.
    /// </summary>
    public void AdminReset();

    /// <summary>
    /// Initializes the database with default configuration and initial data.
    /// </summary>
    public void AdminInitialize();

    public void ReadAll();

    void StartSimulator(int interval); //stage 7

    void StopSimulator(); //stage 7

    /// <summary>
    /// A function to check if the simulatro is stiil ronning.
    /// </summary>
    void IsSimulatorIsRonning();  
}
