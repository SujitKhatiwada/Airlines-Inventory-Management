using static Exercise.Exercise;

namespace Exercise
{
    public interface IFlightRepository
    {
        IList<Flight> GetFlightSchedules();
    }
}