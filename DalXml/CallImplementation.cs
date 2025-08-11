namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the interface
/// </summary>
internal class CallImplementation : ICall
{   /// <summary>
    /// Implementation of creating a call type object and inserting it into an xml box
    /// </summary>
    /// <param name="item">The resulting object to insert into the xml file</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int newId = Config.NextCallId;
        Call temp = item with { Id = newId };
        calls.Add(temp);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deleting a member from the xml file by id
    /// </summary>
    /// <param name="id">id for finding a member to delete</param>
    /// <exception cref="DalDoesNotExistException"> Throwing an exception when the object is not found</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Read(id) is null)
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {id} does not exist");
        calls.Remove(Read(id));
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deleting all objects from the xml file
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }
    /// <summary>
    /// Determining an object by ID
    /// </summary>
    /// <param name="id">id for finding an object and reading it</param>
    /// <returns>The function returns from the xml file the object we were looking for</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(c => c.Id == id);
    }
    /// <summary>
    /// The function reads and returns an element from the xml file by receiving a pointer to a certain filter function
    /// </summary>
    /// <param name="filter">The resulting filter function</param>
    /// <returns>The function returns the member we were looking for</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// A function that reads and returns all objects that meet the resulting filter function
    /// </summary>
    /// <param name="filter">filter function</param>
    /// <returns> Returning a list with the objects that met the requirements </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (filter is not null)
        {
            return calls.Where(filter);
        }
        return calls;
    }

    /// <summary>
    /// Update the object in the xml file
    /// </summary>
    /// <param name="item">The object after the update</param>
    /// <exception cref="DalDoesNotExistException">Throwing an exception when the file is not found</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Read(item.Id) is not null)
        {
            calls.Remove(Read(item.Id));
            calls.Add(item);
            XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {item.Id} does not exist");
        }
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }
}
