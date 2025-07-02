﻿using System;
using System.Collections;
using Dan.Enums;
using Dan.Models;
using UnityEngine;

using static Dan.ConstantVariables;

namespace Dan.Main
{
    public static class LeaderboardCreator
    {
        public static bool LoggingEnabled { get; set; } = true;
        
        private static LeaderboardCreatorBehaviour _behaviour;

        internal static string UserGuid;
        
        private const string FORM_PUBLIC_KEY = "publicKey", FORM_USERNAME = "username", FORM_SCORE = "score",
            FORM_EXTRA = "extra", FORM_USER_GUID = "userGuid";

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            Log("Initializing...");
            _behaviour = new GameObject("[LeaderboardCreator]").AddComponent<LeaderboardCreatorBehaviour>();
            UnityEngine.Object.DontDestroyOnLoad(_behaviour.gameObject);

            // Also add the LeaderboardController for React message handling
            _behaviour.gameObject.AddComponent<LeaderboardController>();

            if (LeaderboardCreatorBehaviour.Config.authSaveMode != AuthSaveMode.Unhandled)
                _behaviour.Authorize(OnAuthorizationAttempted);
        }

        private static void OnAuthorizationAttempted(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                Log("<b><color=#FF0000>Failed to connect to server, trying again...</color></b>");

                IEnumerator Co()
                {
                    yield return new WaitForSeconds(5f);
                    _behaviour.Authorize(OnAuthorizationAttempted);
                }

                _behaviour.StartCoroutine(Co());
                return;
            }
            SetUserGuid(guid);
        }
        
        /// <summary>
        /// Requests a new unique identifier for the user from the server.
        /// NOTE: Use this function if you want to manually handle the user's unique identifier.
        /// IMPORTANT: Set the "Authorization Save Mode" to "Unhandled" in the Settings menu of the Leaderboard Creator window.
        /// </summary>
        /// <param name="userGuidCallback">A callback that returns the user's unique identifier.</param>
        public static void RequestUserGuid(Action<string> userGuidCallback)
        {
            _behaviour.Authorize(userGuidCallback);
        }

        /// <summary>
        /// Sets the user's unique identifier to the given string value.
        /// </summary>
        /// <param name="userGuid">The user's unique identifier.</param>
        public static void SetUserGuid(string userGuid)
        {
            UserGuid = userGuid;
            Log("<b><color=#009900>Initialized!</color></b>");
        }

        /// <summary>
        /// Pings the server to check if a connection can be established.
        /// </summary>
        /// <param name="isOnline">If true, the server is online, else connection failed.</param>
        static void Ping(Action<bool> isOnline) => _behaviour.SendGetRequest(GetServerURL(), isOnline, null);

        /// <summary>
        /// Test endpoint for checking if the server is reachable.
        /// </summary>
        /// /// <param name="isOnline">If true, the server is online, else connection failed.</param>
        public static void Test(Action<bool> isOnline) => _behaviour.SendGetRequest(GetServerURL(Routes.Test), isOnline, null);

        /// <summary>
        /// Fetches a leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard
        /// (retrieve from https://danqzq.itch.io/leaderboard-creator).</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(string publicKey, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            GetLeaderboard(publicKey, false, LeaderboardSearchQuery.Default, callback, errorCallback);

        /// <summary>
        /// Fetches a leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard
        /// (retrieve from https://danqzq.itch.io/leaderboard-creator).</param>
        /// <param name="isInAscendingOrder">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(string publicKey, bool isInAscendingOrder, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            GetLeaderboard(publicKey, isInAscendingOrder, LeaderboardSearchQuery.Default, callback, errorCallback);
        
        /// <summary>
        /// Fetches a leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard
        /// (retrieve from https://danqzq.itch.io/leaderboard-creator).</param>
        /// <param name="searchQuery">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(string publicKey, LeaderboardSearchQuery searchQuery, Action<Entry[]> callback, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }

            var query = $"?publicKey={publicKey}&userGuid={UserGuid}";
            query += searchQuery.ChainQuery();
            
            _behaviour.SendGetRequest(GetServerURL(Routes.GetLeaderboard, query), callback, errorCallback);
        }

        /// <summary>
        /// Fetches a leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard
        /// (retrieve from https://danqzq.itch.io/leaderboard-creator).</param>
        /// <param name="isInAscendingOrder">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="searchQuery">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(string publicKey, bool isInAscendingOrder, LeaderboardSearchQuery searchQuery, Action<Entry[]> callback, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }

            var query = $"?publicKey={publicKey}&userGuid={UserGuid}&isInAscendingOrder={(isInAscendingOrder ? 1 : 0)}";
            query += searchQuery.ChainQuery();
            
            _behaviour.SendGetRequest(GetServerURL(Routes.Get, query), callback, errorCallback);
        }
        
        /// <summary>
        /// Uploads a new entry using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="score">The highscore of the player</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void UploadNewEntry(int score, Action<bool> callback = null, Action<string> errorCallback = null) => 
            UploadNewEntry(score, " ", callback, errorCallback);

        /// <summary>
        /// Uploads a new entry using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="score">The highscore of the player</param>
        /// <param name="extra">Extra data to be stored with the entry (max length of 100, unless using an advanced leaderboard)</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void UploadNewEntry(int score, string extra, Action<bool> callback = null, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            // Use the username from React as both publicKey and username
            UploadNewEntry(LeaderboardController.Username, LeaderboardController.Username, score, extra, callback, errorCallback);
        }

        /// <summary>
        /// Gets the leaderboard using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(Action<Entry[]> callback, Action<string> errorCallback = null) => 
            GetLeaderboard(false, LeaderboardSearchQuery.Default, callback, errorCallback);

        /// <summary>
        /// Gets the leaderboard using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="isInAscendingOrder">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(bool isInAscendingOrder, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            GetLeaderboard(isInAscendingOrder, LeaderboardSearchQuery.Default, callback, errorCallback);
        
        /// <summary>
        /// Gets the leaderboard using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="searchQuery">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(LeaderboardSearchQuery searchQuery, Action<Entry[]> callback, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            GetLeaderboard(LeaderboardController.Username, searchQuery, callback, errorCallback);
        }

        /// <summary>
        /// Gets the leaderboard using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="isInAscendingOrder">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="searchQuery">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetLeaderboard(bool isInAscendingOrder, LeaderboardSearchQuery searchQuery, Action<Entry[]> callback, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            GetLeaderboard(LeaderboardController.Username, isInAscendingOrder, searchQuery, callback, errorCallback);
        }

        /// <summary>
        /// Gets the personal entry using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="callback">Returns the entry data if request is successful</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetPersonalEntry(Action<Entry> callback, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            GetPersonalEntry(LeaderboardController.Username, callback, errorCallback);
        }

        /// <summary>
        /// Gets the total number of entries using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="callback">Returns the total number of entries in the leaderboard.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetEntryCount(Action<int> callback, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            GetEntryCount(LeaderboardController.Username, callback, errorCallback);
        }

        /// <summary>
        /// Deletes the entry using the authenticated username as publicKey (from React frontend).
        /// </summary>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void DeleteEntry(Action<bool> callback = null, Action<string> errorCallback = null)
        {
            if (!LeaderboardController.IsAuthenticated)
            {
                LogError("User not authenticated via React frontend. Call SetUserData first.");
                errorCallback?.Invoke("User not authenticated");
                return;
            }

            DeleteEntry(LeaderboardController.Username, callback, errorCallback);
        }

        /// <summary>
        /// Uploads a new entry to the leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard</param>
        /// <param name="username">The username of the player</param>
        /// <param name="score">The highscore of the player</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void UploadNewEntry(string publicKey, string username, int score, Action<bool> callback = null, Action<string> errorCallback = null) => 
            UploadNewEntry(publicKey, username, score, " ", callback, errorCallback);

        /// <summary>
        /// Uploads a new entry to the leaderboard with the given public key.
        /// </summary>
        /// <param name="publicKey">The public key of the leaderboard</param>
        /// <param name="username">The username of the player</param>
        /// <param name="score">The highscore of the player</param>
        /// <param name="extra">Extra data to be stored with the entry (max length of 100, unless using an advanced leaderboard)</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void UploadNewEntry(string publicKey, string username, int score, string extra, Action<bool> callback = null, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                LogError("Username cannot be null or empty!");
                return;
            }

            if (username.Length > 127)
            {
                LogError("Username cannot be longer than 127 characters!");
                return;
            }
            
            // if (string.IsNullOrEmpty(UserGuid))
            // {
            //     LogError("User GUID is null or empty! Please authorize the user before uploading an entry.");
            //     return;
            // }

            callback += isSuccessful =>
            {
                if (!isSuccessful)
                    LogError("Uploading entry data failed!");
                else
                    Log("Successfully uploaded entry data to leaderboard!");
            };
            
            var entryData = new Models.EntryData(
                publicKey,
                username, 
                score.ToString(), 
                extra,
                UserGuid
            );
            
            // Serialize to JSON and send
            string jsonData = JsonUtility.ToJson(entryData);
            _behaviour.SendPlainPostRequest(GetServerURL(Routes.Upload), jsonData, callback, errorCallback);
        }

        /// <summary>
        /// Deletes the entry in a leaderboard, with the given public key.
        /// </summary>
        /// <param name="publicKey">Public key of the leaderboard.</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void DeleteEntry(string publicKey, Action<bool> callback = null, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }
            
            callback += isSuccessful =>
            {
                if (!isSuccessful)
                    LogError("Deleting entry failed!");
                else
                    Log("Successfully deleted player's entry!");
            };
            
            _behaviour.SendPostRequest(GetServerURL(Routes.DeleteEntry), Requests.Form(
                Requests.Field(FORM_PUBLIC_KEY, publicKey),
                Requests.Field(FORM_USER_GUID, UserGuid)), callback, errorCallback);
        }
        
        /// <summary>
        /// Gets the entry data of the player in a leaderboard, with the given public key.
        /// </summary>
        /// <param name="publicKey">Public key of the leaderboard.</param>
        /// <param name="callback">Returns the entry data if request is successful</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetPersonalEntry(string publicKey, Action<Entry> callback, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }
            
            _behaviour.SendGetRequest(GetServerURL(Routes.GetPersonalEntry, 
                $"?publicKey={publicKey}&userGuid={UserGuid}"), callback, errorCallback);
        }
        
        /// <summary>
        /// Gets the total number of entries in a leaderboard, with the given public key.
        /// </summary>
        /// <param name="publicKey">Public key of the leaderboard.</param>
        /// <param name="callback">Returns the total number of entries in the leaderboard.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public static void GetEntryCount(string publicKey, Action<int> callback, Action<string> errorCallback = null)
        {
            if (string.IsNullOrEmpty(publicKey))
            {
                LogError("Public key cannot be null or empty!");
                return;
            }
            
            _behaviour.SendGetRequest(GetServerURL(Routes.GetEntryCount) + $"?publicKey={publicKey}", callback, errorCallback);
        }
        
        /// <summary>
        /// Resets a player's unique identifier and allows them to submit a new entry to the leaderboard.
        /// </summary>
        public static void ResetPlayer(Action onReset = null)
        {
            _behaviour.ResetAndAuthorize(OnAuthorizationAttempted, onReset);
        }

        internal static void Log(string message)
        {
            if (!LoggingEnabled) return;
            Debug.Log($"[LeaderboardCreator] {message}");
        }
        
        internal static void LogError(string message)
        {
            if (!LoggingEnabled) return;
            Debug.LogError($"[LeaderboardCreator] {message}");
        }
    }
}