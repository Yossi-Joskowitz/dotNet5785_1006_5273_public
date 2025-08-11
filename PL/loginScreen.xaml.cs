using PL.MainVolunteer;
using PL.Volunteer;
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

namespace PL;

/// <summary>
/// Interaction logic for loginScreen.xaml
/// </summary>
public partial class loginScreen : Window
{
    // Represents the BL (Business Logic) of the system.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// DependencyProperty for the UserId
    /// </summary>
    public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(int), typeof(loginScreen), new PropertyMetadata(0));

    public int UserId
    {
        get => (int)GetValue(UserIdProperty);
        set => SetValue(UserIdProperty, value);
    }

    /// <summary>
    /// DependencyProperty for the Password.
    /// </summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(loginScreen), new PropertyMetadata(string.Empty));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    /// <summary>
    /// DependencyProperty for determining if the volunterr is a manager.
    /// </summary>
    public static readonly DependencyProperty IsManagerProperty =
        DependencyProperty.Register("IsManager", typeof(bool), typeof(loginScreen), new PropertyMetadata(false));

    public bool IsManager
    {
        get => (bool)GetValue(IsManagerProperty);
        set => SetValue(IsManagerProperty, value);
    }

    /// <summary>
    /// DependencyProperty for determining if the password is visible.
    /// </summary>
    public static readonly DependencyProperty IsPasswordVisibleProperty =
        DependencyProperty.Register("IsPasswordVisible", typeof(bool), typeof(loginScreen), new PropertyMetadata(false));

    public bool IsPasswordVisible
    {
        get { return (bool)GetValue(IsPasswordVisibleProperty); }
        set { SetValue(IsPasswordVisibleProperty, value); }
    }

    /// <summary>
    /// Cunstructor for the login screen.
    /// </summary>
    public loginScreen()
    {
        InitializeComponent();
        IsManager = false;

    }

    /// <summary>
    /// Login button click 
    /// if the user is a volunteer open the volunteer window
    /// if the user is a manager open tow buttons for a manager window or volunteer window
    /// </summary>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //Administrator login to enter and initialize del list
            //if (UserId == 318715273 || UserId == 212061006)
            //{ MainWindow.ShowWindow(UserId);return; }
            BO.Role role = s_bl.Volunteer.LoginReturnType(s_bl.Volunteer.Read(UserId).FullName, Password);
            if (role == BO.Role.Volunteer)
            {
                new MainVolunteerWindow(UserId).Show();
                Password = "";
                UserId = 0;
            }
            else //role == BO.Role.Management
            {
                IsManager = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    /// <summary>
    /// if the manager clicked on the manager button open the manager window
    /// </summary>
    private void OpenManagerWindow_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.ShowWindow(UserId);
        IsManager = false;
    }

    /// <summary>
    /// if the manager clicked on the volunteer button open the volunteer window
    /// </summary>
    private void OpenVolunteerWindow_Click(object sender, RoutedEventArgs e)
    {
        new MainVolunteerWindow(UserId).Show();
        IsManager = false;
    }
}