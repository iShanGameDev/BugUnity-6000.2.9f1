using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseUser user;
    FirebaseDatabase database;
    FirebaseStorage storage;


    public PlayerData LoadedData;

    [Header("Container")]
    public Transform Container;

    public GameObject SetUsernamePrefab;
    
    
    void Start()
    {
        // LoadingManager.Instance.Loading();
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance;
        storage =  FirebaseStorage.DefaultInstance;

        user = auth.CurrentUser;

        if (user != null)
        {
            //User Exist
            print(user.UserId);
            
            database.RootReference.Child("Users").Child(user.UserId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    //Success
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // print(snapshot.GetRawJsonValue());
                        string json = snapshot.GetRawJsonValue();
                        PlayerData data =  JsonUtility.FromJson<PlayerData>(json);
                        LoadedData = data;
                        if (data.Nickname.Length < 1)
                        {
                            //No nickname
                            print("No Nickname");

                            // GameObject go = new GameObject("Nickname");
                            
                            
                            print("Nickname not found");
 

                        }
                        else
                        {
                            print("Nickname Found");
                            if (data.Profile.Length < 1)
                            {
                                print("No Profile");
                            }
                        }
                    }
                    else
                    {
                        PlayerData data = new PlayerData();
                        string JData = JsonUtility.ToJson(data);

                        database.RootReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(JData).ContinueWith(task =>
                        {
                            if (task.IsCompleted)
                            {
                                print("Data base Updated Successfully");
                            }
                            else
                            {
                                print("Data base Updated Failed");
                            }
                        });
                    }
                }
            });
            
        }
        else
        {
            print("User not found");
            //No User

            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                //Desktop Login
                DesktopLogin();
            }
            else if(SystemInfo.deviceType == DeviceType.Handheld)
            {
                //Android Login
                AndroidLogin();
            }
        }

    }
    void DesktopLogin()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult r = task.Result;
            user = r.User;
            print(user.UserId);
            
            
            
            PlayerData data = new PlayerData();
            string JData = JsonUtility.ToJson(data);
            
            database.RootReference.Child("Users").Child(user.UserId).SetRawJsonValueAsync(JData).ContinueWith(task_ =>
            {
                if (task_.IsCompleted)
                {
                    print("Data base Updated Successfully");
                }
                else
                {
                    print("Data base Updated Failed");
                }
            });
        });
    }

    void AndroidLogin()
    {
        
    }
}
[Serializable]
public class PlayerData
{
    public string Nickname;
    public string Profile;

    public int TotalWin;
    public int TotalLoss;
    public int TotalStepMoved;
}