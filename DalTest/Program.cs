namespace DalTest; 
using Dal;
using DalApi;
using DO;

using System.Transactions;
internal class Program
{
    /// <summary>
    /// Defining stocks for the switch
    /// </summary>
    private enum CoiceForMenu { Exit, Volunteer, Call, Assignment, Config, Initialization, DisplayAll, Reset }
    private enum ChoiceForSub { Exit, Create, Read, ReadAll, Update, Delete, DeleteAll }
    private enum ChoiceForConfig { Exit, AdvanceClockByMinute, AdvanceClockByHour, AdvanceClockByDay, DisplayClock, SetConfigVariable, DisplayConfigVariable, ResetConfigVariable }





    /// <summary>
    /// Defining variables
    /// </summary>
    static readonly IDal s_dal = Factory.Get; //stage 4


    private static readonly Random s_rand = new();
    /// <summary>
    /// The main program with the menu function in it
    /// </summary>
    static void Main(string[] args)
    {
        try
        {
            mainMenu();
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex);
        }

    }
    /// <summary>
    /// The main menu
    /// </summary>
    private static void mainMenu()
    {
        CoiceForMenu choice;
        do
        {
            Console.WriteLine("Enter 0. to Exit the Main Menu:");
            Console.WriteLine("Enter 1. to see Volunteer Menu:");
            Console.WriteLine("Enter 2. to see Call Menu:");
            Console.WriteLine("Enter 3. to see Assignment Menu:");
            Console.WriteLine("Enter 4. to see Config Menu:");
            Console.WriteLine("Enter 5. to initializ the data:");
            Console.WriteLine("Enter 6. to Display all the data in the database:");
            Console.WriteLine("Enter 7. to Reset the Database and configuration data :");
            Console.WriteLine("Enter your choice:");
            ///Convert a number to a stock type
            if (Enum.TryParse(Console.ReadLine(), out choice))
            {             
                try
                {
                    switch (choice)
                    {
                        case CoiceForMenu.Exit:
                            return;
                        case CoiceForMenu.Volunteer:
                            SubMenu("Volunteer");
                            break;
                        case CoiceForMenu.Call:
                            SubMenu("Call");
                            break;
                        case CoiceForMenu.Assignment:
                            SubMenu("Assignment");
                            break;
                        case CoiceForMenu.Config:
                            ConfigSubMenu();
                            break;
                        case CoiceForMenu.Initialization:
                            Initialization.Do(); //stage 4                                                     
                            break;
                        case CoiceForMenu.DisplayAll:
                            DisplayAllVolunteers();
                            DisplayAllCalls();
                            DisplayAllAssignments();
                            break;
                        case CoiceForMenu.Reset:
                            s_dal.ResetDB();//stage 2                          
                            break;
                        default:
                            Console.WriteLine("Invalid choice, please try again");
                            break;
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid choice, please try again");
            }

        } while (choice != CoiceForMenu.Exit);
    }
    /// <summary>
    /// Sub menu
    /// </summary>
    /// <param name="type">The selected variable means volunteer or call or task</param>
    private static void SubMenu(string type)
    {
        ///The output that comes out is like the selected variable 
        ///and the selected option activates the function that corresponds to it
        ChoiceForSub SubNenuChoice;
        do
        {
            Console.WriteLine("Enter 0. to exit the sub  menu:");
            Console.WriteLine($"Enter 1. to Create a new {type}:");
            Console.WriteLine($"Enter 2. to Read a {type}:");
            Console.WriteLine($"Enter 3. to Read all {type}s:");
            Console.WriteLine($"Enter 4. to Update a {type}:");
            Console.WriteLine($"Enter 5. to Delete a {type}:");
            Console.WriteLine($"Enter 6. to Delete all {type}s:");
            Console.WriteLine("Enter your choice:");

            ///Conversion of a number to a stock type variable
            if (Enum.TryParse(Console.ReadLine(), out SubNenuChoice))
            {
                try { 
                    switch (type)
                    {
                    case "Volunteer":
                        VolunteerSubMenu(SubNenuChoice);
                        break;
                    case "Call":
                        CallSubMenu(SubNenuChoice);
                        break;
                    case "Assignment":
                        AssignmentSubMenu(SubNenuChoice);
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again");
                        break;
                    }
                }               
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }               
            }
            else
            {
                Console.WriteLine("Invalid choice, please try again");
            }
        }
        while (SubNenuChoice != ChoiceForSub.Exit);

    }
    /// <summary>
    /// Volunteer submenu
    /// </summary>
    /// <param name="choice">The variable that receives the desired action and invokes the appropriate function</param>
    private static void VolunteerSubMenu(ChoiceForSub choice)
    {
        
            switch (choice)
            {
                case ChoiceForSub.Exit:
                    return;
                case ChoiceForSub.Create:
                    CreateVolunteer();
                    break;
                case ChoiceForSub.Read:
                    ReadVolunteer();
                    break;
                case ChoiceForSub.ReadAll:
                    DisplayAllVolunteers();
                    break;
                case ChoiceForSub.Update:
                    UpdateVolunteer();
                    break;
                case ChoiceForSub.Delete:
                    Console.WriteLine("Enter the Volunteer ID:");
                    int deleteId = int.Parse(Console.ReadLine());
                    s_dal.Volunteer.Delete(deleteId);
                    break;
                case ChoiceForSub.DeleteAll:
                    s_dal.Volunteer.DeleteAll();
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;

            }
        
    }
    /// <summary>
    /// A sub-menu of a assignment
    /// </summary>
    /// <param name="choice">The variable that receives the desired action and invokes the appropriate function</param>
    private static void AssignmentSubMenu(ChoiceForSub choice)
    {

        switch (choice)
        {
            case ChoiceForSub.Exit:
                return;
            case ChoiceForSub.Create:
                CreateAssignment();              
                break;
            case ChoiceForSub.Read:
                ReadAssignment();
                break;
            case ChoiceForSub.ReadAll:
                DisplayAllAssignments();
                break;
            case ChoiceForSub.Update:
                UpdateAssignment();
                break;
            case ChoiceForSub.Delete:
                Console.WriteLine("Enter the Assignment ID:");
                int deleteId = int.Parse(Console.ReadLine());
                s_dal.Assignment.Delete(deleteId);
                break;
            case ChoiceForSub.DeleteAll:
                s_dal.Assignment.DeleteAll();
                break;         
            default:
                Console.WriteLine("Invalid choice, please try again");
                break;
        }

    }
    /// <summary>
    /// A sub-menu of a call
    /// </summary>
    /// <param name="choice">The variable that receives the desired action and invokes the appropriate function</param>
    private static void CallSubMenu(ChoiceForSub choice)
    {

        switch (choice)
        {
            case ChoiceForSub.Exit:
                return;
            case ChoiceForSub.Create:
                CreateCall();                
                break;
            case ChoiceForSub.Read:
                ReadCall();
                break;
            case ChoiceForSub.ReadAll:
                DisplayAllCalls();
                break;
            case ChoiceForSub.Update:
                UpdateCall();
                break;
            case ChoiceForSub.Delete:
                Console.WriteLine("Enter the Call ID:");
                int deleteId = int.Parse(Console.ReadLine());
                s_dal.Call.Delete(deleteId);
                break;
            case ChoiceForSub.DeleteAll:
                s_dal.Call.DeleteAll();
                break;           
            default:
                Console.WriteLine("Invalid choice, please try again");
                break;
        }
    }
    /// <summary>
    /// A sub-menu of a config
    /// </summary>
    private static void ConfigSubMenu()
    {

        ChoiceForConfig SubNenuChoice;
        do
        {
            Console.WriteLine("Enter 0. to exit the sub  menu:");
            Console.WriteLine("Enter 1. to advance the clock by a minute:");
            Console.WriteLine("Enter 2. to advance the clock by a hours:");
            Console.WriteLine("Enter 3. to advance the clock by day:");
            Console.WriteLine("Enter 4. display current value of clock:");
            Console.WriteLine("Enter 5. set a new value to a configuration variable:");
            Console.WriteLine("Enter 6. display current value for a configuration variable:");
            Console.WriteLine("Enter 7. reset values for all configuration variables:");
            Console.WriteLine("Enter your choice:");
            
            if (Enum.TryParse(Console.ReadLine(), out SubNenuChoice))
            { 
                try
                {
                switch (SubNenuChoice)
                {
                    case ChoiceForConfig.Exit:
                        return;
                    case ChoiceForConfig.AdvanceClockByMinute:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                        break;
                    case ChoiceForConfig.AdvanceClockByHour:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                        break;
                    case ChoiceForConfig.AdvanceClockByDay:
                        s_dal.Config.Clock = s_dal.Config.Clock.AddDays(1);
                        break;
                    case ChoiceForConfig.DisplayClock:
                        Console.WriteLine(s_dal.Config.Clock);
                        break;
                    case ChoiceForConfig.SetConfigVariable:
                        UpdateIconfig();
                        break;
                    case ChoiceForConfig.DisplayConfigVariable:
                        ReadIconfig();
                        break;
                    case ChoiceForConfig.ResetConfigVariable:
                        s_dal.Config.Reset();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again");
                        break;
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid choice, please try again");
            }
        }
        while (SubNenuChoice != ChoiceForConfig.Exit);

    }
    /// <summary>
    /// Reads a volunteer's details by their ID and displays the details if found.
    /// </summary>
    private static void ReadVolunteer()
    {
        Console.WriteLine("Enter the Volunteer ID:");
        int readId = int.Parse(Console.ReadLine());
        Volunteer volunteer = s_dal.Volunteer.Read(readId);
        if (volunteer is not null)
        {
            Console.WriteLine(volunteer);
        }
        else
        {
            Console.WriteLine("Volunteer not found.");
        }

    }

    /// <summary>
    /// Reads a assignment's details by their ID and displays the details if found.
    /// </summary>
    private static void ReadAssignment()
    {
        Console.WriteLine("Enter the Assignment ID:");
        int readId = int.Parse(Console.ReadLine());
        Assignment assignment = s_dal.Assignment.Read(readId);
        if (assignment is not null)
        {
            Console.WriteLine(assignment);
        }
        else
        {
            Console.WriteLine("Assignment not found.");
        }
    }

    /// <summary>
    /// Reads a call's details by their ID and displays the details if found.
    /// </summary>
    private static void ReadCall()
    {
        Console.WriteLine("Enter the Call ID:");
        int readId = int.Parse(Console.ReadLine());
        Call call = s_dal.Call.Read(readId);
        if (call is not null)
        {
            Console.WriteLine(call);
        }
        else
        {
            Console.WriteLine("Call not found.");
        }
    }
    /// <summary>
    /// Reception of volunteer details
    /// </summary>
    private static void CreateVolunteer()
    {
        Console.WriteLine("Enter the Volunteer ID:");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the Volunteer Name:");
        string name = Console.ReadLine();
        Console.WriteLine("Enter the Volunteer Phone:");
        string phone = Console.ReadLine();
        Console.WriteLine("Enter the Volunteer Address:");
        string address = Console.ReadLine();
        Console.WriteLine("Enter the Volunteer Email:");
        string mail = Console.ReadLine();
        Console.WriteLine("Enter the Volunteer Role:  1. for Manager, 2. for Volunteer");
        int role = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the status: 1. Active 2, Inactive");
        bool status = int.Parse(Console.ReadLine()) == 1 ? true : false;
        Console.WriteLine("Enter the distance type: 1. for Aerial, 2. for Walking, 3. for Driving");
        int distance = int.Parse(Console.ReadLine())-1;
        Console.WriteLine("Enter your new Password:");
        string password = Console.ReadLine();
        s_dal.Volunteer.Create(new Volunteer(id, name, phone, mail, (Role)role, status, (Distance)distance, password));
    }
    /// <summary>
    ///Update the details of the volunteer and if there is no value, the previous value remains
    /// </summary>
    private static void UpdateVolunteer()
    {
        Console.WriteLine("Enter the Volunteer ID:");
        int id = int.Parse(Console.ReadLine());
        Volunteer volunteer = s_dal.Volunteer.Read(id);
        if (volunteer is not null)
        {
            Console.WriteLine(volunteer);
            Console.WriteLine("Enter the Volunteer Name:");
            string tempName = Console.ReadLine();
            string name = tempName == "" ? volunteer.Name : tempName;
            Console.WriteLine("Enter the Volunteer Phone:");
            string tempPhone = Console.ReadLine();
            string phone = tempPhone == "" ? volunteer.PhoneNumber : tempPhone;
            Console.WriteLine("Enter the Volunteer Address:");
            string tempAddress = Console.ReadLine();
            string address = tempAddress == "" ? volunteer.FullAddress : tempAddress;
            Console.WriteLine("Enter the Volunteer Email:");
            string tempMail = Console.ReadLine();
            string mail = tempMail == "" ? volunteer.Email : tempMail;
            Console.WriteLine("Enter the Volunteer Role:  1. for Manager, 2. for Volunteer");
            string temprole = Console.ReadLine();
            int role = temprole == "" ? (int)volunteer.RoleType : int.Parse(temprole);
            Console.WriteLine("Enter the status: 1. Active 2, Inactive");
            string tempstatus = Console.ReadLine();
            bool status = tempstatus == "" ? volunteer.Active : int.Parse(tempstatus) == 1 ? true : false;          
            Console.WriteLine("Enter the distance type: 1. for Aerial, 2. for Walking, 3. for Driving");
            string tempdistance = Console.ReadLine();
            int distance = tempdistance == "" ? (int)volunteer.DistanceType : int.Parse(tempdistance)-1;       
            Console.WriteLine("Enter your new Password:");
            string tempPassword = Console.ReadLine();
            string password = tempPassword == "" ? volunteer.Password : tempPassword;
            s_dal.Volunteer.Update(new Volunteer(id, name, phone, mail, (Role)role, status, (Distance)distance, password));
        }
        else
        {
            Console.WriteLine("Volunteer not found.");
        }
    }

    /// <summary>
    /// Opening a new call and filling in the details
    /// </summary>
    /// <returns>The function returns the ID of that call</returns>
    private static void CreateCall()
    {
        Console.WriteLine("Enter the Call Type: 1.Food delivery, 2.Equipment delivery, 3.Equipment repair, 4.Rides, 5.Laundry, 6.Storage of belongings, 7.Medical help, 8.Event organization, 9.Psychological assistance, 10.Motivational talks ");
        
int type = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the Call Address:");
        string address = Console.ReadLine();
        Console.WriteLine("Enter the Call Latitude:");
        double latitude = double.Parse(Console.ReadLine());
        Console.WriteLine("Enter the Call Longitude:");
        double longitude = double.Parse(Console.ReadLine());
        Console.WriteLine("Enter the Call Description:");
        string description = Console.ReadLine();
        DateTime openingTime = s_dal.Config.Clock;
        s_dal.Call.Create(new Call(0, (DO.CallType)type, address, latitude, longitude, openingTime, description, openingTime.AddDays(s_rand.Next(5, 20))));
    }

    /// <summary>
    /// Updating reading details and leaving the previous details when no value is received
    /// </summary>
    private static void UpdateCall()
    {
        Console.WriteLine("Enter the Call ID:");
        int id = int.Parse(Console.ReadLine());
        Call call = s_dal.Call.Read(id);
        if (call is not null)
        {

            Console.WriteLine(call);
            Console.WriteLine("Enter the Call Type: 1.Food delivery, 2.Equipment delivery, 3.Equipment repair, 4.Rides, 5.Laundry, 6.Storage of belongings, 7.Medical help, 8.Event organization, 9.Psychological assistance, 10.Motivational talks ");
            string tempType = Console.ReadLine();
            int type = tempType == "" ? (int)call.Type : int.Parse(tempType);          
            Console.WriteLine("Enter the Call Address:");
            string tempAddress = Console.ReadLine();
            string address = tempAddress == "" ? call.FullCallAddress : tempAddress;             
            Console.WriteLine("Enter the Call Latitude:");
            string tempLatitude = Console.ReadLine();
            double latitude = tempLatitude == "" ? call.Latitude : double.Parse(tempLatitude);
            Console.WriteLine("Enter the Call Longitude:");
            string tempLongitude = Console.ReadLine();           
            double longitude = tempLongitude == "" ? call.Longitude : double.Parse(tempLongitude);          
            Console.WriteLine("Enter the Call Description:");
            string tempDescription = Console.ReadLine();
            string description = tempDescription == "" ? call.VerbalDescription : tempDescription;          
            DateTime openingTime = s_dal.Config.Clock;
            s_dal.Call.Update(new Call(id, (DO.CallType)type, address, latitude, longitude, openingTime, description, openingTime.AddDays(s_rand.Next(5, 20))));
        }
        else
        {
            Console.WriteLine("Call not found.");
        }
    }


    /// <summary>
    /// Creating a new task and matching a volunteer to a call
    /// </summary>
    /// <returns>The function returns the ID of that assignment</returns>
    private static void CreateAssignment()
    {
        bool check;
        int callId;
        ///Checking if there is a call with a certain ID number
        do
        {
            check = false;
            Console.WriteLine("Enter the Call ID:");
            callId = int.Parse(Console.ReadLine());
            if (s_dal.Call.Read(callId) is null)
            {
                Console.WriteLine("Call not found, please try again");
                check = true;
            }
                

        } while (check);

        int volunteerId;
        ///Checking if there is a volunteer with a certain ID number
        do
        {
            check = false;
            Console.WriteLine("Enter the Volunteer Id:");
            volunteerId = int.Parse(Console.ReadLine());
            if (s_dal.Volunteer.Read(volunteerId) is null)
            {
                Console.WriteLine("Volunteer not found, please try again");
                check = true;
            }
        } while (check);


        DateTime entryTime = s_dal.Config.Clock;

        Console.WriteLine("Enter the Assignment Type of Ending: 1. Treated,2. SelfCancellation,3. CancelingAnAdministrator");
        int typeOfEnding = int.Parse(Console.ReadLine());
        s_dal.Assignment.Create(new Assignment(0, callId, volunteerId, entryTime, entryTime.AddHours(s_rand.Next(5, 20)), (TypeOfEnding)typeOfEnding));
    }
    /// <summary>
    /// Updating task details
    /// </summary>
    private static void UpdateAssignment()
    {
        Console.WriteLine("Enter the Call ID:");
        int id = int.Parse(Console.ReadLine());
        Assignment assignment = s_dal.Assignment.Read(id);
        if (assignment is not null)
        {
            Console.WriteLine(assignment);

            IEnumerable<Volunteer> Volunteers = s_dal.Volunteer.ReadAll();
            IEnumerable<Call> Calls = s_dal.Call.ReadAll();
            int callId = Calls.ElementAt(s_rand.Next(0, Calls.Count())).Id;
            int volunteerId = Volunteers.ElementAt(s_rand.Next(0, Volunteers.Count())).Id;
            DateTime entryTime = s_dal.Config.Clock;
            Console.WriteLine("Enter the Assignment Type of Ending: 1. Treated,2. SelfCancellation,3. CancelingAnAdministrator");
            string tempTypeOfEnding = Console.ReadLine();
            int typeOfEnding = tempTypeOfEnding == "" ? (int)assignment.TheEndType : int.Parse(tempTypeOfEnding);           
            s_dal.Assignment.Update(new Assignment(id, callId, volunteerId, entryTime, entryTime.AddHours(s_rand.Next(5, 20)), (TypeOfEnding)typeOfEnding));
        }
        else
        {
            Console.WriteLine("Assignment is not found");
        }
    }

    /// <summary>
    /// Printing all volunteer details
    /// </summary>
    private static void DisplayAllVolunteers()
    {
        IEnumerable<Volunteer> volunteers = s_dal.Volunteer.ReadAll();
        foreach (Volunteer volunteer in volunteers)
        {
            Console.WriteLine(volunteer);
        }
    }
    /// <summary>
    /// Printing all call details
    /// </summary>
    private static void DisplayAllCalls()
    {
        IEnumerable<Call> calls = s_dal.Call.ReadAll();
        foreach (var call in calls)
        {
            Console.WriteLine(call);
        }
    }
    /// <summary>
    /// Printing all Assignments details
    /// </summary>
    private static void DisplayAllAssignments()
    {
        IEnumerable<Assignment> assignments = s_dal.Assignment.ReadAll();
        foreach (var assignment in assignments)
        {
            Console.WriteLine(assignment);
        }
    }
    /// <summary>
    /// Update times and if it is not in a compatible format then there is a throwaway
    /// </summary>
    /// <exception cref="FormatException"></exception>
    private static void UpdateIconfig()
    {
        string choice;
        do
        {
            Console.WriteLine("Enter 1 to change the clock or 2 to change the risk range ");
            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.WriteLine("enter the DateTime (in format dd/mm/yy hh:mm:ss): ");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime dt)) throw new FormatException("DateTime is invalid!");
                    s_dal.Config.Clock = dt;
                    break;
                case "2":
                    Console.WriteLine("enter the TimeSpan (in format hh:mm:ss): ");
                    if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan ts)) throw new FormatException("TimeSpan is invalid!");
                    s_dal.Config.RiskRange = ts;
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;
            }
        } while (choice != "1" && choice != "2");
    }
    /// <summary>
    /// Print a certain time
    /// </summary>
    private static void ReadIconfig()
    {
        string choice;
        do { 
        Console.WriteLine("Enter 1 to read the clock or 2 to read the risk range ");
        choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.WriteLine(s_dal.Config.Clock);
                break;
            case "2":
                Console.WriteLine(s_dal.Config.RiskRange);
                break;
            default:
                Console.WriteLine("Invalid choice, please try again");
                break;
        }
        } while (choice != "1" && choice != "2");

    }
}

