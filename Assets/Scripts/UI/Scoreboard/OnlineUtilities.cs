using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class OnlineUtilities {
    public static async Task SignInAnonymously()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return;
            
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as id: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public static async Task SetPlayerName(string name) {
        await SignInAnonymously();
        await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
    }
}