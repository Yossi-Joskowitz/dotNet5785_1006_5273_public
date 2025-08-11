namespace DO;

/// <summary>
///Throwing an exception when the object does not exist
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

/// <summary>
/// Throws an exception when the object already exists
/// </summary>
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
/// <summary>
/// Throw an exception when the object cannot be deleted
/// </summary>
[Serializable]
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
/// <summary>
/// Throw an exception when the object is Null  
/// </summary>
[Serializable]
public class NullReferenceException : Exception
{
    public NullReferenceException(string? message) : base(message) { }
}
/// <summary>
/// Throwing an exception when no value is entered according to the format
/// </summary>
[Serializable]
public class FormatException : Exception
{
    public FormatException(string? message) : base(message) { }
}


[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}