using StarWars.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Utils
{
    internal class HttpFetch
    {
        private static HttpClient _httpClient = new();

        public static List<T> FetchApi<T>(string url)
        {
            ApiResponse<T> response = null;
            try
            {
                response = _httpClient.GetFromJsonAsync<ApiResponse<T>>(url).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }


            if (response != null) return response.Results;
            return [];
        }

        public static async Task<T> FetchSingleItemAsync<T>(string url)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data from {url}: {ex.Message}");
                return default;
            }
        }

    }
}