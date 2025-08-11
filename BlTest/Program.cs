
internal class Program
{
    private enum CoiceForMenu { Exit, Admin, Volunteer, Call}   
    private enum CoiceForAdminSubMenu { Exit, Advances, DisplayClock, DisplayTimeSpan, SetTimeSpan, Initialization, DisplayAll, Reset }
    private enum CoiceForVolunteerSubMenu { Exit, Add, Delete, GetRole, Read, ReadAll, Update }
    private enum CoiceForCallSubMenu
    {
        Exit, Add, ChooseForTreatment, Count, Delete, GetCalls, GetClosed,
        GetOpen, Read, Update, UpdateCancel, UpdateEnd
    }   
    private enum TimeUnit { Minute, Hour, Day, Month, Year }

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        try
        {
            CoiceForMenu choice;
            do
            {
                Console.WriteLine("Enter 0. to Exit the Main Menu:");
                Console.WriteLine("Enter 1. to see Admin Menu:");
                Console.WriteLine("Enter 2. to see Volunteer Menu:");
                Console.WriteLine("Enter 3. to see Call Menu:");
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
                            case CoiceForMenu.Admin:
                                AdminSubMenu();
                                break;
                            case CoiceForMenu.Volunteer:
                                VolunteerSubMenu();
                                break;
                            case CoiceForMenu.Call:
                                CallSubMenu();
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
        catch (Exception ex)
        {

            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// A sub-menu for a configuration entity
    /// </summary>
    private static void AdminSubMenu()
    {
        CoiceForAdminSubMenu choice;
        do
        {
            Console.WriteLine("Enter 0. to Exit the Admin sub Menu:");
            Console.WriteLine("Enter 1. to Advances the clock:");
            Console.WriteLine("Enter 2. to Display the current clock:");
            Console.WriteLine("Enter 3. to Display the risk time:");
            Console.WriteLine("Enter 4. to Set the risk time:");
            Console.WriteLine("Enter 5. to initializ the data:");
            Console.WriteLine("Enter 6. to Display all the data in the database:");
            Console.WriteLine("Enter 7. to Reset the Database and configuration data :");
            Console.WriteLine("Enter your choice:");

            if (Enum.TryParse(Console.ReadLine(), out choice))
            {
                try
                {
                    switch (choice)
                    {
                        case CoiceForAdminSubMenu.Exit:
                            return;
                        case CoiceForAdminSubMenu.Advances:
                            AdvanceClock();
                            break;
                        case CoiceForAdminSubMenu.DisplayClock:
                            Console.WriteLine(s_bl.Admin.GetClock());
                            break;
                        case CoiceForAdminSubMenu.DisplayTimeSpan:
                            Console.WriteLine(s_bl.Admin.GetRiskTime());
                            break;
                        case CoiceForAdminSubMenu.SetTimeSpan:
                            SetTheRiskRange();
                            break;
                        case CoiceForAdminSubMenu.Initialization:
                            s_bl.Admin.AdminInitialize();
                            break;
                        case CoiceForAdminSubMenu.DisplayAll:
                            s_bl.Admin.ReadAll();
                            break;
                        case CoiceForAdminSubMenu.Reset:
                            s_bl.Admin.AdminReset();
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
        } while (choice != CoiceForAdminSubMenu.Exit);
    }

    /// <summary>
    /// Submenu for volunteer entity
    /// </summary>
    private static void VolunteerSubMenu()
    {
        CoiceForVolunteerSubMenu choice;
        do
        {
            Console.WriteLine("Enter 0. to Exit the volunteer sub Menu:");
            Console.WriteLine("Enter 1. to add a Volunteer:");
            Console.WriteLine("Enter 2. to delete a volunteer:");
            Console.WriteLine("Enter 3. to get the type of the volunteer:");
            Console.WriteLine("Enter 4. to read a volunteer:");
            Console.WriteLine("Enter 5. to read all volunteers in list :");
            Console.WriteLine("Enter 6. to update a volunteer :");
            Console.WriteLine("Enter your choice:");

            if (Enum.TryParse(Console.ReadLine(), out choice))
            {
                try
                {
                    switch (choice)
                    {
                        case CoiceForVolunteerSubMenu.Exit:
                            return;
                        case CoiceForVolunteerSubMenu.Add:
                            AddVolunteer();
                            break;
                        case CoiceForVolunteerSubMenu.Delete:
                            Console.WriteLine("Enter the volunteer's id:");
                            s_bl.Volunteer.Delete(int.Parse(Console.ReadLine()));
                            break;
                        case CoiceForVolunteerSubMenu.GetRole:
                            Console.WriteLine("Enter the volunteer's name and paasword:");
                            Console.WriteLine(s_bl.Volunteer.LoginReturnType(Console.ReadLine(), Console.ReadLine()));
                            break;
                        case CoiceForVolunteerSubMenu.Read:
                            Console.WriteLine("Enter the volunteer's id:");
                            Console.WriteLine(s_bl.Volunteer.Read(int.Parse(Console.ReadLine())));
                            break;
                        case CoiceForVolunteerSubMenu.ReadAll:
                            ReadAllVolunteer();
                            break;
                        case CoiceForVolunteerSubMenu.Update:
                            UpdateVolunteer();
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


        } while (choice != CoiceForVolunteerSubMenu.Exit);
    }
    /// <summary>
    /// A sub-menu for a calling entity
    /// </summary>
    private static void CallSubMenu()
    {
        CoiceForCallSubMenu choice;
        do
        {
            Console.WriteLine("Enter 0. to exit the call sub Menu: ");
            Console.WriteLine("Enter 1. to add the call: ");
            Console.WriteLine("Enter 2. to choosing a call for treatment: ");
            Console.WriteLine("Enter 3. to count calls: ");
            Console.WriteLine("Enter 4. to delete call: ");
            Console.WriteLine("Enter 5. to get calls: ");
            Console.WriteLine("Enter 6. to get closed calls: ");
            Console.WriteLine("Enter 7. to get open calls: ");
            Console.WriteLine("Enter 8. to read call: ");
            Console.WriteLine("Enter 9. to update call: ");
            Console.WriteLine("Enter 10. to update cancel iation of treatment: ");
            Console.WriteLine("Enter 11. to update end of treatment: ");

            Console.WriteLine("Enter your choice:");
            if (Enum.TryParse(Console.ReadLine(), out choice))
            {
                try
                {
                    switch (choice)
                    {
                        case CoiceForCallSubMenu.Exit:
                            return;
                        case CoiceForCallSubMenu.Add:
                            AddCall();
                            break;
                        case CoiceForCallSubMenu.ChooseForTreatment:
                            Console.WriteLine("Enter volunteer id and call id:");
                            s_bl.Call.ChoosingACallForTreatment(int.Parse(Console.ReadLine()), int.Parse(Console.ReadLine()));
                            break;
                        case CoiceForCallSubMenu.Count:
                            int[] arr = s_bl.Call.CountCalls();
                            Console.WriteLine(string.Join(", ", arr));
                            break;
                        case CoiceForCallSubMenu.Delete:
                            Console.WriteLine("Enter the call's id:");
                            s_bl.Call.Delete(int.Parse(Console.ReadLine()));
                            break;
                        case CoiceForCallSubMenu.GetCalls:
                            GetAllCalls();
                            break;
                        case CoiceForCallSubMenu.GetClosed:
                            GetAllClosedOrOpenCalls(1);
                            break;
                        case CoiceForCallSubMenu.GetOpen:
                            GetAllClosedOrOpenCalls(2);
                            break;
                        case CoiceForCallSubMenu.Read:
                            Console.WriteLine("Enter the call's id:");
                            Console.WriteLine(s_bl.Call.Read(int.Parse(Console.ReadLine())));
                            break;
                        case CoiceForCallSubMenu.Update:
                            UpdateCall();
                            break;
                        case CoiceForCallSubMenu.UpdateCancel:
                            Console.WriteLine("Enter the volunteer's id:");
                            int volunteerIdCancel = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the assignment's id:");
                            int assignmentIdCancel = int.Parse(Console.ReadLine());
                            s_bl.Call.UpdateCancellationOfTreatment(volunteerIdCancel, assignmentIdCancel);
                            break;
                        case CoiceForCallSubMenu.UpdateEnd:
                            Console.WriteLine("Enter the volunteer's id:");
                            int volunteerIdEnd = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter the assignment's id:");
                            int assignmentIdEnd = int.Parse(Console.ReadLine());
                            s_bl.Call.UpdateEndOfTreatment(volunteerIdEnd, assignmentIdEnd);
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

        } while (choice != CoiceForCallSubMenu.Exit);
    }

    /// <summary>
    /// A function for updating volunteer details
    /// </summary>
    private static void UpdateVolunteer()
    {
        Console.WriteLine("Enter the ID of the person requesting the update.:");
        int requesId = int.Parse(Console.ReadLine());




        Console.WriteLine("Enter the Volunteer ID:");
        int id = int.Parse(Console.ReadLine());
        BO.Volunteer volunteer = s_bl.Volunteer.Read(id);
        if (volunteer is not null)
        {
            Console.WriteLine(volunteer);
            Console.WriteLine("Enter the Volunteer Name or Enter to skip:");
            string tempName = Console.ReadLine();
            string name = tempName == "" ? volunteer.FullName : tempName;
            Console.WriteLine("Enter the Volunteer Phone or Enter to skip:");
            string tempPhone = Console.ReadLine();
            string phone = tempPhone == "" ? volunteer.PhoneNumber : tempPhone;
            Console.WriteLine("Enter the Volunteer Address or Enter to skip:");
            string tempAddress = Console.ReadLine();
            string address = tempAddress == "" ? volunteer.FullAddress : tempAddress;
            Console.WriteLine("Enter the Volunteer Email or Enter to skip:");
            string tempMail = Console.ReadLine();
            string mail = tempMail == "" ? volunteer.Email : tempMail;
            Console.WriteLine("Enter the Volunteer Role:  1. for Manager, 2. for Volunteer , or Enter to skip");
            string temprole = Console.ReadLine();
            int role = temprole == "" ? (int)volunteer.RoleType : int.Parse(temprole);
            Console.WriteLine("Enter the status: 1. Active 2, Inactive , or Enter to skip");
            string tempstatus = Console.ReadLine();
            bool status = tempstatus == "" ? volunteer.Active : int.Parse(tempstatus) == 1 ? true : false;
            Console.WriteLine("Enter the distance type: 1. for Aerial, 2. for Walking, 3. for Driving, or Enter to skip");
            string tempTypeDistance = Console.ReadLine();
            int typeDistance = tempTypeDistance == "" ? (int)volunteer.DistanceType : int.Parse(tempTypeDistance) - 1;
            Console.WriteLine("Enter your new Password or Enter to skip:");
            string tempPassword = Console.ReadLine();
            string password = tempPassword == "" ? volunteer.Password : tempPassword;
            Console.WriteLine("Enter your new max distance or Enter to skip:");
            string tempMaxDistance = Console.ReadLine();
            double? maxDistance = tempMaxDistance == "" ? volunteer.MaxDistance : double.Parse(tempMaxDistance);
            s_bl.Volunteer.Update(requesId, new BO.Volunteer()
            {
                Id = id,
                FullName = name,
                PhoneNumber = phone,
                Email = mail,
                FullAddress = address,
                RoleType = (BO.Role)role,
                Active = status,
                DistanceType = (BO.Distance)typeDistance,
                Password = password,
                MaxDistance = maxDistance
            });
        }
        else
        {
            Console.WriteLine("Volunteer not found.");
        }
    }

    /// <summary>
    /// A function to call all volunteers
    /// </summary>
    /// <exception cref="BO.BlAttemptToPerformAnIllegalActionException"></exception>
    private static void ReadAllVolunteer()
    {
        Console.WriteLine("Enter 0 for everyone 1 for active 2 for inactive.");
        int activeChoich = int.Parse(Console.ReadLine());
        bool? active = activeChoich switch
        {
            0 => null,
            1 => true,
            2 => false,
            _ => throw new BO.BlAttemptToPerformAnIllegalActionException("invalid choich.")
        };


        Console.WriteLine("Enter what you want to sort by.");
        Console.WriteLine("Enter 0. to sort by Id.");
        Console.WriteLine("Enter 1. to sort by Name.");
        Console.WriteLine("Enter 2. to sort by Number Of Calls That Treated.");
        Console.WriteLine("Enter 3. to sort by Number Of Calls That Self Canceled.");
        Console.WriteLine("Enter 4. to sort by Number Of Calls Taken And Expired.");
        int type = int.Parse(Console.ReadLine());

        BO.TypeOfSortingVolunteer typeToSOrt = type switch
        {
            0 => BO.TypeOfSortingVolunteer.Id,
            1 => BO.TypeOfSortingVolunteer.Name,
            2 => BO.TypeOfSortingVolunteer.NumberOfCallsThatTreated,
            3 => BO.TypeOfSortingVolunteer.NumberOfCallsThatSelfCanceled,
            4 => BO.TypeOfSortingVolunteer.NumberOfCallsTakenAndExpired,
            _ => throw new BO.BlAttemptToPerformAnIllegalActionException("invalid choich.")
        };

        foreach (BO.VolunteerInList volunteer in s_bl.Volunteer.ReadVolunteers(active, typeToSOrt))
        {
            Console.WriteLine(volunteer);
        }
    }

    /// <summary>
    /// Function to add a volunteer
    /// </summary>
    private static void AddVolunteer()
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
        int distance = int.Parse(Console.ReadLine()) - 1;
        Console.WriteLine("Enter your new Password:");
        string password = Console.ReadLine();
        Console.WriteLine("Enter your new max distance or Enter to skip:");
        string tempMaxDistance = Console.ReadLine();
        double? maxDistance = tempMaxDistance == "" ? null : double.Parse(tempMaxDistance);
        s_bl.Volunteer.Add(new BO.Volunteer()
        {
            Id = id,
            FullName = name,
            PhoneNumber = phone,
            Email = mail
        ,
            FullAddress = address,
            RoleType = (BO.Role)role,
            Active = status,
            DistanceType = (BO.Distance)distance,
            Password = password,
            MaxDistance = maxDistance
        });
    }

    /// <summary>
    /// Updates the risk range
    /// </summary>
    private static void SetTheRiskRange()
    {
        try
        {
            Console.WriteLine("Enter the time span (e.g., HH:MM, D.HH:MM, HH:MM:SS):");
            s_bl.Admin.SetRiskTime(TimeSpan.Parse(Console.ReadLine()));
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid time format.");
        }
    }

    /// <summary>
    /// A clock advance function asks what kind of time you want to advance and advances
    /// </summary>
    private static void AdvanceClock()
    {
        TimeUnit choice;
        Console.WriteLine("Enter the time unit you want to move forward:");
        Console.WriteLine("0 - Minute");
        Console.WriteLine("1 - Hour");
        Console.WriteLine("2 - Day");
        Console.WriteLine("3 - Month");
        Console.WriteLine("4 - Year");

        if (Enum.TryParse(Console.ReadLine(), out choice))
        {
            switch (choice)
            {
                case TimeUnit.Minute:
                    s_bl.Admin.AdvanceTheClock(BO.TypeOfTime.Minute);
                    break;
                case TimeUnit.Hour:
                    s_bl.Admin.AdvanceTheClock(BO.TypeOfTime.Hour);
                    break;
                case TimeUnit.Day:
                    s_bl.Admin.AdvanceTheClock(BO.TypeOfTime.Day);
                    break;
                case TimeUnit.Month:
                    s_bl.Admin.AdvanceTheClock(BO.TypeOfTime.Month);
                    break;
                case TimeUnit.Year:
                    s_bl.Admin.AdvanceTheClock(BO.TypeOfTime.Year);
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;
            }
        }
        else
        {
            Console.WriteLine("Invalid choice, please try again");
        }
    }
    

    /// <summary>
    /// A function to add a call
    /// </summary>
    private static void AddCall()
    {
        Console.WriteLine("Enter the Call ID:");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the Call Address:");
        string address = Console.ReadLine();
        BO.CallType tempType = SelectCallType();
        Console.WriteLine("Enter your new Verbal description to call or Enter to skip:");
        string DescriptionTemp = Console.ReadLine();
        string? Description = DescriptionTemp != "" ? DescriptionTemp : null;
        Console.WriteLine("Enter your new MaxTime or Enter to skip:");
        string tempMax = Console.ReadLine();
        DateTime? Max = tempMax == "" ? null : DateTime.Parse(tempMax);

        s_bl.Call.Add(new BO.Call()
        {
            Id = id,
            FullAddress = address,
            Type = tempType,
            VerbalDescription = Description,
            MaxTime = Max,
            OpeningTime = s_bl.Admin.GetClock()
        });
    }

    /// <summary>
    /// A function for selecting a call type
    /// </summary>
    /// <returns></returns>
    private static BO.CallType SelectCallType()
    {
        BO.CallType callType;
        int type;
        do
        {
            Console.WriteLine(@"
Enter the status:
1. FoodDelivery,
2. EquipmentDelivery,
3. EquipmentRepair,
4. Rides,
5. Laundry,
6. StorageOfBelongings,
7. MedicalHelp,
8. EventOrganization,
9. PsychologicalAssistance,
10. MotivationalTalks,
11. None ");
            type = int.Parse(Console.ReadLine());
            return callType = type switch
            {
                1 => BO.CallType.FoodDelivery,
                2 => BO.CallType.EquipmentDelivery,
                3 => BO.CallType.EquipmentRepair,
                4 => BO.CallType.Rides,
                5 => BO.CallType.Laundry,
                6 => BO.CallType.StorageOfBelongings,
                7 => BO.CallType.MedicalHelp,
                8 => BO.CallType.EventOrganization,
                9 => BO.CallType.PsychologicalAssistance,
                10 => BO.CallType.MotivationalTalks,
                11 => BO.CallType.None,
            };
        } while (type < 1 || type > 11);
    }

    /// <summary>
    /// A function to return all calls according to a certain filtering and sorting
    /// </summary>
    /// <exception cref="BO.BlAttemptToPerformAnIllegalActionException"></exception>
    private static void GetAllCalls()
    {
        IEnumerable<BO.CallInList> callInList;
        Console.WriteLine("Enter a value to sort, 1 by AssignmateId 2 by CallId 3 by OpeningTime 4 by NumberOfAssignmates 5 by TotalTreatmentTime or enter to continue");
        string temp = Console.ReadLine();
        int? sorting = temp == "" ? null : int.Parse(temp);

        BO.TypeOfSortingCall sort = sorting switch
        {
            1 => BO.TypeOfSortingCall.AssignmateId,
            2 => BO.TypeOfSortingCall.CallId,
            3 => BO.TypeOfSortingCall.OpeningTime,
            4 => BO.TypeOfSortingCall.NumberOfAssignmates,
            5 => BO.TypeOfSortingCall.TotalTreatmentTime,
            _ => BO.TypeOfSortingCall.CallId //default value
        };

        Console.WriteLine("Enter a filter value 0 for no filter 1 for status and 2 for type");
        int Filter = int.Parse(Console.ReadLine());
        BO.TypeOfFiltering? typeToFilter;

        switch (Filter)
        {
            case 0:

                callInList = s_bl.Call.GetCalls(null, null, sort);
                foreach (var item in callInList)
                {
                    Console.WriteLine(item);
                }
                break;
            case 1:
                typeToFilter = BO.TypeOfFiltering.Status;
                Console.WriteLine("Enter the status: 1. Open 2. InTreatment 3. Close 4. Expired 5. OpenAtRisk 6. RiskTreatment");
                int status = int.Parse(Console.ReadLine());
                BO.CallStatus? callStatus = status switch
                {
                    1 => BO.CallStatus.Open,
                    2 => BO.CallStatus.InTreatment,
                    3 => BO.CallStatus.Close,
                    4 => BO.CallStatus.Expired,
                    5 => BO.CallStatus.OpenAtRisk,
                    6 => BO.CallStatus.RiskTreatment,
                    _ => throw new BO.BlAttemptToPerformAnIllegalActionException("invalid choich.")
                };
                callInList = s_bl.Call.GetCalls(typeToFilter, callStatus, sort);
                foreach (var item in callInList)
                {
                    Console.WriteLine(item);
                }
                break;
            case 2:
                typeToFilter = BO.TypeOfFiltering.Type;
                Console.WriteLine("Enter the type: 1. FoodDelivery 2. EquipmentDelivery 3. EquipmentRepair 4. Rides ");
                Console.WriteLine("5. Laundry 6. StorageOfBelongings 7. MedicalHelp 8. EventOrganization 9. PsychologicalAssistance 10. MotivationalTalks 11. None");
                int type = int.Parse(Console.ReadLine());
                BO.CallType? callType = type switch
                {
                    1 => BO.CallType.FoodDelivery,
                    2 => BO.CallType.EquipmentDelivery,
                    3 => BO.CallType.EquipmentRepair,
                    4 => BO.CallType.Rides,
                    5 => BO.CallType.Laundry,
                    6 => BO.CallType.StorageOfBelongings,
                    7 => BO.CallType.MedicalHelp,
                    8 => BO.CallType.EventOrganization,
                    9 => BO.CallType.PsychologicalAssistance,
                    10 => BO.CallType.MotivationalTalks,
                    11 => null,
                    _ => throw new BO.BlAttemptToPerformAnIllegalActionException("invalid choich.")
                };
                callInList = s_bl.Call.GetCalls(typeToFilter, callType, sort);
                foreach (var item in callInList)
                {
                    Console.WriteLine(item);
                }
                break;
            default:
                Console.WriteLine("Invalid choice, please try again");
                break;
        }
    }

    /// <summary>
    /// A function to return all closed or open calls according to a certain filtering and sorting
    /// </summary>
    /// <param name="ClosedOrOpen"> A helper variable that tells me whether to select the open or closed calls</param>
    /// <exception cref="BO.BlAttemptToPerformAnIllegalActionException"></exception>
    private static void GetAllClosedOrOpenCalls(int ClosedOrOpen)
    {
        Console.WriteLine("Enter volunteer id");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine("Enter the type: 1. FoodDelivery 2. EquipmentDelivery 3. EquipmentRepair 4. Rides ");
        Console.WriteLine("5. Laundry 6. StorageOfBelongings 7. MedicalHelp 8. EventOrganization 9. PsychologicalAssistance 10. MotivationalTalks 11. None");
        int type = int.Parse(Console.ReadLine());
        BO.CallType? callType = type switch
        {
            1 => BO.CallType.FoodDelivery,
            2 => BO.CallType.EquipmentDelivery,
            3 => BO.CallType.EquipmentRepair,
            4 => BO.CallType.Rides,
            5 => BO.CallType.Laundry,
            6 => BO.CallType.StorageOfBelongings,
            7 => BO.CallType.MedicalHelp,
            8 => BO.CallType.EventOrganization,
            9 => BO.CallType.PsychologicalAssistance,
            10 => BO.CallType.MotivationalTalks,
            11 => null,
            _ => throw new BO.BlAttemptToPerformAnIllegalActionException("invalid choich.")
        };

        if (ClosedOrOpen == 1)
        {
            Console.WriteLine("Enter number to sort by - 0. to skip, 1. CallId, 2. OpeningTime, 3. EnteringTime, 4. EndOfTime, ");
            int sort = int.Parse(Console.ReadLine());
            BO.TypeOfSortingClosedCalls typeOfSorting = sort switch
            {
                1 => BO.TypeOfSortingClosedCalls.CallId,
                2 => BO.TypeOfSortingClosedCalls.OpeningTime,
                3 => BO.TypeOfSortingClosedCalls.EnteringTime,
                4 => BO.TypeOfSortingClosedCalls.EndOfTime,
                _ => BO.TypeOfSortingClosedCalls.CallId
            };
            IEnumerable<BO.ClosedCallInList> closedCallInLists1 = s_bl.Call.GetClosedCalls(id, callType, typeOfSorting);
            foreach (var item in closedCallInLists1)
            {
                Console.WriteLine(item);
            }
        }
        else if (ClosedOrOpen == 2)
        {
            Console.WriteLine("Enter number to sort by - 0. to skip, 1. CallId, 2. OpeningTime, 3. DistanceFromVolunteer, ");
            int sort = int.Parse(Console.ReadLine());
            BO.TypeOfSortingOpenCalls typeOfSorting = sort switch
            {
                1 => BO.TypeOfSortingOpenCalls.CallId,
                2 => BO.TypeOfSortingOpenCalls.OpeningTime,
                3 => BO.TypeOfSortingOpenCalls.DistanceFromVolunteer,
                _ => BO.TypeOfSortingOpenCalls.CallId
            };
            IEnumerable<BO.OpenCallInList> openCallInLists = s_bl.Call.GetOpenCalls(id, callType, typeOfSorting);
            foreach (var item in openCallInLists)
            {
                Console.WriteLine(item);
            }
        }
    }


    /// <summary>
    /// Call update
    /// </summary>
    private static void UpdateCall()
    {
        Console.WriteLine("Enter the Call ID:");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine(s_bl.Call.Read(id));

        Console.WriteLine("Enter the Call Address:");
        string address = Console.ReadLine();
        BO.CallType tempType = SelectCallType();
        Console.WriteLine("Enter your new Verbal description to call or Enter to skip:");
        string DescriptionTemp = Console.ReadLine();
        string? Description = DescriptionTemp != "" ? DescriptionTemp : null;
        Console.WriteLine("Enter your new MaxTime or Enter to skip:");
        string tempMax = Console.ReadLine();
        DateTime? Max = tempMax == "" ? null : DateTime.Parse(tempMax);

        BO.Call call = new BO.Call()
        {
            Id = id,
            FullAddress = address,
            Type = tempType,
            VerbalDescription = Description,
            MaxTime = Max,
            OpeningTime = s_bl.Admin.GetClock()
        };
        s_bl.Call.Update(call);
    }
}


























