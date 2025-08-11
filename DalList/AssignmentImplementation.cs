namespace Dal;

using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the assignment's interface.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creating a new assignment in the list.
    /// </summary>
    /// <param name="item">The assignment to enter in the list.</param>    
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment temp = item with { Id = newId };
        DataSource.Assignments.Add(temp);    
    }
    /// <summary>
    /// Deleting an assignment from the list.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) is null)
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {id} does not exist");
        DataSource.Assignments.Remove(Read(id));
    }
    /// <summary>
    /// Deleting all the assignments from the list.
    /// </summary>
    /// <exception cref="DalDoesNotExistException">Thrown when the list of assignments is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Assignments is null)
            throw new DalDoesNotExistException("There is no such list");
        DataSource.Assignments.Clear();

    }
    /// <summary>
    /// Reading an assignment from the list.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID, or null if not found.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown when the list of assignments is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        if (DataSource.Assignments is not null)
            return DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        throw new DalDoesNotExistException("The list is null");
    }

    /// <summary>
    /// The function accepts a condition to be tested and returns a pointer to the first 
    /// member that fulfills the condition, and if not found, returns null 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }


    /// <summary>
    /// A function that returns a list of all assignments that meet the resulting filter function.
    /// </summary>
    /// <returns>A list of all assignments or only those who met the conditions</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        => filter == null
            ? from item in DataSource.Assignments
              select item
            : from item in DataSource.Assignments
              where filter(item)
              select item;



    /// <summary>
    /// Updating an assignment in the list.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when an assignment with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        if (Read(item.Id) is not null)
        {
            DataSource.Assignments.Remove(Read(item.Id));
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {item.Id} does not exist");
        }
    }
}
