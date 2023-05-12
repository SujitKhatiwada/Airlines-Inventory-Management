using System;
using System.Linq;
using System.Web;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

// Written By: Sujit Khatiwada , 20023-05-12
// Tool Used: Visual Studio Code 
namespace Exercise
{
    public class Exercise 
    {
        private static readonly int FLIGHT_INVENTORY_CAPACITY = 20;
        public static void Main(string[] args)
        {
            
            var flightDetails = new FlightRepository().GetFlightSchedules().OrderBy(x=>x.Day).ToList();
            
            //list out the loaded flight schedule on the console
            ListFlightSchedules(flightDetails);
            
            //Generate flight itineraries by scheduling a batch of orders.
            GetFlightItineraries(flightDetails);
        }

        private static void ListFlightSchedules(List<Flight> flightDetails)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("User Story 1: Loaded Flight Schedules.");
            Console.WriteLine();
            foreach (var arrivingFlight in flightDetails)
            {
                Console.WriteLine(arrivingFlight.ToString());
            } 
        }


        private static void GetFlightItineraries(List<Flight> flightDetails)
        {
            Console.WriteLine();
            Console.WriteLine("User Story 2: Generated flight itenaries by scheduling a batch of orders.");
            var retVal = new OrderRepository().GetOrderInfoFromJSON();
            Console.WriteLine();
            foreach(var data in retVal)
            {
                var seletedFlight = flightDetails.FirstOrDefault(x => !x.Loaded && data.Destination == x.FlightCode);
                var res = "";
                if(seletedFlight != null)
                {
                    if(seletedFlight.Orders != null && seletedFlight.Orders.Count == FLIGHT_INVENTORY_CAPACITY)
                    {
                        seletedFlight.Loaded = true;
                    }
                    seletedFlight.Orders.Add(data);
                    res = $"order: {data.OrderName}, flightNumber: {seletedFlight.FlightNumber}, departure: {seletedFlight.DepartureFlightCode}, arrival: {seletedFlight.FlightCode}, day: {seletedFlight.Day}";
                }
                else
                {
                    res = $"order: {data.OrderName}, flightNumber: not scheduled";
                }
                Console.WriteLine(res);
            }
        }

        #region Helper Class
        public class Flight
        {
            public Flight(int flightNumber, string flightCity, string flightCode, string departureCity, string departureFlightCode, int day, bool loaded)
            {
                FlightNumber = flightNumber;
                FlightCity = flightCity;
                FlightCode = flightCode;
                DepartureCity = departureCity;
                DepartureFlightCode = departureFlightCode;
                Day = day;
                Loaded = loaded;
                Orders = new List<Order>();
            }
            public int FlightNumber { get; set; }
            public int Day { get; set; }
            public bool Loaded { get; set; }
            public string FlightCity { get; set; }
            public string FlightCode { get; set; }
            public string DepartureCity { get; set; }
            public string DepartureFlightCode { get; set; }
            public IList<Order> Orders { get; set; }

            public override string ToString()
            {
                return $"Flight: {FlightNumber}, Departure: {DepartureFlightCode},  Arrival: {FlightCode}, Day: {Day}";
            }
        }

        public class Order 
        {
            [JsonPropertyName("destination")]
            public string Destination { get; set;}

            public string OrderName { get; set; }

            public int Priority { get; set; }
        }

        #endregion

        #region Repositories

        class FlightRepository : IFlightRepository
        {
            public IList<Flight> GetFlightSchedules()
            {
                var flightNo = 1;
                var day = 1;
                var flights = new List<Flight>();
                //constructor overloading
                flights.Add(new Flight(flightNo++,"Toronto","YYZ","Montreal","YUL",day,false));
                flights.Add(new Flight(flightNo++,"Calgary","YYC","Montreal","YUL",day,false));
                flights.Add(new Flight(flightNo++,"Vancouver","YVR","Montreal","YUL",day,false));

                day++;
                flights.Add(new Flight(flightNo++,"Toronto","YYZ","Montreal","YUL",day,false));
                flights.Add(new Flight(flightNo++,"Calgary","YYC","Montreal","YUL",day,false));
                flights.Add(new Flight(flightNo++,"Vancouver","YVR","Montreal","YUL",day,false));

                return flights;
            }
        }


        class OrderRepository : IOrderRepository
        {
            public IList<Order> GetOrderInfoFromJSON()
            {
                using (StreamReader r = new StreamReader("coding-assigment-orders.json"))
                {
                    try
                    {
                        string json = r.ReadToEnd();
                        var order = JsonConvert.DeserializeObject<Dictionary<string,Order>>(json)?.Select(p =>
                                    new Order { OrderName = p.Key, Destination = p.Value.Destination, Priority = int.Parse(p.Key.Substring(p.Key.LastIndexOf('-') + 1)) })?.ToList();
                        //Order By priority
                        return order?.OrderBy(x=>x.Priority)?.ToList();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("But problem deserealizing the JSON file");
                        Console.WriteLine($"{ex.Message}");
                        return null;
                    }

                }
            }
        }

        #endregion
    }   

}





