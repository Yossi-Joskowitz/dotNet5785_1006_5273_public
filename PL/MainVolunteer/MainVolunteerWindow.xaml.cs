using PL.Call;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
/// The code-behind of a volunteer's main window
/// </summary>
public partial class MainVolunteerWindow : Window
{
    // Represents the BL (Business Logic) of the system
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// The ID of the volunteer
    /// </summary>
    public int VolunteerId { get; set; }

    /// <summary>
    /// DependencyProperty for determining if the password is visible.
    /// </summary>
    public static readonly DependencyProperty IsPasswordVisibleProperty =
        DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(MainVolunteerWindow), new PropertyMetadata(false));

    public bool IsPasswordVisible
    {
        get { return (bool)GetValue(IsPasswordVisibleProperty); }
        set { SetValue(IsPasswordVisibleProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the Volunteer that in the system.
    /// </summary>
    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    public BO.Volunteer Volunteer
    {
        get { return (BO.Volunteer)GetValue(VolunteerProperty); }
        set { SetValue(VolunteerProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the Call in progres object.
    /// </summary>
    public static readonly DependencyProperty CallProperty =
        DependencyProperty.Register("Call", typeof(BO.Call), typeof(MainVolunteerWindow), new PropertyMetadata(null));

    public BO.Call Call
    {
        get { return (BO.Call)GetValue(CallProperty); }
        set { SetValue(CallProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for determining if there a Call In Progres for the volunteer.
    /// </summary>
    public static readonly DependencyProperty CallInProgresProperty =
        DependencyProperty.Register("CallInProgres", typeof(bool), typeof(MainVolunteerWindow), new PropertyMetadata(false));

    public bool CallInProgres
    {
        get => (bool)GetValue(CallInProgresProperty);
        set => SetValue(CallInProgresProperty, value);
    }


    /// <summary>
    /// Cunstructor for the main volunteer window.
    /// </summary>
    public MainVolunteerWindow(int id)
    {
        VolunteerId = id;
        InitializeComponent();
    }
    /// <summary>
    /// Updates the volunteer details when the window loads.
    /// And adds the observer
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerId , UpdateTheVolunteer);
        UpdateTheVolunteer();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(VolunteerId , UpdateTheVolunteer);
    }
    private volatile DispatcherOperation? _observerOperationForUpdateTheVolunteer = null;

    /// <summary>
    /// Updates the volunteer details.
    /// </summary>
    public void UpdateTheVolunteer()
    {
        if (_observerOperationForUpdateTheVolunteer is null || _observerOperationForUpdateTheVolunteer.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateTheVolunteer = Dispatcher.BeginInvoke(() =>
            {

                Volunteer = s_bl.Volunteer.Read(VolunteerId);
                if (Volunteer.callInProgress != null)
                {
                    CallInProgres = true;
                    Call = s_bl.Call.Read(Volunteer.callInProgress.CallId);
                }
                else
                {
                    CallInProgres = false;
                }
            });
    }




    /// <summary>
    /// Button to end call treatment
    /// </summary>
    private void EndButten_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.UpdateEndOfTreatment(Volunteer.Id, Volunteer.callInProgress!.Id);
            CallInProgres = false;

            MessageBox.Show(
                "Treatment completion update passed successfully and thank you for your service.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    /// <summary>
    /// Button to cancel taking a call for treatment
    /// </summary>
    private void CancelButten_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.UpdateCancellationOfTreatment(Volunteer.Id, Volunteer.callInProgress!.Id);
            CallInProgres = false;

            MessageBox.Show("Passed successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    /// <summary>
    /// Button to update volunteer details
    /// </summary>
    private void UpdateButten_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Volunteer.Update(Volunteer.Id, Volunteer);
            if (Volunteer.callInProgress != null)
            {
                CallInProgres = true;
                Call = s_bl.Call.Read(Volunteer.callInProgress.CallId);
            }
            else
            {
                CallInProgres = false;
            }
            MessageBox.Show("Passed successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    /// <summary>
    /// A button that leads to the current volunteer's calling history screen
    /// </summary>
    private void Button_history(object sender, RoutedEventArgs e)
    {
        new CallHistory(Volunteer.Id).Show();
    }


    /// <summary>
    /// A button that leads to a screen for selecting a treatment call for the current volunteer
    /// </summary>
    private void Button_selection(object sender, RoutedEventArgs e)
    {
        if (Volunteer.Active == false)
        {
            MessageBox.Show("You are not active, you can't select a call.");
            return;
        }
        if (Volunteer.callInProgress != null)
        {
            MessageBox.Show("You can't select a new call while you are treating a call.");
            return;
        }
        //Cursor changes to wait      
        Mouse.OverrideCursor = Cursors.Wait;
        new CallSelection(Volunteer.Id).Show();
        //Cursor changes back to normal
        Mouse.OverrideCursor = null;
    }



}