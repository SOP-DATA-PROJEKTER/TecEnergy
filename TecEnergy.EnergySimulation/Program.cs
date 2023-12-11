using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using TecEnergy.Database.Models.DataModels;
using TecEnergy.Database.Repositories;

namespace TecEnergy.EnergySimulation;

internal class Program
{
    private static Timer _timer;
    private static int accCount1;
    private static int accCount2;

    static void Main(string[] args)
    {
        accCount1 = LoadAccCount(1);
        accCount2 = LoadAccCount(2);
        Console.WriteLine("Background service started.");

        // Set up a timer to trigger the SendPostRequest method every 10 seconds
        _timer = new Timer(SendPostRequest, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        // Keep the program running
        Console.ReadLine();
    }

    private static void SendPostRequest(object state)
    {
        try
        {
            // Adjust the URL based on your API endpoint
            string apiUrl = "https://localhost:7141/api/energydata";

            // Create a batch of EnergyData for the last 10 seconds
            var energyDataBatch = GenerateEnergyDataBatch();

            // Serialize the batch to JSON
            string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(energyDataBatch);

            using (HttpClient httpClient = new HttpClient())
            {
                // Send a POST request with the JSON payload
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(apiUrl, content).Result;

                // Check the response if needed
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"POST request sent successfully at {DateTime.Now}, AccCount: {accCount1}");
                    SaveAccCount(accCount1, 1);
                    SaveAccCount(accCount2, 2);
                }
                else
                {
                    Console.WriteLine($"Failed to send POST request. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static List<object> GenerateEnergyDataBatch()
    {
        // Simulate generating a batch of EnergyData for the last 10 seconds
        DateTime currentDateTime = DateTime.UtcNow;
        List<object> energyDataBatch = new List<object>();

        for (int i = 0; i < 10; i++)
        {
            // Generate a random EnergyMeterID for demonstration purposes
            Guid energyMeterId = Guid.Parse("CCC6C8C4-B9DB-4C8D-39D8-08DBEF4C21FB");

            // Simulate energy accumulation
            long accumulatedValue = accCount1++;

            // Create an object in the required format
            var energyDataObject = new
            {
                EnergyMeterID = energyMeterId,
                AccumulatedValue = accumulatedValue
            };

            energyDataBatch.Add(energyDataObject);
        }
        for (int i = 0; i < 10; i++)
        {
            // Generate a random EnergyMeterID for demonstration purposes
            Guid energyMeterId = Guid.Parse("FC8FBF56-46D7-47D9-E486-08DBFA459D3E");

            // Simulate energy accumulation
            long accumulatedValue = accCount2++;

            // Create an object in the required format
            var energyDataObject = new
            {
                EnergyMeterID = energyMeterId,
                AccumulatedValue = accumulatedValue
            };

            energyDataBatch.Add(energyDataObject);
        }


        return energyDataBatch;
    }

    private static int LoadAccCount(int countId)
    {
        try
        {
            // Read the accCount value from a file
            if (File.Exists($"accCount{countId}.txt"))
            {
                string accCountStr = File.ReadAllText($"accCount{countId}.txt");
                return int.Parse(accCountStr);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading accCount: {ex.Message}");
        }

        // Return a default value if loading fails
        return 0;
    }

    private static void SaveAccCount(int accCount, int countId)
    {
        try
        {
            // Save the accCount value to a file
            File.WriteAllText($"accCount{countId}.txt", accCount.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving accCount: {ex.Message}");
        }
    }
}