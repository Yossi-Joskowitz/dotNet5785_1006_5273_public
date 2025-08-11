using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.MainVolunteer;

/// <summary>
/// Interaction logic for CallHistory.xaml
/// </summary>
public partial class CallHistory : Window
{
    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Represents the selected sorting option for the volunteer list.
    /// </summary>
    public BO.TypeOfSortingClosedCalls? Sort { get; set; } = null;

    /// <summary>
    /// Represents the selected filtering option for the volunteer list.
    /// </summary>
    public BO.CallType? Filter { get; set; } = null;

    /// <summary>
    /// The ID of the volunteer that is currently logged in.
    /// </summary>
    public int VolunteerId { get; set; }


    /// <summary>
    /// Dependency property for the Closed Calls.
    /// </summary>
    public static readonly DependencyProperty ClosedCallsProperty =
        DependencyProperty.Register("ClosedCalls", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallHistory), new PropertyMetadata(null));

    public IEnumerable<BO.ClosedCallInList> ClosedCalls
    {
        get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallsProperty); }
        set { SetValue(ClosedCallsProperty, value); }
    }

    // <summary>
    /// DependencyProperty for the Volunteer how is currently logged in.
    /// </summary>
    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(CallHistory), new PropertyMetadata(null));

    public BO.Volunteer Volunteer
    {
        get { return (BO.Volunteer)GetValue(VolunteerProperty); }
        set { SetValue(VolunteerProperty, value); }
    }


    /// <summary>
    /// Cunstructor for the CallHistory window.
    /// </summary>
    public CallHistory(int id)
    {
        VolunteerId = id;
        try
        {
            Volunteer = s_bl.Volunteer.Read(id);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        InitializeComponent();
    }

    /// <summary>
    /// Updating the list of Closed Calls when the window is loaded.
    /// And adding the observer to the volunteer.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerId, UpdateTheCalls);
        UpdateTheCalls();
    }

    /// <summary>
    /// Removing the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(VolunteerId, UpdateTheCalls);
    }

    private volatile DispatcherOperation? _observerOperationForUpdateTheCalls = null;
    /// <summary>
    /// Updating the closd Calls every time there is a change.
    /// </summary>
    public void UpdateTheCalls()
    {
        if (_observerOperationForUpdateTheCalls is null || _observerOperationForUpdateTheCalls.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateTheCalls = Dispatcher.BeginInvoke(() =>
            {

                try
                {
                    ClosedCalls = s_bl.Call.GetClosedCalls(VolunteerId, Filter, Sort);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            });
    }

    /// <summary>
    /// A function that gets the item to filter
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Filter == BO.CallType.None)
        {
            Filter = null;
        }
        UpdateTheCalls();
    }
}