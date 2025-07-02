# React Frontend Integration for Unity Leaderboard Creator

This implementation adds support for receiving user data from a React frontend via the React-Unity package.

## How it works

### React Frontend Message
The React frontend sends user data using:
```javascript
sendMessage("LeaderboardController", "SetUserData", `${username},${mode},${token}`);
```

Where:
- `username`: String - the username to use as both display name and publicKey
- `mode`: String - either "wallet" or "email" 
- `token`: String - the authentication bearer token

### Unity Message Handler
The `LeaderboardController` component automatically receives this message and:
1. Parses the username, mode, and token
2. Stores the authentication data
3. Sets the username as the UserGuid for the leaderboard system

### Using the Authenticated User Data

#### Option 1: Use the new React-specific methods
```csharp
// Upload a score using the authenticated user
LeaderboardCreator.UploadNewEntry(1000, "extra data", 
    success => Debug.Log($"Upload successful: {success}"),
    error => Debug.LogError($"Upload failed: {error}")
);

// Get leaderboard for the authenticated user
LeaderboardCreator.GetLeaderboard(
    entries => Debug.Log($"Got {entries.Length} entries"),
    error => Debug.LogError($"Failed to get leaderboard: {error}")
);
```

#### Option 2: Use the ReactLeaderboardReference
```csharp
var reactLeaderboard = Leaderboards.ReactLeaderboard;
reactLeaderboard.UploadNewEntry(1000, success => Debug.Log($"Upload: {success}"));
reactLeaderboard.GetEntries(entries => Debug.Log($"Entries: {entries.Length}"));
```

### Authentication Requirements

All React-authenticated methods will check `LeaderboardController.IsAuthenticated` before proceeding. If not authenticated, they will return an error.

### HTTP Request Headers

When authenticated via React, all HTTP requests automatically include:
```
Authorization: Bearer {token}
```

### Backward Compatibility

All existing functionality remains unchanged. The original LeaderboardCreator methods that take explicit publicKey parameters continue to work exactly as before.

## Testing

Use the `ReactIntegrationTest` component to test the integration:
1. Add the component to a GameObject in your scene
2. Use the context menu "Simulate React Message" to test
3. Use "Clear Authentication" to reset the authentication state

## Example Integration

```csharp
public class GameManager : MonoBehaviour
{
    void Start()
    {
        // Wait for React authentication
        if (LeaderboardController.IsAuthenticated)
        {
            OnUserAuthenticated();
        }
    }

    void OnUserAuthenticated()
    {
        Debug.Log($"User {LeaderboardController.Username} authenticated via {LeaderboardController.Mode}");
        
        // Now you can use the authenticated leaderboard operations
        var reactLeaderboard = Leaderboards.ReactLeaderboard;
        reactLeaderboard.GetEntries(DisplayLeaderboard);
    }

    void DisplayLeaderboard(Entry[] entries)
    {
        foreach (var entry in entries)
        {
            Debug.Log($"{entry.Rank}. {entry.Username}: {entry.Score}");
        }
    }

    public void SubmitScore(int score)
    {
        if (LeaderboardController.IsAuthenticated)
        {
            LeaderboardCreator.UploadNewEntry(score, 
                success => Debug.Log($"Score submitted: {success}"),
                error => Debug.LogError($"Failed to submit score: {error}")
            );
        }
    }
}
```