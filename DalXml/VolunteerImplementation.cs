namespace Dal;
using DalApi;
using DO;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;


/// <summary>
/// The implementation of the volunteer's interface in the XML.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Creting a new volunteer in the XML.
    /// </summary>
    /// <param name="item">The volunteer to enter.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a volunteer with the specified ID already exists.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        XElement rootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        if (rootElem.Elements().FirstOrDefault(v => v.Element("Id").Value == item.Id.ToString()) != null)
            throw new DalAlreadyExistsException($"An object of type volunteer with ID = {item.Id} already exists");
        XElement volunteerElem = CreateVolunteerElement(item);
        rootElem.Add(volunteerElem);
        XMLTools.SaveListToXMLElement(rootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deleting a volunteer from the XML
    /// </summary>
    /// <param name="id">The Id of the volunteer to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when a volunteer with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement rootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (rootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist")).Remove();
        XMLTools.SaveListToXMLElement(rootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deleting all the volunteers from the XML.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement rootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        rootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(rootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    ///  Reading a volunteer from the XML.
    /// </summary>
    /// <param name="id">The Id of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        XElement volenteertElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(v => v.Element("Id").Value == id.ToString());
        return volenteertElem is null ? null : GetVolunteer(volenteertElem);
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
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => GetVolunteer(v)).FirstOrDefault(filter);
    }

    /// <summary>
    /// reading volunteers from the XML.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>A list of volunteers for whom the filter returns true
    /// and if it is null then all volunteers</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Where(v => filter == null|| filter(GetVolunteer(v)) ).Select(v => GetVolunteer(v));
    }

    /// <summary>
    /// updating a volunteer in the XML.
    /// </summary>
    /// <param name="item">The update volunteer.</param>
    /// <exception cref="DalDoesNotExistException"></exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id) ?? throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist")).Remove();

        volunteersRootElem.Add(new XElement( CreateVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }


    /// <summary>
    /// Converts from a volunteer element to a volunteer object.
    /// </summary>
    /// <param name="v">The volunteer element.</param>
    /// <returns>A object of volunteer.</returns>
    /// <exception cref="DO.FormatException">When he fails to convert it properly</exception>  
    public static Volunteer GetVolunteer(XElement v)
    {
        return new Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new DO.FormatException("can't convert id"),
            Name = (string?)v.Element("Name") ?? "",
            PhoneNumber = (string?)v.Element("Phone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            RoleType = v.ToEnumNullable<Role>( "RoleType")?? Role.Management,
            Active = (bool?)v.Element("Active") ?? false,
            DistanceType = v.ToEnumNullable<Distance>("DistanceType") ?? Distance.Aerial,
            Password = DecryptPassword((string?)v.Element("Password")) ?? "",
            FullAddress = (string?)v.Element("Address") ,
            Latitude = v.ToDoubleNullable("Latitude"),
            Longitude = v.ToDoubleNullable("Longitude"),
            MaxDistance = v.ToDoubleNullable("MaxDistance")
        };
    }
    /// <summary>
    /// Converts from a volunteer object to a volunteer element.
    /// </summary>
    /// <param name="v">The object of volunteer. </param>
    /// <returns>A element of volunteer.</returns>  
    public static XElement CreateVolunteerElement(Volunteer v)
    {
        return new XElement("Volunteer",
            new XElement("Id", v.Id),
            new XElement("Name", v.Name),
            new XElement("Phone", v.PhoneNumber),
            new XElement("Email", v.Email),
            new XElement("RoleType", v.RoleType),
            new XElement("Active", v.Active),
            new XElement("DistanceType", v.DistanceType),
            new XElement("Password", EncryptPassword(v.Password)),
            new XElement("Address", v.FullAddress),
            new XElement("Latitude", v.Latitude),
            new XElement("Longitude", v.Longitude),
            new XElement("MaxDistance", v.MaxDistance));
    }


    private static string? EncryptPassword(string ?password)
    {
        if (password == null)
            return null;
        StringBuilder encrypted = new StringBuilder();
        foreach (char c in password)
        {
            encrypted.Append((char)(c + 3));  
        }
        return encrypted.ToString();
    }

    
    private static string? DecryptPassword(string ?encryptedPassword)
    {
        if (encryptedPassword == null)
            return null;
        StringBuilder decrypted = new StringBuilder();
        foreach (char c in encryptedPassword)
        {
            decrypted.Append((char)(c - 3));  
        }
        return decrypted.ToString();
    }
}
