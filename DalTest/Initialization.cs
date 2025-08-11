
namespace DalTest;
using DalApi;
using DO;
using System;
using System.IO;
using System.Numerics;
using System.Xml.Linq;
/// <summary>
/// Initialization of the records and lists
/// </summary>
public static class Initialization
{
    private static IDal s_dal;
    
    /// <summary>
    /// A random variable
    /// </summary>
    private static readonly Random s_rand = new();

    /// <summary>
    /// A function that actually initializes all the data
    /// </summary>
    /// <param name="s_dal">An object that contains all entities</param>
    /// <exception cref="Exception">Thrown if the initialization process encounters an error.</exception>   
    public static void Do()
    {
        ///Check if the object is null
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        s_dal = DalApi.Factory.Get;

        ///Reset the configuration values and the list values
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();//stage 2     

        ///The functions that send to the actual initialization
        Console.WriteLine("Initializing Volunteer  ...");
        createVolunteer();
        Console.WriteLine("Initializing Call ...");
        createCall();
        Console.WriteLine("Initializing Assignment  ...");
        createAssignment();
    }

    /// <summary>
    /// Initializes the list
    /// </summary>
    private static void createVolunteer()
    {
        ///Arrays of names for volunteers and their email

        string[] volunteerNames =
        {
            "Yuval-Or Michaeli", "Itay Blum", "Amit Grossman", "Tal Shapiro", "Yossi Mizrachi",
            "Shira Ben-David", "Sarah Cohen", "Michal Avraham", "Nir Carmi", "Gal Noy",
            "Ronen Fisher", "Yael Katz", "Hila Raz", "Noam Peretz", "Maya Solomon",
            "Erez Ben-Yosef", "Avital Ashkenazi", "Ariel Ben-Ami", "Tzvika Mor", "Lior Shani",
            "Adi Malka", "Doron Peled", "Eden Levi", "Benny Tal", "Gali Levy",
            "Avi Nave", "Yaara Shmuel", "Yonatan Green", "Hadar Silver", "Ella Sapir",
            "David Golan", "Shani Alon", "Omer Dahan", "Rotem Menashe", "Roi Bachar",
            "Tomer Brill", "Linor Yaakov", "Hodaya Shaked", "Elad Rozen", "Maayan Sasson",
            "Hanan Ravid", "Ron Aloni", "Galit Azulay", "Oded Tal", "Dana Biran",
            "Ofir Gil", "Nofar Ben-Atar", "Ruth Shalom", "Itamar Cohen", "Yael Ben-Zvi"
        };
        string[] volunteermails ={
    "Yuval", "Itay", "Amit", "Tal", "Yossi",
    "Shira", "Sarah", "Michal", "Nir", "Gal",
    "Ronen", "Yael", "Hila", "Noam", "Maya",
    "Erez", "Avital", "Ariel", "Tzvika", "Lior",
    "Adi", "Doron", "Eden", "Benny", "Gali",
    "Avi", "Yaara", "Yonatan", "Hadar", "Ella",
    "David", "Shani", "Omer", "Rotem", "Roi",
    "Tomer", "Linor", "Hodaya", "Elad", "Maayan",
    "Hanan", "Ron", "Galit", "Oded", "Dana",
    "Ofir", "Nofar", "Ruth", "Itamar", "Yael"
    };
        

        string[] addresses =
        {
            // Herzliya
            "Wingate 38 Herzliya",
            "Abba Eban 2 Herzliya",
            "Arlozorov 4 Herzliya",
            "Ben Yehuda 12 Herzliya",
            "Hagalil 11 Herzliya",
            "Maskit 27 Herzliya",
            "Ben Gurion 12 Herzliya",
            "Ramat Yam 60 Herzliya",
            "Weizmann 8 Kfar Saba",
            "Shenkar 3 Herzliya",

            // Netanya
            "Herzl 20 Netanya",
            "Ben Gurion 1 Netanya",
            "Haatzmaut 34 Sderot",
            "Tchernichovsky 7 Netanya",
            "Dizengoff 16 Netanya",
            "Sheshet Hayamim 3 Netanya",
            "Ruppin 21 Netanya",
            "HaNeviim 18 Netanya",
            "Ben Ami 28 Netanya",
            "Tchernichovsky 32 Netanya",

            // Ra'anana
            "Ahuza 124 Raanana",
            "HaPalmach 18 Raanana",
            "Weizmann 40 Kfar Saba",
            "Herzl 53 Raanana",
            "Nordau 5 Raanana",
            "HaRav Kook 10 Raanana",
            "HaYovel 16 Raanana",
            "Ben Gurion 5 Raanana",
            "HaEshel 7 Raanana",
            "Dizengoff 48 Tel Aviv",

            // Tel Aviv
            "Rothschild 22 Tel Aviv",
            "Ibn Gabirol 15 Tel Aviv",
            "Dizengoff 102 Tel Aviv",
            "Allenby 99 Tel Aviv",
            "King George 29 Tel Aviv",
            "Shenkin 17 Tel Aviv",
            "Bograshov 20 Tel Aviv",
            "Frishman 22 Tel Aviv",
            "Yehuda HaMaccabi 15 Tel Aviv",
            "Mikve Israel 19 Tel Aviv",

            // Kfar Saba
            "Weizmann 35 Kfar Saba",
            "Herzl 12 Kfar Saba",
            "Weizmann 50 Kfar Saba",
            "Arlozorov 25 Kfar Saba",
            "Dizengoff 55 Tel Aviv",
            "Herzl 78 Kfar Saba",
            "Tel Hai 33 Kfar Saba",
            "HaRav Maimon 6 Herzliya",
            "Rothschild 9 Kfar Saba",
            "Sokolov 3 Kfar Saba"
        };

        double[] latitudes = {
    32.1793471, 32.1607272, 32.1606701, 32.1627761, 32.1658838, 32.1632571,
    32.1650377, 32.171456, 32.1779805, 32.1602438, 32.3290299, 32.279103,
    31.5300071, 32.3219803, 32.3306203, 32.321458, 32.28087, 32.2967773,
    32.321458, 32.3208972, 32.1808257, 32.1905863, 32.1705469, 32.1775266,
    32.1854065, 32.1961516, 32.1843411, 32.180409, 32.1916612, 32.0748561,
    32.0627896, 32.0726935, 32.0796074, 32.0646173, 32.0717837, 32.0695395,
    32.0769951, 32.0797596, 32.0938841, 32.062857, 32.177421, 32.1709483,
    32.1765135, 32.1708769, 32.0754236, 32.1772724, 32.1737073, 32.1652867,
    32.1709569, 32.1762246
};

        double[] longitudes = {
    34.8058526, 34.8103495, 34.8428238, 34.8451422, 34.8439588, 34.8107107,
    34.8424487, 34.800546, 34.8957918, 34.8095785, 34.8555807, 34.852303,
    34.5897336, 34.8527596, 34.8543236, 34.853196, 34.853791, 34.8577569,
    34.853196, 34.8557568, 34.8739939, 34.8741786, 34.9286637, 34.8678441,
    34.8853162, 34.8743499, 34.8725648, 34.87231, 34.8585775, 34.7761884,
    34.7714756, 34.7816096, 34.7740421, 34.7726696, 34.7731051, 34.7723678,
    34.7692827, 34.7706442, 34.7860657, 34.7770402, 34.899857, 34.9104764,
    34.9015956, 34.9047851, 34.7748328, 34.9121208, 34.9138018, 34.83199,
    34.9070413, 34.9028277
};


        int i = 0;
        ///A loop that will go through the list of volunteers and insert values into the list
        foreach (var name in volunteerNames)
        {
            int id;
            do
                id = s_rand.Next(100000000, 999999999);
            while (s_dal.Volunteer.Read(id) != null || !ValidateId(id));
            
            string mail = $"{volunteermails[i]}@gmail.com" ;
            bool even = (id % 7) != 0 ? true : false;
            int temp = s_rand.Next(1000000, 9999999);
            string phone = "054" + temp.ToString();
            string password = "password123!?PP";
            int max = s_rand.Next(0, 1000);
            string address = addresses[i];
            double latitude = latitudes[i];
            double longitude = longitudes[i++];
            Distance distance = (Distance)s_rand.Next(0, 3);

            if (i==1)
                s_dal.Volunteer.Create( new Volunteer(212061006, name,phone ,mail,Role.Management, even, distance, password, MaxDistance: max, FullAddress: address,Latitude: latitude,Longitude:longitude));
            else
                s_dal.Volunteer.Create( new Volunteer(id, name, phone, mail, Role.Volunteer, even, distance, password,MaxDistance:max,FullAddress:address, Latitude: latitude, Longitude: longitude));

        }
    }

