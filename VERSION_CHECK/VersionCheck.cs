using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class VersionCheck
{
    private const string GitHubApiUrl = "https://api.github.com/repos/KCSanders7070/ERAM_2_GEOJSON/releases/latest";
    private readonly string _currentVersion;

    public VersionCheck(string currentVersion)
    {
        _currentVersion = currentVersion;
    }

    public void CheckForUpdates()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Set required headers for GitHub API
                client.DefaultRequestHeaders.UserAgent.ParseAdd("VersionCheckApp");

                // Make the HTTP GET request
                HttpResponseMessage response = client.GetAsync(GitHubApiUrl).Result; // Blocking call
                response.EnsureSuccessStatusCode();

                // Parse the JSON response
                string responseContent = response.Content.ReadAsStringAsync().Result; // Blocking call
                JObject jsonResponse = JObject.Parse(responseContent);

                // Extract the latest version tag
                string latestVersion = jsonResponse["tag_name"]?.ToString();

                // Returned version value is null or empty
                if (string.IsNullOrEmpty(latestVersion))
                {
                    Console.WriteLine("Could not retrieve the latest version.");
                    PromptToContinue();
                    return;
                }

                // Compare the current version with the latest version
                if (_currentVersion != latestVersion)
                {
                    Console.WriteLine($"A new version is available: {latestVersion}.");
                    Console.WriteLine($"Your current version is:    {_currentVersion}.\n\n");
                    Console.WriteLine("Type 'U' to update. You will be taken to the website to download the latest version.");
                    Console.WriteLine("Type 'C' to continue using this version of the program.\n");

                    string userInput;
                    do
                    {
                        Console.Write("Enter your choice (U/C): ");
                        userInput = Console.ReadLine()?.ToUpper();

                        if (userInput == "U")
                        {
                            Console.WriteLine("Opening the download page...");
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "https://github.com/KCSanders7070/ERAM_2_GEOJSON/blob/master/README.md",
                                UseShellExecute = true
                            });

                            Environment.Exit(0); // Exits the application after download page opens
                        }
                        else if (userInput == "C")
                        {
                            Console.WriteLine("Continuing with the current version...");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please type 'U' to update or 'C' to continue.");
                        }
                    } while (userInput != "U" && userInput != "C");
                }
                else
                {
                    Console.WriteLine("You are using the latest version.");
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine("Network error: Unable to connect to the server. Please check your internet connection.");
            Console.WriteLine($"Details: {httpEx.Message}");
            PromptToContinue();
        }
        catch (JsonReaderException jsonEx)
        {
            Console.WriteLine("Error parsing the server response. It may be malformed or unexpected.");
            Console.WriteLine($"Details: {jsonEx.Message}");
            PromptToContinue();
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred.");
            Console.WriteLine($"Details: {ex.Message}");
            PromptToContinue();
        }
    }

    private void PromptToContinue()
    {
        Console.WriteLine("Type 'C' to continue using this version of the program.\n");
        string userInput;

        do
        {
            Console.Write("Enter your choice (C): ");
            userInput = Console.ReadLine()?.ToUpper();

            if (userInput == "C")
            {
                Console.WriteLine("Continuing with the current version...");
            }
            else
            {
                Console.WriteLine("Invalid input. Please type 'C' to continue.");
            }
        } while (userInput != "C");
    }
}
