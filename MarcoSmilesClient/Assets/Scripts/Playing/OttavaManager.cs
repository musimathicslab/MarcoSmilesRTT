using System;
using System.Collections;
using UnityEngine;

public class OttavaManager : MonoBehaviour
{
    [SerializeField] private HandsManager handsManager;
    [SerializeField] private OVRCameraRig cameraRig;

    //Parametri settings
    [SerializeField] private float centerOffset = -0.2f;
    [SerializeField] private float intervalHeight = 0.2f;
    [SerializeField] private int intervalsCount = 3;
    [SerializeField] private float checkTick = 0.1f;
    private float _center;
    private Transform _centerEyeTransform;


    // Start is called before the first frame update
    private void Start()
    {
        _centerEyeTransform = cameraRig.centerEyeAnchor;
        _center = _centerEyeTransform.position.y + centerOffset;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public static event Action<int> OnOttavaChanged;

    public IEnumerator StartCheckOttava()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkTick);

            int upperBound, lowerBound;

            if (intervalsCount % 2 == 0)
            {
                upperBound = Mathf.RoundToInt(intervalsCount / 2);
                lowerBound = (upperBound - 1) * -1;
            }
            else
            {
                upperBound = lowerBound = Mathf.RoundToInt(intervalsCount / 2);
                lowerBound *= -1;
            }

            var righHandPose = handsManager.GetRootPose(HandsManager.HandSide.Right);
            _center = _centerEyeTransform.position.y + centerOffset;
            
            var handY = righHandPose.position.y;
            var value = handY - _center + intervalHeight / 2;
            var fascia = Mathf.FloorToInt(value / intervalHeight);

            //Debug.Log($"[CambioOttava] Center: {center} Hand: {handY}");
            Debug.Log($"[CambioOttava] center {_center} handY {handY} value {value} fascia {fascia}");

            if (fascia <= upperBound && fascia >= lowerBound)
            {
                OnOttavaChanged?.Invoke(fascia);
                Debug.Log($"[CambioOttava]1 suono {fascia} lowerBound {lowerBound} upperBound {upperBound}");
            }
            else if (fascia >= upperBound)
            {
                OnOttavaChanged?.Invoke(upperBound);
                Debug.Log($"[CambioOttava]2 suono {upperBound} lowerBound {lowerBound} upperBound {upperBound}");
            }
            else if (fascia <= lowerBound)
            {
                OnOttavaChanged?.Invoke(lowerBound);
                Debug.Log($"[CambioOttava]3 suono {lowerBound} lowerBound {lowerBound} upperBound {upperBound}");
            }
        }
    }

    public void SetCenterOffset(float value)
    {
        centerOffset = value;
    }
    public float GetCenterOffset => centerOffset;

    public void SetIntervalHeight(float value)
    {
        intervalHeight = value;
    }

    public float GetIntervalHeight => intervalHeight;

    public void SetIntervalCount(int value)
    {
        intervalsCount = value;
    }
    public int GetIntervalCount => intervalsCount;

    public void SetCheckTick(float value)
    {
        checkTick = value;
    }
    public float GetCheckTick => checkTick;
}