using System;
using System.Collections;
using UnityEngine;

public class RibattutaManager : MonoBehaviour
{
    [SerializeField] private HandsManager handsManager;
    [SerializeField] private OVRCameraRig cameraRig;

    //Parametri settings
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float thresholdDistance = 0.06f;
    private Transform _centerEyeTransform;

    // Start is called before the first frame update
    private void Start()
    {
        _centerEyeTransform = cameraRig.centerEyeAnchor;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public static event Action OnRibattuta;

    public IEnumerator StartRibattuta()
    {
        while (true)
        {
            var rightHandPose = handsManager.GetRootPose(HandsManager.HandSide.Right);
            var leftHandPose = handsManager.GetRootPose(HandsManager.HandSide.Left);

            var centerEyePos = _centerEyeTransform.position;
            var rightStartPos = rightHandPose.position;
            var leftStartPos = leftHandPose.position;

            var rightReferenceDirection = (rightStartPos - centerEyePos).normalized;
            var leftReferenceDirection = (leftStartPos - centerEyePos).normalized;

            yield return new WaitForSeconds(checkInterval);

            rightHandPose = handsManager.GetRootPose(HandsManager.HandSide.Right);
            leftHandPose = handsManager.GetRootPose(HandsManager.HandSide.Left);

            var rightDisplacement = rightHandPose.position - rightStartPos;
            var leftDisplacement = leftHandPose.position - leftStartPos;

            var rightMovementAlongDirection = Vector3.Dot(rightDisplacement, rightReferenceDirection);
            var leftMovementAlongDirection = Vector3.Dot(leftDisplacement, leftReferenceDirection);

            if (rightMovementAlongDirection >= thresholdDistance || leftMovementAlongDirection >= thresholdDistance)
            {
                Debug.Log("[RibattutaManager] Ribattuta trovata");
                OnRibattuta?.Invoke();
            }
        }
    }

    public void SetCheckInterval(float value)
    {
        checkInterval = value;
    }
    
    public float GetCheckInterval => checkInterval;

    public void SetThresholdDistance(float value)
    {
        thresholdDistance = value;
    }
    
    public float GetThresholdDistance => thresholdDistance;
}