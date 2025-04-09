namespace APBD5;

public class EmptySystemException : Exception
{
    public EmptySystemException() : base("Operation system is not installed.") { }
}

public class EmptyBatteryException : Exception
{
    public EmptyBatteryException() : base("Battery level is too low to turn it on.") { }
}

public class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong network name.") { }
}