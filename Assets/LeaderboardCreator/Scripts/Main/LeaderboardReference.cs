using System;
using Dan.Models;

namespace Dan.Main
{
    public class LeaderboardReference
    {
        public string PublicKey { get; }

        public LeaderboardReference(string publicKey) => PublicKey = publicKey;

        public void UploadNewEntry(string username, int score, Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.UploadNewEntry(PublicKey, username, score, callback, errorCallback);
        
        public void UploadNewEntry(string username, int score, string extraData, Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.UploadNewEntry(PublicKey, username, score, extraData, callback, errorCallback);

        public void GetEntries(Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(PublicKey, callback, errorCallback);
        
        public void GetEntries(bool isAscending, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(PublicKey, isAscending, callback, errorCallback);
        
        public void GetEntries(LeaderboardSearchQuery query, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(PublicKey, query, callback, errorCallback);
        
        public void GetEntries(bool isAscending, LeaderboardSearchQuery query, Action<Entry[]> callback, Action<string> errorCallback = null) =>
            LeaderboardCreator.GetLeaderboard(PublicKey, isAscending, query, callback, errorCallback);
        
        public void GetPersonalEntry(Action<Entry> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetPersonalEntry(PublicKey, callback, errorCallback);
        
        public void GetEntryCount(Action<int> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetEntryCount(PublicKey, callback, errorCallback);
        
        public void DeleteEntry(Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.DeleteEntry(PublicKey, callback, errorCallback);
        
        public void ResetPlayer(Action onReset = null) => LeaderboardCreator.ResetPlayer(onReset);

        public void TestConnection(Action<bool> callback = null, Action<string> errorCallback = null)=>
            LeaderboardCreator.Test(callback);
    }

    /// <summary>
    /// React-authenticated leaderboard reference that uses the username from React frontend as publicKey
    /// </summary>
    public class ReactLeaderboardReference
    {
        /// <summary>
        /// Uploads a new entry using the authenticated user from React frontend
        /// </summary>
        /// <param name="score">The highscore of the player</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void UploadNewEntry(int score, Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.UploadNewEntry(score, callback, errorCallback);
        
        /// <summary>
        /// Uploads a new entry using the authenticated user from React frontend
        /// </summary>
        /// <param name="score">The highscore of the player</param>
        /// <param name="extraData">Extra data to be stored with the entry</param>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void UploadNewEntry(int score, string extraData, Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.UploadNewEntry(score, extraData, callback, errorCallback);

        /// <summary>
        /// Gets entries using the authenticated user from React frontend
        /// </summary>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetEntries(Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(callback, errorCallback);
        
        /// <summary>
        /// Gets entries using the authenticated user from React frontend
        /// </summary>
        /// <param name="isAscending">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetEntries(bool isAscending, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(isAscending, callback, errorCallback);
        
        /// <summary>
        /// Gets entries using the authenticated user from React frontend
        /// </summary>
        /// <param name="query">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetEntries(LeaderboardSearchQuery query, Action<Entry[]> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetLeaderboard(query, callback, errorCallback);
        
        /// <summary>
        /// Gets entries using the authenticated user from React frontend
        /// </summary>
        /// <param name="isAscending">If true, the leaderboard will be sorted in ascending order.</param>
        /// <param name="query">A struct with additional search parameters for filtering entries.</param>
        /// <param name="callback">Returns entries of the leaderboard if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetEntries(bool isAscending, LeaderboardSearchQuery query, Action<Entry[]> callback, Action<string> errorCallback = null) =>
            LeaderboardCreator.GetLeaderboard(isAscending, query, callback, errorCallback);
        
        /// <summary>
        /// Gets the personal entry using the authenticated user from React frontend
        /// </summary>
        /// <param name="callback">Returns the entry data if request is successful</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetPersonalEntry(Action<Entry> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetPersonalEntry(callback, errorCallback);
        
        /// <summary>
        /// Gets the entry count using the authenticated user from React frontend
        /// </summary>
        /// <param name="callback">Returns the total number of entries in the leaderboard.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void GetEntryCount(Action<int> callback, Action<string> errorCallback = null) => 
            LeaderboardCreator.GetEntryCount(callback, errorCallback);
        
        /// <summary>
        /// Deletes the entry using the authenticated user from React frontend
        /// </summary>
        /// <param name="callback">Returns true if the request was successful.</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void DeleteEntry(Action<bool> callback = null, Action<string> errorCallback = null) => 
            LeaderboardCreator.DeleteEntry(callback, errorCallback);
        
        /// <summary>
        /// Resets the player
        /// </summary>
        /// <param name="onReset">Callback when reset is complete</param>
        public void ResetPlayer(Action onReset = null) => LeaderboardCreator.ResetPlayer(onReset);

        /// <summary>
        /// Tests the connection
        /// </summary>
        /// <param name="callback">Returns true if the server is online</param>
        /// <param name="errorCallback">Returns an error message if the request failed.</param>
        public void TestConnection(Action<bool> callback = null, Action<string> errorCallback = null)=>
            LeaderboardCreator.Test(callback);
    }
}