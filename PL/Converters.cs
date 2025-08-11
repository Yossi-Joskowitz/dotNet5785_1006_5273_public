using BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PL;
/// <summary>
/// This converter is used to determine whether a TextBox is editable or read-only 
/// based on the value of a property (ButtonText). 
/// If the value is "Add", the TextBox will be read-only; otherwise, it will be editable.
/// </summary>
public class IDEditableConverter : IValueConverter
{

    /// <summary>
    /// Converts the value of the ButtonText property to a boolean value for the IsReadOnly property of a TextBox.
    /// </summary>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        return value != "Add" ? true : false;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}


/// <summary>
/// This converter is used to convert a boolean value to a Visibility value.
/// It allows you to control the visibility of elements based on a boolean value.
/// If the value is true, the element will be visible; otherwise, it will be collapsed.
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = (bool)value;
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}


/// <summary>
/// This converter is used to convert a boolean value to a Visibility value.
/// It allows you to control the visibility of elements based on a boolean value.
/// If the value is false, the element will be visible; otherwise, it will be collapsed.
/// </summary>
public class BooleanToVisibilityFalseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = (bool)value;
        return boolValue ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}

/// <summary>
/// This converter is used to determine whether a volunteer can be deleted based on various conditions:
/// - If the volunteer has no calls that were treated, self-canceled, or expired, and the call ID is null,
/// the element will be visible (indicating that the volunteer can be deleted).
/// Otherwise, the element will be collapsed (indicating that the volunteer cannot be deleted).
/// </summary>
public class VolunteerVisibilityForDeleteConverter : IMultiValueConverter
{
    
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

        int numberOfCallsThatTreated = (int)values[0];
        int numberOfCallsThatSelfCanceled = (int)values[1];
        int numberOfCallsTakenAndExpired = (int)values[2];
        int numberOfCallsTakenAndCancelingAnAdministrator = (int)values[3];
        int ? numberOfIdCall = (int ?)values[4];

        
        if (numberOfCallsThatTreated == 0 &&
            numberOfCallsThatSelfCanceled == 0 &&
            numberOfCallsTakenAndExpired == 0 &&
            numberOfCallsTakenAndCancelingAnAdministrator == 0 &&
            numberOfIdCall == null)
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// This converter is used to determine whether a call can be deleted based on its call ID and status.
/// If the call has no assigned volunteers (i.e., the AssignInList is empty) and the status is "Open",
/// the element will be visible, otherwise it will be collapsed.
/// </summary>
public class CallVisibilityDeleteConverter : IMultiValueConverter
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is int callId && values[1] is BO.CallStatus status)
        {
            if (!s_bl.Call.Read(callId).AssignInList.Any() && status == CallStatus.Open)
            {
                return Visibility.Visible;
            }
        }

        return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// This converter is used to determine whether a call can be cancelled based on its status.
/// If the status is "RiskTreatment" or "InTreatment", the element will be visible, otherwise it will be collapsed.
/// </summary>
public class CallVisibilityForCancelConverter : IValueConverter
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is BO.CallStatus status)
        {
            return status == BO.CallStatus.RiskTreatment || status == BO.CallStatus.InTreatment ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// This converter is used to convert an object to a list of key-value pairs.
/// It converts the properties of the object into a list of KeyValuePair<string, object> where the key is the property name 
/// and the value is the corresponding property value.
/// </summary>
public class ObjectToKeyValuePairsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;

        // יוצרים אוסף של שם שדה וערך
        return value.GetType()
                    .GetProperties()
                    .Select(prop => new KeyValuePair<string, object>(prop.Name, prop.GetValue(value)))
                    .ToList();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class IsEnabledfalseToTrueConvert : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}