    /// <summary>
    /// Validates an ID number using a checksum algorithm.
    /// </summary>
    /// <param name="id">The ID number to validate.</param>
    /// <returns>True if the ID is valid; otherwise, false.</returns>
    public static bool ValidateId(int id)
    {

        if (id < 100000000 || id > 999999999)
            return false;
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = id % 10;
            id /= 10;
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }
            sum += digit;
        }
        return sum % 10 == 0;
    }

    /// <summary>
    /// Initializes the call list
    /// </summary>
    private static void createCall()
    {
        ///Array with addresses
        string[] addresses =
      {
            
            "Wingate 38 Herzliya",
            "Abba Eban 2 Herzliya",
            "Arlozorov 4 Herzliya",
            "Ben Yehuda 12 Herzliya",
            "Hagalil 11 Herzliya",
            "Maskit 27 Herzliya",
            "Ben Gurion 12 Herzliya",
            "Ramat Yam 60 Herzliya",
            "Weizmann 8 Kfar Saba",
            "Shenkar 3 Herzliya",

            
            "Herzl 20 Netanya",
            "Ben Gurion 1 Netanya",
            "Haatzmaut 34 Sderot",
            "Tchernichovsky 7 Netanya",
            "Dizengoff 16 Netanya",
            "Sheshet Hayamim 3 Netanya",
            "Ruppin 21 Netanya",
            "HaNeviim 18 Netanya",
            "Ben Ami 28 Netanya",
            "Tchernichovsky 32 Netanya",

            
            "Ahuza 124 Raanana",
            "HaPalmach 18 Raanana",
            "Weizmann 40 Kfar Saba",
            "Herzl 53 Raanana",
            "Nordau 5 Raanana",
            "HaRav Kook 10 Raanana",
            "HaYovel 16 Raanana",
            "Ben Gurion 5 Raanana",
            "HaEshel 7 Raanana",
            "Dizengoff 48 Tel Aviv",

            
            "Rothschild 22 Tel Aviv",
            "Ibn Gabirol 15 Tel Aviv",
            "Dizengoff 102 Tel Aviv",
            "Allenby 99 Tel Aviv",
            "King George 29 Tel Aviv",
            "Shenkin 17 Tel Aviv",
            "Bograshov 20 Tel Aviv",
            "Frishman 22 Tel Aviv",
            "Yehuda HaMaccabi 15 Tel Aviv",
            "Mikve Israel 19 Tel Aviv",

            
            "Weizmann 35 Kfar Saba",
            "Herzl 12 Kfar Saba",
            "Weizmann 50 Kfar Saba",
            "Arlozorov 25 Kfar Saba",
            "Dizengoff 55 Tel Aviv",
            "Herzl 78 Kfar Saba",
            "Tel Hai 33 Kfar Saba",
            "HaRav Maimon 6 Herzliya",
            "Rothschild 9 Kfar Saba",
            "Sokolov 3 Kfar Saba"
        };

        double[] latitudes = {
    32.1793471, 32.1607272, 32.1606701, 32.1627761, 32.1658838, 32.1632571,
    32.1650377, 32.171456, 32.1779805, 32.1602438, 32.3290299, 32.279103,
    31.5300071, 32.3219803, 32.3306203, 32.321458, 32.28087, 32.2967773,
    32.321458, 32.3208972, 32.1808257, 32.1905863, 32.1705469, 32.1775266,
    32.1854065, 32.1961516, 32.1843411, 32.180409, 32.1916612, 32.0748561,
    32.0627896, 32.0726935, 32.0796074, 32.0646173, 32.0717837, 32.0695395,
    32.0769951, 32.0797596, 32.0938841, 32.062857, 32.177421, 32.1709483,
    32.1765135, 32.1708769, 32.0754236, 32.1772724, 32.1737073, 32.1652867,
    32.1709569, 32.1762246
};

        double[] longitudes = {
    34.8058526, 34.8103495, 34.8428238, 34.8451422, 34.8439588, 34.8107107,
    34.8424487, 34.800546, 34.8957918, 34.8095785, 34.8555807, 34.852303,
    34.5897336, 34.8527596, 34.8543236, 34.853196, 34.853791, 34.8577569,
    34.853196, 34.8557568, 34.8739939, 34.8741786, 34.9286637, 34.8678441,
    34.8853162, 34.8743499, 34.8725648, 34.87231, 34.8585775, 34.7761884,
    34.7714756, 34.7816096, 34.7740421, 34.7726696, 34.7731051, 34.7723678,
    34.7692827, 34.7706442, 34.7860657, 34.7770402, 34.899857, 34.9104764,
    34.9015956, 34.9047851, 34.7748328, 34.9121208, 34.9138018, 34.83199,
    34.9070413, 34.9028277
};
        ///An array with an optional description to describe readings
        string[] callsDescription = {
    "There is a unit in the area who needs food. Who can deliver?",
    "We have a soldier stuck without a meal. Can anyone bring food?",
    "A hot meal is needed urgently at the base. Volunteers?",
    "Extra food supplies are required for tomorrow's operations.",
    "A family nearby cooked meals; we need help delivering them.",
    
    // EquipmentDelivery
    "A unit requires equipment to be delivered to a remote location.",
    "Critical supplies need transportation to the frontline. Who can assist?",
    "Spare parts for vehicles are ready for pickup and delivery.",
    "New equipment needs to be brought to the main storage area.",
    "Rain gear is required for the soldiers on patrol. Can anyone bring it?",
    
    // EquipmentRepair
    "A broken radio needs immediate repair to restore communication.",
    "Field equipment is damaged. Who can repair or replace it?",
    "Tents were torn in the storm and need fixing ASAP.",
    "A soldier's backpack strap is broken. Who can help repair it?",
    "The water pump isn't working. A technician is needed urgently.",
    
    // Rides
    "A soldier needs a ride to the base from the train station.",
    "Two soldiers are stranded without transportation. Can anyone help?",
    "A ride is needed to deliver documents to headquarters.",
    "A volunteer with a car is needed for a supply run.",
    "A group of soldiers needs transport back to their barracks.",
    
    // Laundry
    "Dirty uniforms have piled up. Can anyone do a batch of laundry?",
    "The soldiers' bedding needs urgent washing. Volunteers?",
    "We need help laundering blankets for a large unit.",
    "Who can take care of a load of laundry for the soldiers?",
    "Wet clothes need drying and folding. Assistance required.",
    
    // StorageOfBelongings
    "Personal belongings need safe storage for a few days.",
    "A soldier is heading to the field and needs a place to store valuables.",
    "Unit gear needs to be held securely while repairs are done.",
    "Who can keep a package safe until the weekend?",
    "Temporary storage is required for heavy equipment.",
    
    // MedicalHelp
    "A medic is needed for a soldier feeling unwell.",
    "Urgent medical supplies are required for a nearby unit.",
    "First aid assistance is needed at the base immediately.",
    "A volunteer doctor is required for a quick health check-up.",
    "A soldier twisted his ankle and needs medical attention.",
    
    // EventOrganization
    "Help is needed to set up a morale-boosting event for the soldiers.",
    "We’re organizing a celebration for the unit. Who can assist?",
    "Volunteers are required to arrange an appreciation dinner.",
    "A movie night is planned for the soldiers. Help setting up?",
    "A farewell party for a retiring officer is being arranged.",
    
    // PsychologicalAssistance
    "A soldier is struggling emotionally. Can someone help?",
    "We’re looking for a volunteer psychologist to speak with the unit.",
    "Support is needed for soldiers dealing with stress.",
    "A soldier is feeling overwhelmed and needs someone to talk to.",
    "The unit is requesting a workshop on stress management.",
    
    // MotivationalTalks
    "Looking for a motivational speaker to uplift the soldiers.",
    "A soldier who excelled in combat wants to share his story.",
    "Organizing a talk to inspire the unit. Who can assist?",
    "We’re planning a motivational session for the soldiers.",
    "A veteran will be speaking about resilience. Help is needed setting up."
};
        ///A loop that will go through the array of readings and insert data
        for (int i=0; i < 50; i++)
        {
       
            CallType type = (CallType)s_rand.Next(0, 10);
            string description = callsDescription[s_rand.Next(0, callsDescription.Length)];
            DateTime openingTime = new DateTime(s_dal.Config.Clock.Year - 2, 1, 1);
            int range = (s_dal.Config.Clock - openingTime).Days;
            openingTime = openingTime.AddDays(s_rand.Next(range));

            s_dal.Call.Create(new Call(0,type , addresses [i], latitudes[i], longitudes[i] ,  openingTime,description,openingTime.AddYears(3)));
        }
    }

    /// <summary>
    /// Initialize the list of assignment
    /// </summary>
    private static void createAssignment()
    {
        ///The variable outside the loop to prevent data overflow
        int i =0;
        IEnumerable<Volunteer> Volunteers = s_dal.Volunteer.ReadAll();
        int voluteerId = Volunteers.ElementAt(s_rand.Next(0, 50)).Id;

        IEnumerable<Call> Calls = s_dal.Call.ReadAll();
        ///A loop that will go through the list of assignments and insert 10 values into the list
        for (;i<10;i++)
        {
            Call call = Calls.ElementAt(i);
            DateTime entryTime = call.OpeningTime.AddDays(s_rand.Next(2,20));
            TypeOfEnding typeOfEnding = (TypeOfEnding)s_rand.Next(0, 3);
            s_dal.Assignment.Create(new Assignment(0, call.Id, voluteerId, entryTime , entryTime.AddHours(s_rand.Next(30)),typeOfEnding));
        }
        ///A loop that will go through the list of assignments and insert 10 values into the list
        ///From number 10 to 15
        for (; i < 15; i++)
        {
            do
            {
                voluteerId = Volunteers.ElementAt(s_rand.Next(0, 50)).Id;
            }
            while (s_dal.Call.Read(voluteerId) is not null);
            Call call = Calls.ElementAt(i);
            DateTime entryTime = call.OpeningTime.AddDays(2);
            s_dal.Assignment.Create(new Assignment(0, call.Id, voluteerId, entryTime, entryTime.AddHours(s_rand.Next(30)), TypeOfEnding.Treated));
        }

        ///A loop that will go through the list of assignments and insert 10 values into the list
        ///From number 45 to 49

        for (; i < 25; i++)
        {
            voluteerId = Volunteers.ElementAt(s_rand.Next(0, 50)).Id;
            Call call = Calls.ElementAt(i);
            DateTime entryTime = call.OpeningTime.AddDays(2);
            TypeOfEnding typeOfEnding = (TypeOfEnding)s_rand.Next(1, 3);
            s_dal.Assignment.Create(new Assignment(0, call.Id, voluteerId, entryTime, entryTime.AddHours(s_rand.Next(30)), TheEndType: typeOfEnding));
        }


        ///A loop that will go through the list of assignments and insert 20 values into the list
        ///From number 25 to 45
        ///Only takes active volunteers who don't already have an assignment
        ///and gives them open status
        for (; i < 45; i++) 
        {
            do
            {
                voluteerId = Volunteers.ElementAt(s_rand.Next(0, 50)).Id;   
            }
            while (s_dal.Assignment.Read(a => a.VolunteerId == voluteerId && a.ExitTime == null) != null || s_dal.Volunteer.Read(voluteerId)!.Active == false );
            Call call = Calls.ElementAt(i);
            DateTime entryTime = call.OpeningTime.AddDays(2);
            s_dal.Assignment.Create(new Assignment(0, call.Id, voluteerId, entryTime));
        }
    }
}
