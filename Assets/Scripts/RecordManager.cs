using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
//using UniRx;
public class RecordManager : NetworkBehaviour
{

    [SerializeField] public GameObject m_RecordPrefab;
    private class Record
    {
        public GameObject m_prefab;
        public String image_url;
        public Texture image;
        public Vector3 position;
        public List<string> tags;

        public Record(GameObject prefab, String url)
        {
            m_prefab = prefab;
            image_url = url;
        }
        
        public Record(Texture texture)
        {
            image = texture;
            
            tags = new List<string>();
        }

        public Record(Texture texture, Vector3 pos)
        {
            
        }

        public void instanciate()
        {
            displayRecord(image, position);   
        }


        
        public void moveTo(Vector3 _pos)
        {
            position = _pos;
        }
        
        void displayRecord(Texture texture, Vector3 pos)
        {
            position = new Vector3(0 + Random.Range(-4.0f, 4.0f), 3 + Random.Range(-2.0f, 3.0f),  6);
            GameObject prefab = Instantiate(m_prefab, position, Quaternion.identity);
            prefab.GetComponent<MeshRenderer>().material.mainTexture = texture;
            prefab.transform.position = position;
            NetworkServer.Spawn(prefab);
            Debug.Log("display record");
        }

    }
    
   
    private FirebaseApp app;
    private DatabaseReference db_reference;
    private FirebaseStorage storage;

    private List<Record> records;
    private List<Record> instaciatedRecords;
    
    // Start is called before the first frame update
    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        records = new List<Record>();
        instaciatedRecords = new List<Record>();
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp, i.e.
                app = FirebaseApp.DefaultInstance;
                app.SetEditorDatabaseUrl("https://sketchtalk-a09ef.firebaseio.com/");
                db_reference = FirebaseDatabase.DefaultInstance.GetReference("Records");
                // where app is a Firebase.FirebaseApp property of your application class.

                // Set a flag here indicating that Firebase is ready to use by your
                // application.
                Debug.Log("initialized firebase settings.");
//                db_reference.GetValueAsync();
                db_reference.ChildAdded += HandleChildAdded;
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
        
    }

    

    
    // Update is called once per frame
    void Update()
    {
        while (records.Count > 0)
        {
            StartCoroutine(SetTexture(records[0]));
            instaciatedRecords.Add(records[0]);
            records.RemoveAt(0);
        }
    }

    void SetFromSnapshot(DataSnapshot snapshot)
    {
        
    }
   
    IEnumerator SetTexture(Record record)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(record.image_url))
        {
            yield return www.SendWebRequest();                
    
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {
                record.image = ((DownloadHandlerTexture)www.downloadHandler).texture;
                record.instanciate();
            }
        }
    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        DataSnapshot record = args.Snapshot;
        string reference = Convert.ToString(record.Child("reference").Value);
        float x = Convert.ToSingle(record.Child("x").Value);
        float y = Convert.ToSingle(record.Child("y").Value);
        float z = Convert.ToSingle(record.Child("z").Value);
        Vector3 pos = new Vector3(x, y, z);
        StorageReference image_reference = storage.GetReference(reference);
        image_reference.GetDownloadUrlAsync().ContinueWith(async (Task<Uri> storage_task) => {
            if (storage_task.IsFaulted || storage_task.IsCanceled) {
                Debug.Log(storage_task.Exception.ToString());
                // Uh-oh, an error occurred!
            } else
            {
                var result = await storage_task;
                String url = result.ToString();
                Record r = new Record(m_RecordPrefab, url);
                records.Add(r);
                
            }
        });   
    }
}
