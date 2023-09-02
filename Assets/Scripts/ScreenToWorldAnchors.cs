using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class ScreenToWorldAnchors : MonoBehaviour
{
    public TextMeshProUGUI distanceText;

    [SerializeField] GameObject[] anchorPrefabs; // Anchor prefab array corresponding to the objects to detect

    static List<GameObject> anchorGameObjects = new List<GameObject>();

    int anchorIndex;
    List<float> distances = new List<float>();
    Dictionary<int, ARAnchor> anchorDictionary = new Dictionary<int, ARAnchor>(); // Stores spawned anchors

    ARRaycastManager raycastManager;
    ARAnchorManager anchorManager;
    ARPlaneManager planeManager;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    GameObject latestAnchorGameObject;

    // In Manual mode, the first anchor created will be representative of the hole 
    // while the corresponding anchors will be the balls
    public void ManualAnchorAndDistances(Vector2 screenPosition)
    {
        if (anchorIndex == 0)
        {
            CreateAnchor(anchorIndex, screenPosition);
            anchorIndex = 1;
        }
        else
        {
            CreateAnchor(anchorIndex, screenPosition);
            CalculateDistance();
        }
    }

    // For automatic mode, first detect and spawn anchor at hole and then at the balls
    public void AutomaticAnchorAndDistances(Vector2 screenPosition)
    {
        if (anchorIndex == 0)
        {
            if (SsdSpecific.objectToSpawn == 0)
            {
                CreateAnchor(anchorIndex, screenPosition);
                anchorIndex = 1;
            }
            else
                return;
        }
        else
        {
            if (SsdSpecific.objectToSpawn > 0)
            {
                CreateAnchor(anchorIndex, screenPosition);
                bool tooClose = false;
                if (anchorGameObjects.Count > 2)
                {
                    for (int i = 1; i < anchorGameObjects.Count - 1; i++)
                    {
                        Vector3 latestPosition = latestAnchorGameObject.transform.position;
                        Vector3 otherPosition = anchorGameObjects[i].transform.position;

                        // If two (ball) objects are less than 10cm from each other, deem it to be too close
                        if (Vector3.Distance(otherPosition, latestPosition) < 0.1f)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                }

                // if two ball objects are too close, avoid spawning anchor to avoid doubling of anchors for same ball
                if (tooClose)
                {
                    anchorGameObjects.Remove(latestAnchorGameObject);
                    Destroy(latestAnchorGameObject);
                    return;
                }

                CalculateDistance();
            }
        }
    }

    // In case you only need to measure the distance between two created anchor objects
    public void TwoObjectAnchorsAndDistances(int objectIndex, Vector2 screenPosition)
    {
        if (objectIndex >= 0 && objectIndex < anchorPrefabs.Length)
        {
            CreateAnchor(objectIndex, screenPosition);
            if (anchorDictionary.Count == anchorPrefabs.Length)
                CalculateDistance();

            if (ModeHandler.manualMode)
                anchorIndex++;
        }
    }

    public void ResetAnchorsAndDistances()
    {
        anchorIndex = 0;
        distanceText.text = "Distance: ";

        foreach (var anchor in anchorDictionary.Values)
        {
            if (anchor != null)
                Destroy(anchor);
        }
        foreach (GameObject anchorObject in anchorGameObjects)
        {
            if (anchorObject != null)
                Destroy(anchorObject);
        }

        distances.Clear();
        anchorGameObjects.Clear();
        anchorDictionary.Clear();
    }

    void CreateAnchor(int objectIndex, Vector2 screenPosition)
    {
        GameObject selectedPrefab = anchorPrefabs[objectIndex];
        List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;
            var hitPlane = planeManager.GetPlane(hitTrackableId);

            var anchor = anchorManager.AttachAnchor(hitPlane, hitPose);
            anchorDictionary[objectIndex] = anchor;

            latestAnchorGameObject = Instantiate(selectedPrefab, anchor.transform);
            latestAnchorGameObject.name = selectedPrefab.name + "_" + objectIndex; // For easy identification
            anchorGameObjects.Add(latestAnchorGameObject);
        }
    }

    void CalculateDistance()
    {
        if(anchorDictionary.Count > 1)
        {
            ARAnchor firstAnchor = anchorDictionary[0];

            // Get the latest anchor
            ARAnchor latestAnchor = anchorDictionary[anchorDictionary.Count - 1];

            float distance = Vector3.Distance(firstAnchor.transform.position, latestAnchor.transform.position);
            distances.Add(distance);
            distanceText.text = "Distance: " + string.Join(", ", distances);
        }        
    }

    void OnDisable()
    {
        ResetAnchorsAndDistances();
    }
}