namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the volunteer's interface.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creting a new volunteer in the list.
    /// </summary>
    /// <param name="item">The volunteer to enter.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a volunteer with the specified ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"An object of type volunteer with ID = {item.Id} already exists");
        DataSource.Volunteers.Add(item);

    }
    /// <summary>
    /// Deleting a volunteer from the list
    /// </summary>
    /// <param name="id">The Id of the volunteer to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) is  null)
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {id} does not exist");
        DataSource.Volunteers.Remove(Read(id));
    }
    /// <summary>
    /// Deleting all the volunteers from the list.
    /// </summary>
    /// <exception cref="DalDoesNotExistException">Thrown when the list of volunteers is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Volunteers is  null)
            throw new DalDoesNotExistException("There is no such list");
        DataSource.Volunteers.Clear();
        ;
    }
    /// <summary>
    /// Reading a volunteer from the list
    /// </summary>
    /// <param name="id">The Id of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown when the list of volunteers is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        if (DataSource.Volunteers is not null)
            return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        throw new DalDoesNotExistException("The list is does not exis.");
    }


    /// <summary>
    /// The function accepts a condition to be tested and returns a pointer to the first 
    /// member that fulfills the condition, and if not found, returns null
    /// </summary>
    /// <param name="filter">Function that gets  volunteer and returns  bool</param>
    /// <returns>A volunteer for whom the filter returned true, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        if (DataSource.Volunteers is not null)
            return DataSource.Volunteers.FirstOrDefault(filter);
        throw new DalDoesNotExistException("The list is does not exis.");
    }

    /// <summary>
    /// reading volunteers from the list.
    /// </summary>
    /// <returns>A list of volunteers for whom the filter returns true
    /// and if it is null then all volunteers</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
       => filter == null
           ? from item in DataSource.Volunteers
             select item
           : from item in DataSource.Volunteers
             where filter(item)
             select item;
    /// <summary>
    /// updating a volunteer in the list
    /// </summary>
    /// <param name="item">The update volunteer.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        if (Read(item.Id)is not null) 
        {
            DataSource.Volunteers.Remove(Read(item.Id));
            DataSource.Volunteers.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {item.Id} does not exist");
        }
    }
}
