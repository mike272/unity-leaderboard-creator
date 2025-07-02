using UnityEngine;
using Dan.Main;

namespace Dan.Demo
{
    /// <summary>
    /// Test script to demonstrate React frontend integration
    /// </summary>
    public class ReactIntegrationTest : MonoBehaviour
    {
        [Header("Test Data")]
        [SerializeField] private string testUsername = "testUser123";
        [SerializeField] private string testMode = "wallet";
        [SerializeField] private string testToken = "test_bearer_token_123";
        [SerializeField] private int testScore = 1000;

        void Start()
        {
            // Test the message handler functionality
            TestMessageHandling();
        }

        /// <summary>
        /// Test the SetUserData message handling as it would be called from React
        /// </summary>
        public void TestMessageHandling()
        {
            Debug.Log("Testing React message handling...");

            // Find the LeaderboardController (should be on the [LeaderboardCreator] GameObject)
            var controller = FindObjectOfType<LeaderboardController>();
            if (controller == null)
            {
                Debug.LogError("LeaderboardController not found! Make sure LeaderboardCreator is initialized.");
                return;
            }

            // Simulate the React sendMessage call
            string userData = $"{testUsername},{testMode},{testToken}";
            controller.SetUserData(userData);

            // Verify the data was set correctly
            if (LeaderboardController.IsAuthenticated)
            {
                Debug.Log($"✓ Authentication successful!");
                Debug.Log($"  Username: {LeaderboardController.Username}");
                Debug.Log($"  Mode: {LeaderboardController.Mode}");
                Debug.Log($"  Token: {LeaderboardController.Token}");

                // Test the React leaderboard operations
                TestReactLeaderboardOperations();
            }
            else
            {
                Debug.LogError("✗ Authentication failed!");
            }
        }

        /// <summary>
        /// Test the new React-authenticated leaderboard operations
        /// </summary>
        private void TestReactLeaderboardOperations()
        {
            Debug.Log("Testing React leaderboard operations...");

            // Test upload entry (this will use the username as publicKey)
            LeaderboardCreator.UploadNewEntry(testScore, "Test extra data", 
                success => Debug.Log($"Upload result: {success}"),
                error => Debug.LogError($"Upload error: {error}")
            );

            // Test get leaderboard
            LeaderboardCreator.GetLeaderboard(
                entries => Debug.Log($"Retrieved {entries.Length} entries"),
                error => Debug.LogError($"Get leaderboard error: {error}")
            );

            // Test using the ReactLeaderboardReference
            var reactLeaderboard = Leaderboards.ReactLeaderboard;
            reactLeaderboard.UploadNewEntry(testScore + 100, "React leaderboard test",
                success => Debug.Log($"React leaderboard upload result: {success}"),
                error => Debug.LogError($"React leaderboard upload error: {error}")
            );
        }

        /// <summary>
        /// Public method that can be called to simulate the React message
        /// </summary>
        [ContextMenu("Simulate React Message")]
        public void SimulateReactMessage()
        {
            TestMessageHandling();
        }

        /// <summary>
        /// Clear authentication data for testing
        /// </summary>
        [ContextMenu("Clear Authentication")]
        public void ClearAuthentication()
        {
            LeaderboardController.ClearUserData();
            Debug.Log("Authentication data cleared");
        }
    }
}