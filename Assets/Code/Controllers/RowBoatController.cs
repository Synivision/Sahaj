using UnityEngine;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Pooling;
using Assets.Code.UnityBehaviours;
using UnityEngine.UI;
using System.Collections.Generic;

public class RowBoatController : MonoBehaviour
{

    public float speed = 1.0F;
    public float startTime;
    public float journeyLength;
    public GameObject rowPrefab ;
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
    // Use this for initialization
    public void Initialize (IoCResolver resolver,bool attackMode, string rowBoatName)
    {
        _resolver = resolver;
        _resolver.Resolve(out _unityReference);
        _resolver.Resolve(out _playerManager);
        isInitialised = true;
        attackModeBool = attackMode;
        _statusCanvas = transform.GetChild(0).gameObject;
        RowBoatName = rowBoatName;

    }

   

    void Start(){
        startTime = Time.time;
        //rowPrefab = this.gameObject;
        //destinationPosition = transform.position;	
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
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            rowPrefab.transform.position = Vector3.Lerp(rowPrefab.transform.position, destinationPosition, fracJourney);
            _statusCanvas.SetActive(false);

        }
        else {
            //show canvas about seat details
            //calculate seats left

            int count = 0;

            Dictionary<int, string> _seatsDictionary;
                _playerManager.Model.RowBoatCountDict.TryGetValue(RowBoatName, out _seatsDictionary);

            foreach (var seat in _seatsDictionary) {
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
}

