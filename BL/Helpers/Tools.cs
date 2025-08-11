using BO;
using DalApi;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Helpers;

internal static class Tools
{
    internal static ObserverManager Observers = new();

    private static IDal s_dal = Factory.Get;

    // API key .
    private const string ApiKey = "your-locationiq-api-key";

    // Base URL for the API performing the Forward Geocoding.
    private const string ForwardGeocodeUrl = "https://us1.locationiq.com/v1/search.php";

    // Base URL for the Mapbox API to calculate the distance between two points
    private static readonly string apiUrl = "https://api.mapbox.com/directions/v5/mapbox";

    // Access token for the Mapbox API .
    private const string accessToken = "your-mapbox-access-token";





    /// <summary>
    /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <returns>A tuple containing the latitude and longitude of the address, or null if not found.</returns>
    /// <exception cref="Exception">Thrown if the API call fails or no results are found.</exception>
    public static async Task<(double? Latitude, double? Longitude)> GetCoordinates(string address)
    {
        using (var client = new HttpClient())
        {
            // Build the request URL
            var requestUrl = $"{ForwardGeocodeUrl}?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";

            HttpResponseMessage response;
            try
            {
                // Make the request asynchronously
                response = await client.GetAsync(requestUrl);
            }
            catch (Exception ex)
            {
                throw new BO.BlApiException("Error occurred while calling the API.", ex);
            }

            // If the request fails, throw an exception
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // If too many requests, wait 1 second before retrying
                    Task.Delay(1000).Wait();  // Wait for 1 second synchronously
                    return await GetCoordinates(address); // Retry after waiting
                }
                else
                {
                    throw new BO.BlApiException($"API call failed with status code {response.StatusCode}");
                }
            }

            // Read the response content as a string asynchronously
            var content = await response.Content.ReadAsStringAsync();

            // Parse the received JSON content
            var json = JsonDocument.Parse(content).RootElement;

            // If no results are found, throw an exception
            if (json.GetArrayLength() == 0)
                throw new BlApiException("No results found for the given address.");

            // Get the first result from the array
            var firstResult = json[0];

            // Extract the latitude and longitude from the first result
            double latitude = double.Parse(firstResult.GetProperty("lat").GetString() ?? throw new BlApiException("Latitude is missing or null."));  // Latitude
            double longitude = double.Parse(firstResult.GetProperty("lon").GetString() ?? throw new BlApiException("Longitude is missing or null.")); // Longitude

