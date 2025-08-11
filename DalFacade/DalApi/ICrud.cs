using DO;

namespace DalApi;

public interface ICrud<T> where T : class
                            
{
    /// <summary>
    /// Creates a new entity of general type
    /// </summary>
    /// <param name="item">item of some kind</param>
    void Create(T item);

    /// <summary>
    /// The function checks by ID whether a certain object is found and if so returns it
    /// </summary>
    /// <param name="id">ID of an object</param>
    /// <returns>The function returns the object</returns>
    T? Read(int id);

    /// <summary>
    /// The function accepts a condition to be tested and returns a pointer to the first 
    /// member that fulfills the condition, and if not found, returns null
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    T? Read(Func<T, bool> filter);
    /// <summary>
    /// The function reads many objects, puts them in a list and returns the list to us
    /// </summary>
    /// <returns>the list of objects </returns>
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null);
    /// <summary>
    /// Function to update object details
    /// </summary>
    /// <param name="item"></param>
    void Update(T item); //Updates entity object
    /// <summary>
    /// Deleting a certain object
    /// </summary>
    /// <param name="id">ID card of a certain object</param>
    void Delete(int id); //Deletes an object by its Id
    /// <summary>
    /// Deleting all objects
    /// </summary>
    void DeleteAll(); //Delete all entity objects


}


