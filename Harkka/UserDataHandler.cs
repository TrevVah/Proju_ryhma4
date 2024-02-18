﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace KilsatMassiks
{
    internal class UserDataHandler
    {
        User currentUser {  get; set; }


        public UserDataHandler(User currentUser) 
        {  
            this.currentUser = currentUser; 
        }

        public void UpdateTrip(DateTime date, int km, int status) 
        {
            Debug.WriteLine("Updating a trip...");

            //Directory check + creation
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Trips\\" + currentUser.getID().ToString() + "\\" + date.ToString("yyyy\\\\MM"));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.WriteLine("No existing trips, new directories created: " + directoryPath);
                }
            //File check + creation + init
            string jsonFilePath = Path.Combine(directoryPath, "trips.json");
                if (!File.Exists(jsonFilePath))
                {
                    File.Create(jsonFilePath).Close();
                    Debug.WriteLine("JSON file created successfully: " + jsonFilePath);

                    int x = 0;
                    Trip empty = new Trip();
                    List<Trip> tripList = new List<Trip>();
                    while(DateTime.DaysInMonth(date.Year, date.Month) > x)
                    {
                        tripList.Add(new Trip());
                        x++;
                    }
                    string jsonString = JsonSerializer.Serialize(tripList);
                    jsonString = jsonString.Replace("},{", "},\n{");
                    jsonString = jsonString.Replace("[", "[\n");
                    jsonString = jsonString.Replace("]", "\n]");

                    File.WriteAllText(jsonFilePath, jsonString);

                    Debug.WriteLine("JSON file initialized.");
                }
            //A trip update
            Debug.WriteLine("Updating a trip to: " + jsonFilePath);
                {
                    string[] tripsAsJSON = File.ReadAllLines(jsonFilePath);
                    Debug.WriteLine(tripsAsJSON[date.Day]);
                    Trip trip = JsonSerializer.Deserialize<Trip>(tripsAsJSON[date.Day].TrimEnd(','));
                    trip._date_time = date;
                    trip._km = km;
                    trip._status = status;
                    string updatedTripJSON = JsonSerializer.Serialize(trip);
                    tripsAsJSON[date.Day] = updatedTripJSON + ",";
                    File.WriteAllLines(jsonFilePath, tripsAsJSON);
                }

        }

        public Trip? GetDailyTrip(DateTime date)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Trips\\" + currentUser.getID().ToString() + "\\" + date.ToString("yyyy\\\\MM"));
            string jsonFilePath = Path.Combine(directoryPath, "trips.json");
            if (!File.Exists(jsonFilePath))
            {
                return null;
            }
            else
            {
                string[] tripsAsJSON = File.ReadAllLines(jsonFilePath);
                Debug.WriteLine(tripsAsJSON[date.Day]);
                Trip trip = JsonSerializer.Deserialize<Trip>(tripsAsJSON[date.Day].TrimEnd(','));
                return trip;
            }
        }
    }
}