            return (latitude, longitude);
        }
    }



    /// <summary>
    /// Checks whether a given address is valid by querying an external API.
    /// </summary>
    /// <param name="address">The address to validate.</param>
    /// <returns>True if the address is valid; otherwise, false.</returns>
    /// <exception cref="Exception">Thrown if the API call fails.</exception>
    public static bool IsValidAddress(string address)
    {
        using (var client = new HttpClient())
        {
            try
            {
                // Build the request URL
                var requestUrl = $"{ForwardGeocodeUrl}?key={ApiKey}&q={Uri.EscapeDataString(address)}&format=json";
                var response = client.GetAsync(requestUrl).Result;

                // If the request failed, throw an exception
                if (!response.IsSuccessStatusCode)
                    throw new BO.BlApiException($"API call failed with status code {response.StatusCode}");

                // Read the response content as a string
                var content = response.Content.ReadAsStringAsync().Result;

                // Parse the received content as JSON
                var json = JsonDocument.Parse(content).RootElement;

                // If no results are found, the address is not valid
                return json.GetArrayLength() > 0;
            }
            catch (Exception ex)
            {
                throw new BO.BlApiException(@"An error has occurred.
Please check if your address is correct.
This could also be a problem with your internet connection.
Please check your connection and try again. If the problem persists, contact support");
            }
        }
    }




    /// <summary>
    /// Calculates the distance in kilometers between two addresses using their coordinates (latitude and longitude).
    /// </summary>
    /// <param name="address1">The first address as a string.</param>
    /// <param name="address2">The second address as a string.</param>
    /// <returns>The distance between the two addresses in kilometers.</returns>
    /// <exception cref="Exception">Thrown if one or both addresses cannot be resolved to coordinates.</exception>
    public static double GetDistance(double? lat1, double? lon1, double lat2, double lon2, DO.Distance mode)
    {

        double lat1Value;
        double lon1Value;

        if (lat1.HasValue && lon1.HasValue)
        {
            lat1Value = lat1.Value;
            lon1Value = lon1.Value;
        }
        else
        {
            throw new BlApiException("One or both coordinates are null.");
        }

        if (mode != DO.Distance.Aerial)
        {
            return GetDistanceFromMapbox(lat1Value, lon1Value, lat2, lon2, mode);
        }

        //Radius of the Earth in kilometers
        const double earthRadiusKm = 6371.0;

        // Compute the differences between the latitudes and longitudes
        double deltaLat = lat2 - lat1Value;
        double deltaLon = lon2 - lon1Value;

        // Apply the Haversine formula
        double a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                   Math.Cos(lat1Value) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Calculate the distance
        double distance = earthRadiusKm * c;

        return distance / 1000;
    }


    /// <summary>
    /// Calculates the distance between two points using Mapbox API.
    /// </summary>
    /// <param name="lat1">Latitude of the first point.</param>
    /// <param name="lon1">Longitude of the first point.</param>
    /// <param name="lat2">Latitude of the second point.</param>
    /// <param name="lon2">Longitude of the second point.</param>
    /// <param name="mode">The mode of transportation (e.g., walking, driving).</param>
    /// <returns>The distance between the two points in kilometers.</returns>
    /// <exception cref="BO.BlApiException">Thrown if the API call fails or returns an error.</exception>
    public static double GetDistanceFromMapbox(double lat1, double lon1, double lat2, double lon2, DO.Distance mode)
    {
        string requestUrl = $"{apiUrl}/{mode.ToString().ToLower()}/{lon1},{lat1};{lon2},{lat2}?access_token={accessToken}";

        using (var client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(requestUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(content);
                    // Find the distance value in the response
                    var startIndex = content.IndexOf("\"distance\":") + 11;
                    var endIndex = content.IndexOf(",", startIndex);
                    string distanceString = content.Substring(startIndex, endIndex - startIndex);

                    // Return the distance in kilometers
                    return double.Parse(distanceString) / 1000;
                }
                else
                {
                    throw new BO.BlApiException("API call failed with status code " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new BO.BlApiException(@"Something went wrong with the distance calculation
Check if there is really access between the volunteer and the reading.
This could also be a problem with your internet connection.
Please check your connection and try again. If the problem persists, contact support.");
            }

        }
    }




    /// <summary>
    /// Converts the properties of an object to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="t">The object whose properties are being converted to string.</param>
    /// <returns>A string representation of the object's properties.</returns>
    public static string ToStringProperty<T>(this T t)
    {
        var properties = t.GetType().GetProperties();
        var sb = new StringBuilder();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(t) ?? "null";
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                sb.Append($"{prop.Name}: \n");
                foreach (var item in enumerable)
                {
                    sb.Append($"-{item}\n");
                }
            }
            else
            {

                sb.Append($"{prop.Name}: {value}\n");
            }
        }
        return sb.ToString();

    }


    /// <summary>
    /// Call status check
    /// </summary>
    /// <param name="CallId">The call</param>
    /// <returns>BO.CallStatus </returns>
    /// <exception cref="BlDoesNotExistException">Exception when no call found</exception>
    internal static BO.CallStatus GetCallStatus(int CallId)
    {
        DO.Call doCall;
        lock (AdminManager.BlMutex)
            doCall = s_dal.Call.Read(CallId) ?? throw new BlDoesNotExistException("The call does not exist.");

        DO.Assignment? assignment;
        lock (AdminManager.BlMutex)
            assignment = s_dal.Assignment.ReadAll(a => a.CallId == doCall.Id).LastOrDefault();

        if (assignment == null || (assignment != null && assignment.TheEndType != null && assignment.TheEndType != DO.TypeOfEnding.Treated))
        {
            if (doCall.MaxTimeToFinish < AdminManager.Now && doCall.MaxTimeToFinish != null)
                return BO.CallStatus.Expired;
            else if (doCall.MaxTimeToFinish != null && doCall.MaxTimeToFinish - AdminManager.RiskRange < AdminManager.Now)
                return BO.CallStatus.OpenAtRisk;
            else
                return BO.CallStatus.Open;
        }
        else if (assignment?.TheEndType == DO.TypeOfEnding.Treated)
            return BO.CallStatus.Close;
        else
        {
            if (doCall.MaxTimeToFinish < AdminManager.Now && doCall.MaxTimeToFinish != null)
                return BO.CallStatus.Expired;
            else if (doCall.MaxTimeToFinish != null && doCall.MaxTimeToFinish - AdminManager.RiskRange < AdminManager.Now)
                return BO.CallStatus.RiskTreatment;
            else
                return BO.CallStatus.InTreatment;
        }



    }

    /// <summary>
    /// Update expired for calls and assignments
    /// </summary>
    internal static void UpdateExpired()
    {

        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
            calls = s_dal.Call.ReadAll(c => GetCallStatus(c.Id) == BO.CallStatus.Expired).ToList();

        IEnumerable<DO.Assignment> assignments1;
        lock (AdminManager.BlMutex)
            assignments1 = (from call in calls
                            let assignment = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).LastOrDefault()
                            where assignment == null || (assignment.ExitTime != null && assignment.TheEndType != DO.TypeOfEnding.CancellationHasExpired)
                            select new DO.Assignment
                            {
                                Id = 0,
                                CallId = call.Id,
                                VolunteerId = 0,
                                EntryTime = AdminManager.Now,
                                ExitTime = AdminManager.Now,
                                TheEndType = DO.TypeOfEnding.CancellationHasExpired
                            }).ToList();

        IEnumerable<DO.Assignment> assignments2;
        lock (AdminManager.BlMutex)
            assignments2 = (from call in calls
                            let assignment = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).LastOrDefault()
                            where assignment != null && assignment.ExitTime == null && assignment.TheEndType == null
                            select new DO.Assignment
                            {
                                Id = assignment.Id,
                                CallId = call.Id,
                                VolunteerId = assignment.VolunteerId,
                                EntryTime = assignment.EntryTime,
                                ExitTime = AdminManager.Now,
                                TheEndType = DO.TypeOfEnding.CancellationHasExpired
                            }).ToList();


        List<int> volunteersId = new();
        List<int> callsId = new();

        foreach (var assignment in assignments1)
        {
            callsId.Add(assignment.CallId);
            lock (AdminManager.BlMutex)
                s_dal.Assignment.Create(assignment);
        }

        foreach (var assignment in assignments2)
        {
            volunteersId.Add(assignment.VolunteerId);
            callsId.Add(assignment.CallId);
            lock (AdminManager.BlMutex)
                s_dal.Assignment.Update(assignment);
        }

        foreach (var id in volunteersId)
        {
            VolunteerManager.Observers.NotifyItemUpdated(id);
        }

        foreach (var id in callsId)
        {
            CallManager.Observers.NotifyItemUpdated(id);
        }

        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();
    }
}

