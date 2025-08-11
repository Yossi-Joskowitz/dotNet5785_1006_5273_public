namespace BlImplementation;
using BlApi;

using Helpers;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// The observer manager for the volunteer.    
    /// </summary>
    public void AddObserver(Action listObserver) =>
        VolunteerManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        VolunteerManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        VolunteerManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        VolunteerManager.Observers.RemoveObserver(id, observer);



    /// <summary>
    /// Adds a new volunteer to the system.
    /// Validates format and logical constraints on the volunteer's details.
    /// Throws an exception if a volunteer with the same ID already exists.
    /// </summary>
    /// <param name="volunteer">The new volunteer object to add.</param>
    public void Add(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            VolunteerManager.IntegrityCheck(boVolunteer);

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(VolunteerManager.ConvertFromBOVolunteerToDOVolunteer(boVolunteer));

            VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.Id);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BO.BlDoesNotExistException("The Data base dose not exist.", e);
        }
        catch (DO.DalAlreadyExistsException e)
        {
            throw new BO.BlAlreadyExistsException("The volunteer already exists.", e);
        }
        lock (AdminManager.BlMutex)
            _ = VolunteerManager.UpdateCoordinatesForVolunteerAddressAsync(_dal.Volunteer.Read(boVolunteer.Id)!);
    }



    /// <summary>
    /// Deletes a volunteer from the system.
    /// Can only delete volunteers who are not currently handling or have never handled a call.
    /// Throws an exception if the deletion is not allowed or the volunteer does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {

            lock (AdminManager.BlMutex)
                if (_dal.Volunteer.Read(id) == null)
                    throw new BO.BlDoesNotExistException("The volunterr dose not exist.");

            lock (AdminManager.BlMutex)
                if ((BO.Role)_dal.Volunteer.Read(id)!.RoleType == BO.Role.Management && _dal.Volunteer.ReadAll(v => (BO.Role)v.RoleType == BO.Role.Management).Count() == 1)
                    throw new BO.BlAttemptToPerformAnIllegalActionException("There must be at least one administrator in the system.");

            lock (AdminManager.BlMutex)
                if (_dal.Assignment.ReadAll(a => a.VolunteerId == id).Any())
                    throw new BO.BlAttemptToPerformAnIllegalActionException("You cannot delete a volunteer who has been assigned to a call.");

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Delete(id);

            VolunteerManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(id);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BO.BlDoesNotExistException("There is no volunteer with this ID.", e);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }
    }


    /// <summary>
    /// Returns the role of the user based on ID and password.
    /// Throws an exception if the user does not exist or if the password is incorrect.
    /// </summary>
    /// <param name="name">The user's ID.</param>
    /// <param name="pass">The user's password.</param>
    /// <returns>The role of the user as a BO.Role.</returns>
    public BO.Role LoginReturnType(string name, string pass)
    {
        try
        {
            lock (AdminManager.BlMutex)
                return (BO.Role)(_dal.Volunteer.Read(v => v.Name == name && v.Password == pass) ?? throw new BO.BlDoesNotExistException("The user does not exist or the password is incorrect.")).RoleType;
        }
        catch (DO.DalXMLFileLoadCreateException DoException)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", DoException);
        }
    }




    /// <summary>
    /// Retrieves detailed information about a volunteer.
    /// Includes details about the volunteer and their current call, if applicable.
    /// Throws an exception if the volunteer ID does not exist.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>A BO.Volunteer object with full details.</returns>
    public BO.Volunteer Read(int id)
    {
        try
        {
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(id) ?? null;

            if (volunteer == null)
                throw new BO.BlDoesNotExistException("The volunteer does not exist.");
            return VolunteerManager.ConvertFromDOVolunteerToBOVolunteer(volunteer);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BO.BlDoesNotExistException("The Data base dose not exist.", e);
        }
    }


    /// <summary>
    /// Retrieves a filtered and sorted list of volunteers.
    /// - If 'active' is null, returns the full list.
    /// - If 'sorting' is null, sorts by default (ID).
    /// </summary>
    /// <param name="active">Filter volunteers by active status (nullable).</param>
    /// <param name="sorting">Sort volunteers by specified field (nullable).</param>
    /// <returns>A list of volunteers (BO.VolunteerInList).</returns>
    public IEnumerable<BO.VolunteerInList> ReadVolunteers(bool? active, BO.TypeOfSortingVolunteer? sorting)
    {
        try
        {
            IEnumerable<BO.VolunteerInList> volunteersInList;
            lock (AdminManager.BlMutex)
                volunteersInList = _dal.Volunteer.ReadAll(Volunteer => active == null || Volunteer.Active == active).
                    Select(volunteer => VolunteerManager.ConvertFronDOVolunteerToBOVolunteerInList(volunteer));

            return VolunteerManager.SortList(volunteersInList, sorting);
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BO.BlDoesNotExistException("The Data base dose not exist.", e);
        }
    }



    /// <summary>
    /// Updates volunteer details.
    /// Validates that the requester is either the volunteer themselves or an admin.
    /// Performs format and logical validation before updating.
    /// Throws an exception if the volunteer ID does not exist.
    /// </summary>
    /// <param name="id">The ID of the person requesting the update.</param>
    /// <param name="volunteer">The updated volunteer object.</param>
    public void Update(int id, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            DO.Volunteer? doVolunteer;
            lock (AdminManager.BlMutex)
                doVolunteer = _dal.Volunteer.Read(boVolunteer.Id) ?? null;
            if (doVolunteer == null)
                throw new BO.BlDoesNotExistException("There is no volunteer with this ID.");

            if (!VolunteerManager.CanUpdate(id, boVolunteer.Id))
                throw new BO.BlAttemptToPerformAnIllegalActionException("Only the administrator or the volunteer themselves will be able to update ");

            lock (AdminManager.BlMutex)
                if (boVolunteer.Active == false && _dal.Assignment.Read(a => a.VolunteerId == boVolunteer.Id && a.ExitTime == null) != null)
                    throw new BO.BlAttemptToPerformAnIllegalActionException("Cannot update inactive as long as there is a call in progress.");

            lock (AdminManager.BlMutex)
                if ((BO.Role)doVolunteer.RoleType != boVolunteer.RoleType && boVolunteer.RoleType == BO.Role.Volunteer
                        && _dal.Volunteer.ReadAll(v => (BO.Role)v.RoleType == BO.Role.Management).Count() == 1)
                    throw new BO.BlAttemptToPerformAnIllegalActionException("There must be at least one administrator in the system.");

            lock (AdminManager.BlMutex)
                if ((BO.Role)doVolunteer.RoleType != boVolunteer.RoleType && (BO.Role)_dal.Volunteer.Read(id)!.RoleType == BO.Role.Volunteer)
                    throw new BO.BlAttemptToPerformAnIllegalActionException("Only the administrator can update the role of the volunteer.");

            VolunteerManager.IntegrityCheck(boVolunteer);

            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(VolunteerManager.ConvertFromBOVolunteerToDOVolunteer(boVolunteer));

            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalXMLFileLoadCreateException e)
        {
            throw new BO.BlXMLFileLoadCreateException("The XML file is not found or there is a problem opening it.", e);
        }
        catch (DO.DalDoesNotExistException e)
        {
            throw new BO.BlDoesNotExistException("The Data base dose not exist.", e);
        }

        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
            volunteer = _dal.Volunteer.Read(boVolunteer.Id)!;
        _ = VolunteerManager.UpdateCoordinatesForVolunteerAddressAsync(volunteer);

    }
}
