using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class SsdSpecific : MonoBehaviour
{
    public static int objectToSpawn;
    public static Vector2 rectPos;

    public Button createDetectedAnchorButton;
    public bool automaticSpawning;
    [SerializeField] int[] objectModelIds;   // HoleID should be first in the list
    [SerializeField] ScreenToWorldAnchors screenToWorldAnchors;
    [SerializeField] SSD.Options options = default;
    [SerializeField] AspectRatioFitter frameContainer = null;
    [SerializeField] Text framePrefab = null;
    [SerializeField, Range(0f, 1f)] float scoreThreshold = 0.4f;
    [SerializeField] TextAsset labelMap = null;

    SSD ssd;
    Text[] frames;
    RectTransform imageRect;
    string[] labels;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // This is an example usage of the NNAPI delegate.
        if (options.accelerator == SSD.Accelerator.NNAPI && !Application.isEditor)
        {
            string cacheDir = Application.persistentDataPath;
            string modelToken = "ssd-token";
            var interpreterOptions = new InterpreterOptions();
            var nnapiOptions = NNAPIDelegate.DefaultOptions;
            nnapiOptions.AllowFp16 = true;
            nnapiOptions.CacheDir = cacheDir;
            nnapiOptions.ModelToken = modelToken;
            interpreterOptions.AddDelegate(new NNAPIDelegate(nnapiOptions));
            ssd = new SSD(options, interpreterOptions);
        }
        else
#endif 
        {
            ssd = new SSD(options);
        }

        // Init frames
        frames = new Text[10];
        Transform parent = frameContainer.transform;
        for (int i = 0; i < frames.Length; i++)
        {
            frames[i] = Instantiate(framePrefab, Vector3.zero, Quaternion.identity, parent);
            frames[i].transform.localPosition = Vector3.zero;
        }

        // Labels
        labels = labelMap.text.Split('\n');
        if (!automaticSpawning && ModeHandler.automaticMode)
        {
            if (createDetectedAnchorButton != null)
            {
                createDetectedAnchorButton.gameObject.SetActive(true);
                createDetectedAnchorButton.onClick.AddListener(() => screenToWorldAnchors.AutomaticAnchorAndDistances(rectPos));
            }
            else
            {
                Debug.LogError("No Button detected. Will spawn anchors automatically");
                automaticSpawning = true;
            }
        }
    }

    void OnDestroy()
    {
        ssd?.Dispose();
    }

    public void Invoke(Texture texture)
    {
        ssd.Invoke(texture);

        SSD.Result[] results = ssd.GetResults();
        Vector2 size = (frameContainer.transform as RectTransform).rect.size;

        for (int i = 0; i < 10; i++)
        {
            SetFrame(frames[i], results[i], size);
        }
    }

    void SetFrame(Text frame, SSD.Result result, Vector2 size)
    {
        if (result.score < scoreThreshold)
        {
            frame.gameObject.SetActive(false);
            return;
        }
        else
        {
            frame.gameObject.SetActive(true);
        }

        frame.text = $"{result.classID}: {GetLabelName(result.classID)}: {(int)(result.score * 100)}%";

        var rt = frame.transform as RectTransform;
        rt.anchoredPosition = result.rect.position * size - size * 0.5f;
        rt.sizeDelta = result.rect.size * size;

        objectToSpawn = -1; // Initialize to a value that indicates no detection
        //string debug = detected.ToString() + " id: " + objectIds[i] + " class: " + result.classID;

        for (int i = 0; i < objectModelIds.Length; i++)
        {
            bool detected = result.classID == objectModelIds[i];

            if (detected)
            {
                objectToSpawn = i; // Store the index of the detected object
                imageRect = frame.GetComponentInChildren<RectTransform>();
                float xPosition = imageRect.position.x + imageRect.sizeDelta.x + imageRect.rect.width / 2;
                float yPosition = imageRect.position.y - imageRect.sizeDelta.y - imageRect.rect.height / 2;
                rectPos = new Vector2(xPosition, yPosition);

                if (automaticSpawning && ModeHandler.automaticMode)
                {
                    screenToWorldAnchors.AutomaticAnchorAndDistances(rectPos);
                }

                break; // Exit the loop once a detection is found
            }
        }

        /*if (objectToSpawn == -1)
        {
            rectPos = frame.transform.position; // Set default position if no detection occurred
        }*/
    }

    private string GetLabelName(int id)
    {
        if (id < 0 || id >= labels.Length - 1)
        {
            return "?";
        }
        return labels[id + 1];
    }

}