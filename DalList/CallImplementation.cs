namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the call's interface.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// Creting a new call in the list.
    /// </summary>
    /// <param name="item">The call to enter in the list.</param>
    /// <returns>The ID of the newly created call.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call temp = item with { Id = newId };
        DataSource.Calls.Add(temp);
       
    }
    /// <summary>
    /// Deleting a call from the list.
    /// </summary>
    /// <param name="id">The id for the call to delete from the list.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a call with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        if (Read(id) is  null)
            throw new DalDoesNotExistException($"An object of type call with ID = {id} does not exist");
        DataSource.Calls.Remove(Read(id));
    }
    /// <summary>
    /// Deleting all the calls from the list.
    /// </summary>
    /// <exception cref="DalDoesNotExistException">Thrown when the list is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        if (DataSource.Calls is  null)
            throw new DalDoesNotExistException("There is no such list");
        DataSource.Calls.Clear();
        
    }
    /// <summary>
    /// Reading a call from the list.
    /// </summary>
    /// <param name="id">The id for the call to read from the list.</param>
    /// <returns>The call with the same id,, or null if not found..</returns>
    /// <exception cref="DalDoesNotExistException">Thrown when the list of calls is null.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        if (DataSource.Calls is not null)
            return DataSource.Calls.FirstOrDefault(c => c.Id == id);
        throw new DalDoesNotExistException("The list is not exist");
    }


    /// <summary>
    /// The function accepts a condition to be tested and returns a pointer to the first 
    /// member that fulfills the condition, and if not found, returns null
    /// </summary>
    /// <param name="filter"> The resulting filter</param>
    /// <returns> An object that meets the required filtering will be returned</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// A function that returns a list of all calls that meet the resulting filter function.
    /// </summary>
    /// <returns>A list of all assignments or only those who met the conditions.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    
        => filter == null
            ? from item in DataSource.Calls
              select item
            : from item in DataSource.Calls
              where filter(item)
              select item;



    /// <summary>
    /// Updating a call in the list.
    /// </summary>
    /// <param name="item">The id for the call to update in the list.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the specified call does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        if (Read(item.Id) is not null)
        {
            DataSource.Calls.Remove(Read(item.Id));
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {item.Id} does not exist");
        }
    }
}
