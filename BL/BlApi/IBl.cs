﻿namespace BlApi;

public interface IBl
{
    IAdmin Admin { get; }
    ICall Call { get; }
    IVolunteer Volunteer { get; }

}
