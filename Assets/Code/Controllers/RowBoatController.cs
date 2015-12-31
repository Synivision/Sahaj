using UnityEngine;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Pooling;
using Assets.Code.UnityBehaviours;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline.Providers;

public class RowBoatController : MonoBehaviour
{

    public float speed = 1.0F;
    public float startTime;
    public float journeyLength;
    private PoolingObjectManager _poolingObjectManager;
    private IoCResolver _resolver;
    private UnityReferenceMaster _unityReference;
    bool isInitialised = false;
    bool hasReachedDestination = false;
    public Vector3 destinationPosition;
    private bool attackModeBool;
    private GameObject _statusCanvas;
    public string RowBoatName;
    private PlayerManager _playerManager;

    private GameDataProvider _gameDataProvider;
    public Dictionary<int, string> SeatsDictionary;
    private PrefabProvider _prefabProvider;

    private LevelManager _levelManager;

    private bool _rowBoatEmpty;
    // Use this for initialization
    public void Initialize (IoCResolver resolver,bool attackMode, string rowBoatName, LevelManager levelManager)
    {
        _resolver = resolver;
        _resolver.Resolve(out _unityReference);
        _resolver.Resolve(out _playerManager);
        _resolver.Resolve(out _gameDataProvider);
        _resolver.Resolve(out _prefabProvider);

        isInitialised = true;
        attackModeBool = attackMode;
        _statusCanvas = transform.GetChild(0).gameObject;
        RowBoatName = rowBoatName;

        if (rowBoatName!=null) {
            _playerManager.Model.RowBoatCountDict.TryGetValue(RowBoatName, out SeatsDictionary);
            _rowBoatEmpty = false;
        }

        if (attackModeBool)
        {
            _statusCanvas.SetActive(false);
            _levelManager = levelManager;
        }
        else {
            _statusCanvas.SetActive(true);
        }

    }

    void Start(){
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update ()
    {

        if (isInitialised)
        {
            transform.LookAt(_unityReference.Sun.transform, -Vector3.down);
        }


        if (attackModeBool)
        {

            //For Attack State
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destinationPosition, fracJourney);

            if (Vector3.Distance(gameObject.transform.position, destinationPosition) <= 15 && !_rowBoatEmpty)
            {
                SpawnPirateFromRowboats(destinationPosition);
            }

        }
        else {
            //For Ship Base State


            //show canvas about seat details
            //calculate seats left

            int count = 0;
            foreach (var seat in SeatsDictionary) {
                count++;
                if (seat.Value.Equals("")) {
                    count--;
                }
            }

            _statusCanvas.SetActive(true);
            var panel = _statusCanvas.transform.GetChild(0);
            var textView = panel.transform.GetChild(0).GetComponent<Text>();

            if (count == 6)
            {
                textView.text = "Seats Full";
            }
            else if (count == 0) {
                textView.text = "Seats Empty";
            }
            else {
                textView.text = (6 - count) + " Seats Left";
            }
            count = 0;
        }
    }

    public void SpawnPirateFromRowboats(Vector3 spawnPosition)
    {
        foreach (var seat in SeatsDictionary) {

            _levelManager.CreatePirate(seat.Value, spawnPosition+new Vector3(0,seat.Key,0));
        }

        _rowBoatEmpty = true;

    }
}

