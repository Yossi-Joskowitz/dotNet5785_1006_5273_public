using BO;

namespace BlApi;
/// <summary>
/// Interface for managing volunteers in the system.
/// </summary>
public interface IVolunteer: IObservable 
{
    /// <summary>
    /// Returns the role of the user based on ID and password.
    /// Throws an exception if the user does not exist or if the password is incorrect.
    /// </summary>
    /// <param name="name">The user's ID.</param>
    /// <param name="pass">The user's password.</param>
    /// <returns>The role of the user as a BO.Role.</returns>
    public BO.Role LoginReturnType(string name, string pass);


    /// <summary>
    /// Retrieves a filtered and sorted list of volunteers.
    /// - If 'active' is null, returns the full list.
    /// - If 'sorting' is null, sorts by default (ID).
    /// </summary>
    /// <param name="active">Filter volunteers by active status (nullable).</param>
    /// <param name="sorting">Sort volunteers by specified field (nullable).</param>
    /// <returns>A list of volunteers (BO.VolunteerInList).</returns>
    public IEnumerable<BO.VolunteerInList> ReadVolunteers(bool ? active , TypeOfSortingVolunteer? sorting );

    /// <summary>
    /// Retrieves detailed information about a volunteer.
    /// Includes details about the volunteer and their current call, if applicable.
    /// Throws an exception if the volunteer ID does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>A BO.Volunteer object with full details.</returns>
    public BO.Volunteer Read(int id);

    /// <summary>
    /// Updates volunteer details.
    /// Validates that the requester is either the volunteer themselves or an admin.
    /// Performs format and logical validation before updating.
    /// Throws an exception if the volunteer ID does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer to update.</param>
    /// <param name="volunteer">The updated volunteer object.</param>
    public void Update(int id,BO.Volunteer volunteer);


    /// <summary>
    /// Deletes a volunteer from the system.
    /// Can only delete volunteers who are not currently handling or have never handled a call.
    /// Throws an exception if the deletion is not allowed or the volunteer does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    public void Delete(int id);



    /// <summary>
    /// Adds a new volunteer to the system.
    /// Validates format and logical constraints on the volunteer's details.
    /// Throws an exception if a volunteer with the same ID already exists.
    /// </summary>
    /// <param name="volunteer">The new volunteer object to add.</param>
    public void Add(BO.Volunteer volunteer);

}
