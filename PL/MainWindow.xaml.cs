using BO;
using PL.Volunteer;
using PL.Call;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using DO;
using System.Windows.Threading;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// A static instance of the MainWindow.
    /// </summary>
    private static MainWindow? _instance;

    /// <summary>
    /// A function to open the window. If it is already open, bring it to the front.
    /// </summary>
    public static void ShowWindow(int id)
    {
        if (_instance == null || !_instance.IsLoaded)
        {
            _instance = new MainWindow(id);
            _instance.Show();
        }
        else
        {
            MessageBox.Show("There is already one administrator in the system.");
            _instance.Activate();
        }
    }

    // Represents the BL (Business Logic) of the system
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// The Id of the manager who opened the window.
    /// </summary>
    private int ManagerId { get; set; }

    /// <summary>
    /// DependencyProperty for the CurrentTime.
    /// </summary>
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the risk range in the system.
    /// </summary>
    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    /// <summary>
    /// Defines the DependencyProperty for Call status.
    /// </summary>
    public static readonly DependencyProperty CallStatusProperty =
    DependencyProperty.Register("CallStatus", typeof(ObservableCollection<KeyValuePair<BO.CallStatus, int>>), typeof(MainWindow),
        new PropertyMetadata(new ObservableCollection<KeyValuePair<BO.CallStatus, int>>()));

    /// <summary>
    /// Gets or sets the CallTypes property.
    /// Contains an ObservableCollection of KeyValuePair objects representing CallTypes and their counts.
    /// This property is bound to the UI to display the data dynamically.
    /// </summary>
    public ObservableCollection<KeyValuePair<BO.CallStatus, int>> CallStatus
    {
        get { return (ObservableCollection<KeyValuePair<BO.CallStatus, int>>)GetValue(CallStatusProperty); }
        set { SetValue(CallStatusProperty, value); }
    }

    /// <summary>
    /// DependecyProperty for the SelectedCallStatus from to list of calls groped by status.
    /// </summary>
    public static readonly DependencyProperty SelectedCallStatusProperty = DependencyProperty.Register(nameof(SelectedCallStatus), typeof(KeyValuePair<BO.CallStatus, int>?),
        typeof(MainWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public KeyValuePair<BO.CallStatus, int>? SelectedCallStatus
    {
        get => (KeyValuePair<BO.CallStatus, int>?)GetValue(SelectedCallStatusProperty);
        set => SetValue(SelectedCallStatusProperty, value);
    }


    /// <summary>
    /// The cunstructor for the MainWindow.
    /// </summary>
    public MainWindow(int id)
    {
        ManagerId = id;
        InitializeComponent();
    }

    /// <summary>
    /// Initializes observers and updates time and risk range when the window loads.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AddClockObserver(ClockObserver);
        s_bl.Admin.AddConfigObserver(ConfigObserver);
        s_bl.Call.AddObserver(UpdateCallStatus);
        RiskRange = s_bl.Admin.GetRiskTime();
        CurrentTime = s_bl.Admin.GetClock();
        UpdateCallStatus();
    }

    /// <summary>
    /// Removes observers when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        s_bl.Call.RemoveObserver(UpdateCallStatus);
        //Closing the simulator if it is running
        if (IsSimulatorRunning)
        {
            s_bl.Admin.StopSimulator();
            MessageBox.Show("The simulator has stopped.");
        }
    }

    private volatile DispatcherOperation? _observerOperationForUpdateCallStatus = null;
    private volatile DispatcherOperation? _observerOperationForClockObserver = null;
    private volatile DispatcherOperation? _observerOperationForConfigObserver = null;
    /// <summary>
    /// Updates the CallStatus collection with the latest data.
    /// Fetches the count of calls for each Callstatus and updates the ObservableCollection.
    /// This ensures that the UI remains in sync with the underlying data.
    /// </summary>
    private void UpdateCallStatus()
    {
        if (_observerOperationForUpdateCallStatus is null || _observerOperationForUpdateCallStatus.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateCallStatus = Dispatcher.BeginInvoke(() =>
            {

                int[] counts = s_bl.Call.CountCalls();
                var updatedCallTypes = Enum.GetValues(typeof(BO.CallStatus))
                    .Cast<BO.CallStatus>()
                    .Select((type, index) => new KeyValuePair<BO.CallStatus, int>(type, counts[index]))
                    .ToList();

                CallStatus.Clear();
                foreach (var item in updatedCallTypes)
                {
                    CallStatus.Add(item);
                }
            });
    }

    /// <summary>
    /// Observer for the clock to update the current time.
    /// </summary>
    private void ClockObserver()
    {
        if (_observerOperationForClockObserver is null || _observerOperationForClockObserver.Status == DispatcherOperationStatus.Completed)
            _observerOperationForClockObserver = Dispatcher.BeginInvoke(() =>
            {
                CurrentTime = s_bl.Admin.GetClock();
            });
    }

    /// <summary>
    /// Observer for the system configuration to update the risk range.
    /// </summary>
    private void ConfigObserver()
    {
        if (_observerOperationForConfigObserver is null || _observerOperationForConfigObserver.Status == DispatcherOperationStatus.Completed)
            _observerOperationForConfigObserver = Dispatcher.BeginInvoke(() =>
            {
                RiskRange = s_bl.Admin.GetRiskTime();
            });
    }

    /// <summary>
    /// Updates the risk range when the button is clicked.
    /// </summary>
    private void SetRiskRangeButten_Click(object sender, RoutedEventArgs e)
    {
        if (s_bl.Admin.GetRiskTime() != RiskRange)
        {
            MessageBox.Show("The risk range was updated successfully.");
            s_bl.Admin.SetRiskTime(RiskRange);
        }
        else
        {
            MessageBox.Show("Need to change before updating.");
        }
    }

    /// <summary>
    /// Advances the clock by a miunte.
    /// </summary>
    private void AddOneMinuteButten_Click(object sender, RoutedEventArgs e)
    {
        AddvancedClock(TypeOfTime.Minute);
    }

    /// <summary>
    /// Advances the clock by a Hour.
    /// </summary>
    private void AddOneHourButten_Click(object sender, RoutedEventArgs e)
    {
        AddvancedClock(TypeOfTime.Hour);
    }

    /// <summary>
    /// Advances the clock by a Day.
    /// </summary>
    private void AddOneDayButten_Click(object sender, RoutedEventArgs e)
    {
        AddvancedClock(TypeOfTime.Day);
    }

    /// <summary>
    /// Advances the clock by a Month.
    /// </summary>
    private void AddOneMonthButten_Click(object sender, RoutedEventArgs e)
    {
        AddvancedClock(TypeOfTime.Month);
    }

    /// <summary>
    /// Advances the clock by a Year.
    /// </summary>
    private void AddOneYearButten_Click(object sender, RoutedEventArgs e)
    {
        AddvancedClock(TypeOfTime.Year);
    }

    /// <summary>
    /// Advances the clock by specific time intervals when buttons are clicked.
    /// </summary>
    private void AddvancedClock(TypeOfTime type)
    {
        try
        {
            s_bl.Admin.AdvanceTheClock(type);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    /// <summary>
    /// Opens the volunteers in list window.
    /// </summary>
    private void HandleVolunteerInListButten_Click(object sender, RoutedEventArgs e)
    {
        VolunteerListWindow.ShowWindow(ManagerId);
    }

    /// <summary>
    /// Opens the calls in list window.
    /// </summary>
    private void HandleCallInListButten_Click(object sender, RoutedEventArgs e)
    {
        CallListWindow.ShowWindow(ManagerId);
    }

    /// <summary>
    /// Initializes the database after confirming the user's choice.
    /// </summary>
    private void InitializeDBButten_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            //Cursor changes to wait
            Mouse.OverrideCursor = Cursors.Wait;
            //Close all windows except the main window
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this && window is not loginScreen)
                {
                    window.Close();
                }
            }
            try
            {
                s_bl.Admin.AdminInitialize();
                MessageBox.Show("The system has successfully booted.", "success", MessageBoxButton.OK,
                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Cursor changes back to normal
            Mouse.OverrideCursor = null;



        }
    }

    /// <summary>
    /// Resets the database after confirming the user's choice.
    /// </summary>
    private void ResetDBButten_Click(object sender, RoutedEventArgs e)
    {

        var result = MessageBox.Show("Are you sure you want to reset the datebase?", "confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            //Cursor changes to wait
            Mouse.OverrideCursor = Cursors.Wait;
            //Close all windows except the main window
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this && window is not loginScreen)
                {
                    window.Close();
                }
            }
            try
            {
                s_bl.Admin.AdminReset();
                MessageBox.Show("System reset successfully.", "success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Cursor changes back to normal
            Mouse.OverrideCursor = null;
        }
    }



    /// <summary>
    /// A function that opens the CallListWindow for the selected status.
    /// </summary>
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            SelectedCallStatus = (KeyValuePair<BO.CallStatus, int>)e.AddedItems[0];
            BO.CallStatus key = SelectedCallStatus.Value.Key;
            CallListWindow.ShowWindow(ManagerId, key);
            SelectedCallStatus = null;
        }
    }

    /// <summary>
    /// DependencyProperty for the nimutes for the simulator.
    /// </summary>
    public static readonly DependencyProperty NumberOfMinutesProperty =
        DependencyProperty.Register("NumberOfMinutes", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

    public int NumberOfMinutes
    {
        get { return (int)GetValue(NumberOfMinutesProperty); }
        set { SetValue(NumberOfMinutesProperty, value); }
    }

    /// <summary>
    /// DependencyProperty that checks is the simulator is ronning.
    /// </summary>
    public static readonly DependencyProperty IsSimulatorRunningProperty =
        DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

    public bool IsSimulatorRunning
    {
        get { return (bool)GetValue(IsSimulatorRunningProperty); }
        set { SetValue(IsSimulatorRunningProperty, value); }
    }

    /// <summary>
    /// DependencyProperty the status of the simulator.
    /// </summary>
    public static readonly DependencyProperty SimulatorStatusProperty =
        DependencyProperty.Register("SimulatorStatus", typeof(string), typeof(MainWindow), new PropertyMetadata("Start Simulator"));

    public string SimulatorStatus
    {
        get { return (string)GetValue(SimulatorStatusProperty); }
        set { SetValue(SimulatorStatusProperty, value); }
    }

    /// <summary>
    /// The button that turns the simulator on and off.
    /// </summary>
    private void StartOrStopSimulatorButten_Click(object sender, RoutedEventArgs e)
    {

        if (!IsSimulatorRunning)
        {
            IsSimulatorRunning = true;
            s_bl.Admin.StartSimulator(NumberOfMinutes);
            SimulatorStatus = "Stop Simulator";
            MessageBox.Show("The simulator has started.");
        }
        else
        {
            s_bl.Admin.StopSimulator();
            Mouse.OverrideCursor = Cursors.Wait;
            s_bl.Admin.IsSimulatorIsRonning();
            Mouse.OverrideCursor = null;
            IsSimulatorRunning = false;
            SimulatorStatus = "Start Simulator";
            MessageBox.Show("The simulator has stopped.");
        }
    }



}