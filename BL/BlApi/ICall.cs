using BO;

namespace BlApi;

/// <summary>
/// Interface for managing calls in the system.
/// </summary>
public interface ICall :IObservable 
{

    /// <summary>
    /// Gets the count of calls by their statuses.
    /// </summary>
    /// <returns>An array where each index represents the count of calls with a corresponding status.</returns>
    public int[] CountCalls();
    /// <summary>
    /// Retrieves a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filter">The field to filter by (nullable).</param>
    /// <param name="ob">The value to filter by (nullable).</param>
    /// <param name="sort">The field to sort by (nullable).</param>
    /// <returns>A filtered and sorted enumerable of calls.</returns>
    public IEnumerable<BO.CallInList> GetCalls(TypeOfFiltering? filter, object? ob, TypeOfSortingCall? sort);
    /// <summary>
    /// Reads the details of a specific call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call.</param>
    /// <returns>The details of the call.</returns>
    public BO.Call Read(int id);
    /// <summary>
    /// Updates the details of a specific call.
    /// </summary>   
    /// <param name="call">The updated call details.</param>
    public void Update(BO.Call call);
    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    public void Delete(int id);
    /// <summary>
    /// Adds a new call to the system.
    /// </summary>
    /// <param name="call">The call to add.</param>
    public void Add(BO.Call call);
    /// <summary>
    /// Retrieves a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="filter">The type of call to filter by (nullable).</param>
    /// <param name="sort">The field to sort by (nullable).</param>
    /// <returns>A sorted and filtered list of closed calls.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int id, CallType? filter, TypeOfSortingClosedCalls? sort);
    /// <summary>
    /// Retrieves a list of open calls available for a volunteer to choose from.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="filter">The type of call to filter by (nullable).</param>
    /// <param name="sort">The field to sort by (nullable).</param>
    /// <returns>A sorted and filtered list of open calls.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCalls(int id, CallType? filter, TypeOfSortingOpenCalls? sort);
    /// <summary>
    /// Marks a call as treated by a specific volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the call to mark as treated.</param>
    public void UpdateEndOfTreatment(int id, int assignmentId);
    /// <summary>
    /// Cancels the treatment of a specific call.
    /// </summary>
    /// <param name="id">The ID of the volunteer or manager.</param>
    /// <param name="assignmentId">The ID of the call to cancel.</param>
    public void UpdateCancellationOfTreatment(int id, int assignmentId);
    /// <summary>
    /// Assigns a call to a volunteer for treatment.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="callId">The ID of the call to assign.</param>
    public void ChoosingACallForTreatment(int id, int callId);



}
