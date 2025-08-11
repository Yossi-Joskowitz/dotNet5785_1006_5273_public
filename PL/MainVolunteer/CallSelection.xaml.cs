using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;

namespace PL.MainVolunteer;

/// <summary>
/// Interaction logic for CallSelection.xaml
/// </summary>
public partial class CallSelection : Window
{
    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Represents the selected sorting option for the volunteer list.
    /// </summary>
    public BO.TypeOfSortingOpenCalls? Sort { get; set; } = null;

    /// <summary>
    /// Represents the selected filtering option for the volunteer list.
    /// </summary>
    public BO.CallType? Filter { get; set; } = null;

    /// <summary>
    /// The selected call in the list.
    /// </summary>
    public BO.OpenCallInList? SelectedCall { get; set; }

    /// <summary>
    /// The ID of the volunteer that is currently logged in.
    /// </summary>
    public int VolunteerId { get; set; }

    /// <summary>
    /// DependencyProperty for the Volunteer object.
    /// </summary>
    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(CallSelection), new PropertyMetadata(null));

    public BO.Volunteer Volunteer
    {
        get { return (BO.Volunteer)GetValue(VolunteerProperty); }
        set { SetValue(VolunteerProperty, value); }
    }

    /// <summary>
    /// Dependency property for the Volunteer open calls List.
    /// </summary>
    public static readonly DependencyProperty OpenCallProperty =
        DependencyProperty.Register("OpenCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(CallSelection), new PropertyMetadata(null));

    public IEnumerable<BO.OpenCallInList> OpenCalls
    {
        get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallProperty); }
        set { SetValue(OpenCallProperty, value); }
    }

    /// <summary>
    /// DependencyProperty for the Description property, with a default value of "The Description".
    /// </summary>
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(CallSelection), new PropertyMetadata("The Description"));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }




    /// <summary>
    /// Initializing the window
    /// </summary>
    public CallSelection(int id)
    {
        VolunteerId = id;
        try
        {
            Volunteer = s_bl.Volunteer.Read(VolunteerId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        InitializeComponent();
    }

    /// <summary>
    /// Updating the list when the window is loaded
    /// And adding the observer to the volunteer.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerId , UpdataTheList);
        s_bl.Volunteer.AddObserver(UpdataTheList);
        UpdataTheList();
    }

    /// <summary>
    /// Removing the observer when the window is closed
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Volunteer.RemoveObserver(UpdataTheList);
        s_bl.Volunteer.RemoveObserver(VolunteerId , UpdataTheList);
    }

    private volatile DispatcherOperation? _observerOperationForUpdataTheList = null;
    /// <summary>
    /// Updating the list when there any changes in the open calls list, or for changes in the current filtering or sorting.
    /// </summary>
    public void UpdataTheList()
    {
        if (_observerOperationForUpdataTheList is null || _observerOperationForUpdataTheList.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdataTheList = Dispatcher.BeginInvoke(() =>
            {

                try
                {
                    OpenCalls = s_bl.Call.GetOpenCalls(VolunteerId, Filter, Sort);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            });
    }


    /// <summary>
    /// A function thet handles the selection of a call for treatment
    /// </summary>
    private void SelectButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Call.ChoosingACallForTreatment(VolunteerId, SelectedCall!.CallId);
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Updating the screen when there is a certain sorting or filtering
    /// </summary>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (Filter == BO.CallType.None)
            {
                Filter = null;
            }
            Mouse.OverrideCursor = Cursors.Wait;
            UpdataTheList();
            Mouse.OverrideCursor = null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Defining the call description cube
    /// </summary>
    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedCall != null && SelectedCall.VerbalDescription != null)
            Description = SelectedCall!.VerbalDescription!;
        else
            Description = "The Description:";

    }

    /// <summary>
    /// Handling when the user double clicks
    /// </summary>
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        MessageBox.Show("Please click Select", "Comment", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <summary>
    /// updating the volunteer details when the button is clicked
    /// updating the volunteer address
    /// </summary>
    private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Volunteer.Update(VolunteerId, Volunteer);
            MessageBox.Show("Address update successful.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}