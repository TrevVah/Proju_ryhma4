using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace KilsatMassiks
{
    public class UserDataHandler
    {
        public User currentUser {  get; set; }


        public UserDataHandler(User currentUser) 
        {  
            this.currentUser = currentUser; 
        }

        public void UpdateTrip(DateTime date, int km, float duration, string info, int status) 
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
            //A trip update to database
            Debug.WriteLine("Updating a trip to: " + jsonFilePath);
                {
                    //JSON containing Month's trips for each day into List
                    List<Trip> monthTrips = new List<Trip>();
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    monthTrips = JsonSerializer.Deserialize<List<Trip>>(jsonContent);
                    //Update content
                    monthTrips[date.Day - 1].date_time = date;
                    monthTrips[date.Day - 1].km = km;
                    monthTrips[date.Day - 1].status = status;
                    monthTrips[date.Day - 1].info = info;
                    monthTrips[date.Day - 1].duration = duration;
                    //Updated List serialized back to JSON file
                    string jsonString = JsonSerializer.Serialize(monthTrips);
                    jsonString = jsonString.Replace("},{", "},\n{");
                    jsonString = jsonString.Replace("[", "[\n");
                    jsonString = jsonString.Replace("]", "\n]");

                    File.WriteAllText(jsonFilePath, jsonString);

                    Debug.WriteLine("JSON file updated.");
                }

        }

        public bool UpdateUser(User _user)
        {
            Debug.WriteLine("Finding user database...");

            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.json");
            if (!File.Exists(jsonFilePath))
            {
                File.Create(jsonFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonFilePath, jsonInitContent);
                Debug.WriteLine("Users.json was not found and was created.");
            }
            
            Debug.WriteLine("Updating an user...");
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                List<User> users = JsonSerializer.Deserialize<List<User>>(jsonString);
                foreach(User user in users)
                {
                    if (user.getID() == _user.getID())
                    {
                        users[user.getID()-1] = _user;
                        jsonString = JsonSerializer.Serialize(users);
                        jsonString = jsonString.Replace("},{", "},\n{");
                        jsonString = jsonString.Replace("[", "[\n");
                        jsonString = jsonString.Replace("]", "\n]");
                        File.WriteAllText(jsonFilePath, jsonString);
                        Debug.WriteLine("User updated.");
                        return true;
                    }
                }
            }
            return false;
        }

        public bool UpdatePassword(User _user, Password _pass)
        {
            Debug.WriteLine("Finding user database...");
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.json");
            Debug.WriteLine("Finding password database...");
            string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");

            if (!File.Exists(jsonPasswordFilePath))
            {
                File.Create(jsonPasswordFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonFilePath, jsonInitContent);
                Debug.WriteLine("Passwords.json was not found and was created.");
            }

            Debug.WriteLine("Updating an user's password...");
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                string jsonPassString = File.ReadAllText(jsonPasswordFilePath);
                List<User> users = JsonSerializer.Deserialize<List<User>>(jsonString);
                List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPassString);
                foreach (User user in users)
                {
                    if (user.getID() == _user.getID())
                    {
                        passwords[user.getID() - 1] = _pass;
                        jsonPassString = JsonSerializer.Serialize(passwords);
                        jsonPassString = jsonPassString.Replace("},{", "},\n{");
                        jsonPassString = jsonPassString.Replace("[", "[\n");
                        jsonPassString = jsonPassString.Replace("]", "\n]");
                        File.WriteAllText(jsonPasswordFilePath, jsonPassString);
                        Debug.WriteLine("User's password was updated.");
                        return true;
                    }
                }
            }
            return false;
        }

        public Trip? GetDailyTrip(DateTime date)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Trips\\" + currentUser.getID().ToString() + "\\" + date.ToString("yyyy\\\\MM"));
            string jsonFilePath = Path.Combine(directoryPath, "trips.json");
            if (!File.Exists(jsonFilePath))
            {
                Debug.WriteLine("File does not exist.");
                return null;
            }
            else
            {
                string[] tripsAsJSON = File.ReadAllLines(jsonFilePath);
                Trip trip = JsonSerializer.Deserialize<Trip>(tripsAsJSON[date.Day].TrimEnd(','));
                if (trip.status == null) { return null; }
                else { return trip; }
            }
        }
    }
}
