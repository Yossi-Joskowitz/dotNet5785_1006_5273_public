using DO;
namespace DalApi;

public interface IConfig
{
    DateTime Clock { get; set; }//the system clock

    TimeSpan RiskRange { get; set; }//the range of time for the risk

    void Reset(); //reset the system clock and the risk range

}
