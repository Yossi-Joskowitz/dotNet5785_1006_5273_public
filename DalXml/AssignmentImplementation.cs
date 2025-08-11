namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the interface
/// </summary>
internal class AssignmentImplementation : IAssignment
{ 
    /// <summary>
    /// Implementation of creating a task type object and inserting it into an xml box
    /// </summary>
    /// <param name="item"> A variable of type assignment </param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        int newId = Config.NextAssignmentId;
        Assignment temp = item with { Id = newId };
        assignments.Add(temp);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deleting an object by id
    /// </summary>
    /// <param name="id">Receiving an id for searching and deleting an object</param>
    /// <exception cref="DalDoesNotExistException">Throwing an exception when the object is not found </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Read(id) is null)
            throw new DalDoesNotExistException($"An object of type volunteer with ID = {id} does not exist");
        assignments.Remove(Read(id));
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deleting all details from the xml file
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reading one of the objects from the xml file according to a certain id
    /// </summary>
    /// <param name="id">id of the object we want to read</param>
    /// <returns> The function will return us the requested object from the xml file </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(a => a.Id == id);
    }

    /// <summary>
    /// A function to call an object by a pointer to a function when the pointer can receive a different function each time
    /// </summary>
    /// <param name="filter">the desired filter</param>
    /// <returns>Returning the first object that met the conditions of the received function</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// A function that receives a pointer to a function to select all objects that meet the desired parameters
    /// In case the filter is empty, the function returns the entire list
    /// </summary>
    /// <param name="filter">The function with the desired filter</param>
    /// <returns> The function returns a list with all the objects that met the requirements</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (filter is not null)
        {
            return assignments.Where(filter);
        }
        return assignments;
    }

    /// <summary>
    /// Updating the object in the xml file
    /// </summary>
    /// <param name="item">The object after the update</param>
    /// <exception cref="DalDoesNotExistException"> Throwing an exception when we did not find the desired object </exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Read(item.Id) is not null)
        {
            assignments.Remove(Read(item.Id));
            assignments.Add(item);
            XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"An object of type assignment with ID = {item.Id} does not exist");
        }
    }
}
