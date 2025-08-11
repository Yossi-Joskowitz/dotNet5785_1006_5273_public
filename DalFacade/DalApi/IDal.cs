namespace DalApi;
/// <summary>
/// An interface that contains 4 objects of the other interface types, and an reset for all of them.
/// </summary>
public interface IDal
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IConfig Config { get; }
    void ResetDB();

}
