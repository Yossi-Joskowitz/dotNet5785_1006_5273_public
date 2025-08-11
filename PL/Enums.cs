using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

/// <summary>
/// This class represents a collection of volunteers in list sorting types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class VolunteerInListCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfSortingVolunteer> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfSortingVolunteer)) as IEnumerable<BO.TypeOfSortingVolunteer>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of calls in list sorting types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class CallInListCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfSortingCall> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfSortingCall)) as IEnumerable<BO.TypeOfSortingCall>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of closed calls in list sorting types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class ClosedCallsCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfSortingClosedCalls> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfSortingClosedCalls)) as IEnumerable<BO.TypeOfSortingClosedCalls>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of ope calls in list sorting types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class OpenCallsCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfSortingOpenCalls> s_enums =
        (Enum.GetValues(typeof(BO.TypeOfSortingOpenCalls)) as IEnumerable<BO.TypeOfSortingOpenCalls>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of call types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of call status.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of volunteer distance types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class VolunteerDistanceCollection : IEnumerable
{
    static readonly IEnumerable<BO.Distance> s_enums =
        (Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// This class represents a collection of volunteer role types.
/// It implements IEnumerable to allow iteration over the enum values.
/// </summary>
internal class VolunteerTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}







