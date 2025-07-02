using UnityEngine;
using Dan.Main;

namespace Dan.Main
{
    /// <summary>
    /// Controller to handle messages from React frontend via React-Unity package
    /// </summary>
    public class LeaderboardController : MonoBehaviour
    {
        // Static data to store authentication information from React
        public static string Username { get; private set; }
        public static string Mode { get; private set; } // "wallet" or "email"
        public static string Token { get; private set; } // Bearer token
        public static bool IsAuthenticated => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Token);

        /// <summary>
        /// Message handler called by React frontend via sendMessage("LeaderboardController", "SetUserData", username, mode, token)
        /// </summary>
        /// <param name="userData">Comma-separated string containing username,mode,token</param>
        public void SetUserData(string userData)
        {
            if (string.IsNullOrEmpty(userData))
            {
                LeaderboardCreator.LogError("SetUserData received empty or null data");
                return;
            }

            // Parse the comma-separated data: username,mode,token
            var parts = userData.Split(',');
            if (parts.Length != 3)
            {
                LeaderboardCreator.LogError($"SetUserData expected 3 parameters, got {parts.Length}. Data: {userData}");
                return;
            }

            Username = parts[0].Trim();
            Mode = parts[1].Trim();
            Token = parts[2].Trim();

            // Validate mode
            if (Mode != "wallet" && Mode != "email")
            {
                LeaderboardCreator.LogError($"Invalid mode: {Mode}. Expected 'wallet' or 'email'");
                return;
            }

            LeaderboardCreator.Log($"User data set successfully - Username: {Username}, Mode: {Mode}");
            
            // Set the username as the user GUID for the leaderboard system
            LeaderboardCreator.SetUserGuid(Username);
        }

        /// <summary>
        /// Alternative method to set user data with individual parameters
        /// </summary>
        /// <param name="username">Username from React</param>
        /// <param name="mode">Authentication mode ("wallet" or "email")</param>
        /// <param name="token">Bearer authentication token</param>
        public void SetUserDataIndividual(string username, string mode, string token)
        {
            Username = username;
            Mode = mode;
            Token = token;

            // Validate mode
            if (mode != "wallet" && mode != "email")
            {
                LeaderboardCreator.LogError($"Invalid mode: {mode}. Expected 'wallet' or 'email'");
                return;
            }

            LeaderboardCreator.Log($"User data set successfully - Username: {username}, Mode: {mode}");
            
            // Set the username as the user GUID for the leaderboard system
            LeaderboardCreator.SetUserGuid(username);
        }

        /// <summary>
        /// Clear authentication data
        /// </summary>
        public static void ClearUserData()
        {
            Username = null;
            Mode = null;
            Token = null;
            LeaderboardCreator.Log("User data cleared");
        }
    }
}