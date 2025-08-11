using BO;
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

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    /// <summary>
    /// A static instance of the VolunteerListWindow.
    /// </summary>
    private static VolunteerListWindow? _instance;

    /// <summary>
    /// Function to open the window. If it is already open, bring it to the front.
    /// </summary>
    public static void ShowWindow(int managerId)
    {
        if (_instance == null || !_instance.IsLoaded)
        {
            _instance = new VolunteerListWindow(managerId);
            _instance.Show();
        }
        else
        {
            _instance.Activate();
        }
    }

    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Currently selected volunteer.
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    /// <summary>
    /// Represents the selected filtering option for the volunteer list.
    /// </summary>
    public static bool? Active { get; set; } = null;

    /// <summary>
    /// Represents the selected sorting option for the volunteer list.
    /// </summary>
    public BO.TypeOfSortingVolunteer? Sort { get; set; } = null;

    /// <summary>
    /// The manager ID of the current manager that in the system.
    /// </summary>
    private int ManagerId { get; set; }

    /// <summary>
    /// Dependency property for the Volunteer in List.
    /// </summary>
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    /// <summary>
    /// The constructor for the VolunteerListWindow.
    /// </summary>
    public VolunteerListWindow(int managerId)
    {
        ManagerId = managerId;
        InitializeComponent();
    }

    /// <summary>
    /// Adds the observer and initializes the volunteer list when the window loads.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(QueryVolunteerList);
        QueryVolunteerList();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(QueryVolunteerList);
        _instance = null;
    }
    private volatile DispatcherOperation? _observerOperationForQueryVolunteerList = null;

    /// <summary>
    /// Queries the volunteer list based on the current filters and sorting options.
    /// </summary>
    private void QueryVolunteerList()
    {
        if (_observerOperationForQueryVolunteerList is null || _observerOperationForQueryVolunteerList.Status == DispatcherOperationStatus.Completed)
            _observerOperationForQueryVolunteerList = Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    VolunteerList = s_bl?.Volunteer.ReadVolunteers(Active, Sort)!;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
    }

    /// <summary>
    /// Updates the volunteer list based on the selected sorting option.
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        QueryVolunteerList();
    }

    /// <summary>
    /// Updates the filter (active, inactive, or all) based on the selected radio button.
    /// </summary>
    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {

        if (sender is not RadioButton radioButton || radioButton.Tag == null)
            return;

        Active = radioButton.Tag.ToString() switch
        {
            "Active" => true,
            "Inactive" => false,
            _ => null
        };

        QueryVolunteerList();
    }

    /// <summary>
    /// Opens the VolunteerWindow for the selected volunteer when double-clicked in update mode.
    /// </summary>
    private void VolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            var window = new VolunteerWindow(ManagerId, SelectedVolunteer.Id);
            window.Show();
        }
    }

    /// <summary>
    /// Opens a new VolunteerWindow to add a volunteer in add mode.
    /// </summary>
    private void AddVolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }

    /// <summary>
    /// Deletes the selected volunteer after user confirmation.
    /// </summary>
    private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        // Check if a volunteer is selected.
        if (SelectedVolunteer != null)
        {
            var result = MessageBox.Show($"Are you sure you want to delete the volunteer {SelectedVolunteer.FullName}?",
                                          "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            // Check if the user confirmed the deletion.
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.Delete(SelectedVolunteer.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}