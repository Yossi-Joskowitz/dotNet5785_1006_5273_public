using BO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallListWindow.xaml
/// </summary>
public partial class CallListWindow : Window
{
    /// <summary>
    /// A static instance of the CallListWindow.
    /// </summary>
    private static CallListWindow? _instance;

    /// <summary>
    /// A function to open the window. If it is already open, bring it to the front.
    /// </summary>
    public static void ShowWindow(int id, BO.CallStatus? callStatus = null)
    {
        if (_instance == null || !_instance.IsLoaded)
        {
            _instance = new CallListWindow(id, callStatus);
            _instance.Show();
        }
        else
        {
            if((BO.CallStatus?)CallFilterObject != callStatus)
            {
                Filter = BO.TypeOfFiltering.Status;
                CallFilterObject = callStatus;
                _instance.UpdateTheCallsInList();
                _instance.Activate();
            }
            else
            {
                _instance.Activate();
            }
        }
    }

    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// The Id of the manager that is currently logged in.
    /// </summary>
    private int ManagerId { get; set; }

    /// <summary>
    /// The currently selected call in the list.
    /// </summary>
    public BO.CallInList? SelectedCall { get; set; }

    /// <summary>
    /// Represents the selected sorting option for the calls in the list.
    /// </summary>
    public BO.TypeOfSortingCall? Sort { get; set; } = null;

    /// <summary>
    /// Represents the selected filtering option for the calls in the list.
    /// </summary>
    public static BO.TypeOfFiltering? Filter { get; set; } = null;

    /// <summary>
    /// The object to filter the calls by.
    /// </summary>
    public static Object? CallFilterObject { get; set; } = null;

    /// <summary>
    /// Dependency property for the CallList.
    /// </summary>
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    /// <summary>
    /// The cunsructor for the CallListWindow.
    /// </summary>
    public CallListWindow(int managerid, BO.CallStatus? callStatus)
    {
        if (callStatus != null)
        {
            Filter = BO.TypeOfFiltering.Status;
            CallFilterObject = callStatus;
        }
        ManagerId = managerid;
        InitializeComponent();
    }

    /// <summary>
    /// Adds the observer and initializes the call list when the window loads.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(UpdateTheCallsInList);
        UpdateTheCallsInList();
    }

   
    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(UpdateTheCallsInList);
        _instance = null;
        Filter = null;
        CallFilterObject = null;
    }

    private volatile DispatcherOperation? _observerOperationForUpdateTheCallsInList = null;
    /// <summary>
    /// Queries the Call list based on the current filters and sorting options.
    /// </summary>
    private void UpdateTheCallsInList()
    {
        if (_observerOperationForUpdateTheCallsInList is null || _observerOperationForUpdateTheCallsInList.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateTheCallsInList = Dispatcher.BeginInvoke(() =>
            {

                try
                {
                    CallList = s_bl?.Call.GetCalls(Filter, CallFilterObject, Sort)!;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
    }

    /// <summary>
    /// Updates the list of calls when the sorting option is changed.
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateTheCallsInList();
    }

    /// <summary>
    /// Updates the list of calls when the filtering option is changed.
    /// The Filter can change or toa status type or to a call type.
    /// </summary>
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem selectedMenuItem)
        {
            if (Enum.TryParse(selectedMenuItem.Tag.ToString(), out BO.CallStatus status))
            {
                Filter = BO.TypeOfFiltering.Status;
                CallFilterObject = status;
                UpdateTheCallsInList();
            }
            else if (Enum.TryParse(selectedMenuItem.Tag.ToString(), out BO.CallType type))
            {
                Filter = BO.TypeOfFiltering.Type;
                CallFilterObject = type;
                UpdateTheCallsInList();
            }
            else
            {
                Filter = null;
                CallFilterObject = null;
                UpdateTheCallsInList();
            }
        }
    }

    /// <summary>
    /// A function that handles the double click on a call in the list.
    /// The function opens the CallWindow for the selected call.
    /// </summary>
    private void CallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
        {
            var callWindow = new CallWindow(SelectedCall.CallId);
            callWindow.Show();
        }
    }


    /// <summary>
    /// Deletes the selected call from the list.
    /// </summary>
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {

        // Check if a volunteer is selected.
        if (SelectedCall != null)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the call {SelectedCall.CallId}?",
                                          "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            // Check if the user confirmed the deletion.
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.Delete(SelectedCall.CallId);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// A function that opens the CallWindow in add mode.
    /// </summary>
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    /// <summary>
    /// A function that handles the cancellation of a call for treatment for the selected call.
    /// The cancellation is done by the manager.
    /// </summary>
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.UpdateCancellationOfTreatment(ManagerId, (int)SelectedCall.AssignmateId!);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}

