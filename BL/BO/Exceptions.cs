using System;

namespace BO;


/// <summary>
/// Throws an exception when there is a data or format mismatch.
/// </summary>
[Serializable]
public class BlDataOrFormatMismatchException : Exception
{
    public BlDataOrFormatMismatchException(string? message) : base(message) { }
}

/// <summary>
/// Throws an exception when attempt to perform an illegal action exception
/// </summary>
[Serializable]
public class BlAttemptToPerformAnIllegalActionException : Exception
{
    public BlAttemptToPerformAnIllegalActionException(string? message) : base(message) { }
}

/// <summary>
///Throwing an exception when the object does not exist
/// </summary>
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string? message, Exception? innerException) : base(message, innerException) { }
}


[Serializable]
public class BlApiException: Exception
{
    public BlApiException(string? message) : base(message) { }
    public BlApiException(string? message, Exception? innerException) : base(message, innerException) { }
}




/// <summary>
/// Throws an exception when the object already exists
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException) { }
}
/// <summary>
/// Throw an exception when the object cannot be deleted
/// </summary>
[Serializable]
public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
    public BlDeletionImpossible(string? message, Exception? innerException) : base(message, innerException) { }
}
/// <summary>
/// Throw an exception when the object is Null  
/// </summary>
[Serializable]
public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string? message, Exception? innerException) : base(message, innerException) { }
}
/// <summary>
/// Throwing an exception when no value is entered according to the format
/// </summary>
[Serializable]
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string? message, Exception? innerException) : base(message, innerException) { }
}


[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    //public BlXMLFileLoadCreateException(string? message) : base(message) { }
    public BlXMLFileLoadCreateException(string? message, Exception? innerException) : base(message, innerException) { }
}

[Serializable]
public class BLTemporaryNotAvailableException : Exception
{     
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
}