using System;
using UnityEngine;

namespace Dan.Main
{
    /// <summary>
    /// Simple test to verify the React integration functionality works correctly
    /// </summary>
    public static class ReactIntegrationValidator
    {
        /// <summary>
        /// Run basic validation tests for the React integration
        /// </summary>
        public static void RunTests()
        {
            Debug.Log("=== React Integration Validation ===");
            
            try
            {
                // Test 1: Authentication state when not authenticated
                TestUnauthenticatedState();
                
                // Test 2: SetUserData parsing
                TestSetUserData();
                
                // Test 3: Authentication state when authenticated
                TestAuthenticatedState();
                
                // Test 4: Clear authentication
                TestClearAuthentication();
                
                Debug.Log("✓ All React integration tests passed!");
            }
            catch (Exception e)
            {
                Debug.LogError($"✗ React integration test failed: {e.Message}");
            }
        }
        
        private static void TestUnauthenticatedState()
        {
            Debug.Log("Testing unauthenticated state...");
            
            // Clear any existing auth
            LeaderboardController.ClearUserData();
            
            if (LeaderboardController.IsAuthenticated)
            {
                throw new Exception("IsAuthenticated should be false when not authenticated");
            }
            
            if (!string.IsNullOrEmpty(LeaderboardController.Username))
            {
                throw new Exception("Username should be null when not authenticated");
            }
            
            Debug.Log("✓ Unauthenticated state test passed");
        }
        
        private static void TestSetUserData()
        {
            Debug.Log("Testing SetUserData parsing...");
            
            // Create a test controller (simulating the one that would be created automatically)
            var testObject = new GameObject("TestController");
            var controller = testObject.AddComponent<LeaderboardController>();
            
            try
            {
                // Test valid data
                string testData = "testUser123,wallet,bearer_token_abc123";
                controller.SetUserData(testData);
                
                if (LeaderboardController.Username != "testUser123")
                {
                    throw new Exception($"Expected username 'testUser123', got '{LeaderboardController.Username}'");
                }
                
                if (LeaderboardController.Mode != "wallet")
                {
                    throw new Exception($"Expected mode 'wallet', got '{LeaderboardController.Mode}'");
                }
                
                if (LeaderboardController.Token != "bearer_token_abc123")
                {
                    throw new Exception($"Expected token 'bearer_token_abc123', got '{LeaderboardController.Token}'");
                }
                
                Debug.Log("✓ SetUserData parsing test passed");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testObject);
            }
        }
        
        private static void TestAuthenticatedState()
        {
            Debug.Log("Testing authenticated state...");
            
            if (!LeaderboardController.IsAuthenticated)
            {
                throw new Exception("Should be authenticated after SetUserData");
            }
            
            Debug.Log("✓ Authenticated state test passed");
        }
        
        private static void TestClearAuthentication()
        {
            Debug.Log("Testing clear authentication...");
            
            LeaderboardController.ClearUserData();
            
            if (LeaderboardController.IsAuthenticated)
            {
                throw new Exception("Should not be authenticated after ClearUserData");
            }
            
            Debug.Log("✓ Clear authentication test passed");
        }
    }
}

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Menu item to run React integration tests in the Unity Editor
/// </summary>
public static class ReactIntegrationMenu
{
    [MenuItem("Tools/Leaderboard Creator/Test React Integration")]
    public static void TestReactIntegration()
    {
        Dan.Main.ReactIntegrationValidator.RunTests();
    }
}
#endif