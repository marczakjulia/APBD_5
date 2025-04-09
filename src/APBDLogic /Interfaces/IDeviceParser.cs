namespace APBD5;
/// <summary>
/// Interface for parsing different devices, created based on Interface Segregation Principle 
/// </summary>
public interface IDeviceParser
{
    PersonalComputer ParsePC(string line, int lineNumber);
    Smartwatch ParseSmartwatch(string line, int lineNumber);
    Embedded ParseEmbedded(string line, int lineNumber);
    Device ParseDevice(string line, int lineNumber);
}