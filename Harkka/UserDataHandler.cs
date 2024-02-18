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
                    trip.date_time = date;
                    trip.km = km;
                    trip.status = status;
                    string updatedTripJSON = JsonSerializer.Serialize(trip);
                    tripsAsJSON[date.Day] = updatedTripJSON + ",";
                    File.WriteAllLines(jsonFilePath, tripsAsJSON);
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
                        users[user.getID()+1] = _user;
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

        public string? AddUser(string _first_name, string _last_name, string _email, string _password)
        {
            Debug.WriteLine("Finding user database...");

            string jsonUserFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Users.json");
            string jsonPasswordFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Passwords.json");

            if (!File.Exists(jsonUserFilePath))
            {
                File.Create(jsonUserFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonUserFilePath, jsonInitContent);
                Debug.WriteLine("Users.json was not found and was created.");
            }

            if (!File.Exists(jsonPasswordFilePath))
            {
                File.Create(jsonPasswordFilePath).Close();
                string jsonInitContent = "[\n]";
                File.WriteAllText(jsonPasswordFilePath, jsonInitContent);
                Debug.WriteLine("Passwords.json was not found and was created.");
            }

            Debug.WriteLine("Adding an user...");
            {
                string jsonUsersString = File.ReadAllText(jsonUserFilePath);
                string jsonPasswordsString = File.ReadAllText(jsonPasswordFilePath);
                List<User> users = JsonSerializer.Deserialize<List<User>>(jsonUsersString);
                List<Password> passwords = JsonSerializer.Deserialize<List<Password>>(jsonPasswordsString);
                foreach (User user in users)
                {
                    if (user.getEmail() == _email)
                    {
                        return "User with that email already exist.";
                    }
                }

                User newUser = new User(users.Count+1, _first_name, _last_name, _email);
                users.Add(newUser);

                jsonUsersString = JsonSerializer.Serialize(users);
                jsonUsersString = jsonUsersString.Replace("},{", "},\n{");
                jsonUsersString = jsonUsersString.Replace("[", "[\n");
                jsonUsersString = jsonUsersString.Replace("]", "\n]");

                byte[] _salt = Hasher.GenerateSalt();
                byte[] _hashedPassword = Hasher.ComputeHash(_password, _salt);

                Password newPassword = new Password(users.Count, _hashedPassword, _salt);
                Debug.WriteLine(JsonSerializer.Serialize(newPassword));
                passwords.Add(newPassword);

                jsonPasswordsString = JsonSerializer.Serialize(passwords);
                jsonPasswordsString = jsonPasswordsString.Replace("},{", "},\n{");
                jsonPasswordsString = jsonPasswordsString.Replace("[", "[\n");
                jsonPasswordsString = jsonPasswordsString.Replace("]", "\n]");


                File.WriteAllText(jsonUserFilePath, jsonUsersString);
                File.WriteAllText(jsonPasswordFilePath, jsonPasswordsString);
                Debug.WriteLine("User added.");
            }

            return null;
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
