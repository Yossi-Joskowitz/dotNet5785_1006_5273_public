using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;
using System;

using XamlHelpers;

using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    // Represents the BL (Business Logic) of the system
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// A string for the button text.
    /// </summary>
    public string ButtonText { get; set; } = "Add";

    /// <summary>
    /// The volunteer ID of the selcted volunteer or 0 if it is a new volunteer.
    /// </summary>
    private int VolunteerId { get; set; }

    /// <summary>
    /// The manager ID of the current manager that in the system.
    /// </summary>
    private int ManagerId { get; set; }

    /// <summary>
    /// DependencyProperty for determining if the password is visible.
    /// </summary>
    public static readonly DependencyProperty IsPasswordVisibleProperty =
        DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool IsPasswordVisible
    {
        get { return (bool)GetValue(IsPasswordVisibleProperty); }
        set { SetValue(IsPasswordVisibleProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the Volunteer object.
    /// </summary>
    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public BO.Volunteer Volunteer
    {
        get { return (BO.Volunteer)GetValue(VolunteerProperty); }
        set { SetValue(VolunteerProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the CallInProgres determining if there is a call in progress.
    /// </summary>
    public static readonly DependencyProperty CallInProgresProperty =
        DependencyProperty.Register("CallInProgres", typeof(bool), typeof(VolunteerWindow), new PropertyMetadata(false));

    public bool CallInProgres
    {
        get => (bool)GetValue(CallInProgresProperty);
        set => SetValue(CallInProgresProperty, value);
    }



    /// <summary>
    /// Constructor for VolunteerWindow.
    /// </summary>
    public VolunteerWindow(int managerId = 0, int id = 0)
    {
        ManagerId = managerId;
        VolunteerId = id;
        IsPasswordVisible = false;
        ButtonText = VolunteerId == 0 ? "Add" : "Update";
        InitializeComponent();
    }

    /// <summary>
    /// Opens the window for the manager to view the volunteer in the list or the one, based on the ID.
    /// And adding the observer to the volunteer.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (VolunteerId == 0)
            Volunteer = new BO.Volunteer();
        else
        {                      
            UpdateVolunteer();                        
            s_bl.Volunteer.AddObserver(VolunteerId, UpdateVolunteer);
            
        }

    }

    /// <summary>
    /// Removes the observer when the window is closed
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (VolunteerId != 0)
            s_bl.Volunteer.RemoveObserver(VolunteerId, UpdateVolunteer);
    }
    private volatile DispatcherOperation? _observerOperationForUpdateVolunteer = null;

    /// <summary>
    /// A method to update the volunteer object.
    /// </summary>
    private void UpdateVolunteer()
    {
        if (_observerOperationForUpdateVolunteer is null || _observerOperationForUpdateVolunteer.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateVolunteer = Dispatcher.BeginInvoke(() =>
            {

                try
                {
                    Volunteer = s_bl.Volunteer.Read(VolunteerId);
                    if (Volunteer.callInProgress != null)
                        CallInProgres = true;
                }
                catch (Exception e)
                {

                    if (e.Message == "The volunteer does not exist.")
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                    }
                }
            });
    }

    /// <summary>
    /// Handles the Add/Update button click.
    /// Adds or updates a volunteer based on the current state.
    /// </summary>
    private void AddOrUpdateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add")
                s_bl.Volunteer.Add(Volunteer);
            else
                s_bl.Volunteer.Update(ManagerId, Volunteer);
            MessageBox.Show("Passed successfully.");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        

    }
}