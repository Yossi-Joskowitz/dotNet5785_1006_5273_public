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

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// The Id of The call, 0 for a new call.
    /// </summary>
    private int CallId { get; set; }

    /// <summary>
    /// Text for the button, either "Add" or "Update"
    /// </summary>
    public string ButtonText { get; set; } = "Add";

    /// <summary>
    /// DependencyProperty for the Call object.
    /// </summary>
    public static readonly DependencyProperty CallProperty =
        DependencyProperty.Register("Call", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    public BO.Call Call
    {
        get { return (BO.Call)GetValue(CallProperty); }
        set { SetValue(CallProperty, value); }
    }



    /// <summary>
    /// Constructor for CallWindow.
    /// Initializes a new or existing call based on the provided ID.
    /// </summary>
    /// <param name="callId">The ID of the call, or 0 for a new call.</param>
    public CallWindow(int id = 0)
    {
        CallId = id;
        ButtonText = CallId == 0 ? "Add" : "Update";
        InitializeComponent();
    }

    /// <summary>
    /// Updates the call details when the window is loaded.
    /// And adds the observer for the call if it's not a now call.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CallId == 0)
        {
            try
            {
                Call = new BO.Call();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            s_bl.Call.AddObserver(CallId, UpdateCall);
            UpdateCall();
        }
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (CallId != 0)
            s_bl.Call.RemoveObserver(CallId, UpdateCall);
    }

    private volatile DispatcherOperation? _observerOperationForUpdateCall = null;
    /// <summary>
    /// Updates the call details.
    /// </summary>
    private void UpdateCall()
    {
        if (_observerOperationForUpdateCall is null || _observerOperationForUpdateCall.Status == DispatcherOperationStatus.Completed)
            _observerOperationForUpdateCall = Dispatcher.BeginInvoke(() =>
            {

                try
                {
                    Call = s_bl.Call.Read(CallId);
                }
                catch (Exception e)
                {
                    if (e.Message == $"The call with the ID {CallId} does not exist.")
                        this.Close();
                    else
                        MessageBox.Show(e.Message);
                }
            });
    }

    /// <summary>
    /// Handles the Add/Update button click.
    /// Adds or updates a call based on the current state.
    /// </summary>
    private void AddOrUpdateButten_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add")
                s_bl.Call.Add(Call);
            else
                s_bl.Call.Update(Call);
            MessageBox.Show("Passed successfully.");
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}