using System.Collections;
using UnityEngine;

public class HandStillnessManager : MonoBehaviour
{
    [SerializeField] private HandsManager handsManager;
    [SerializeField] private float stillnessThreshold = 0.015f;
    [SerializeField] private float tickCheck = 0.5f;

    private Pose[] _lastLeftPoses;

    private Pose[] _lastRightPoses;

    public bool AreHandsStill { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public IEnumerator AreHandsStillCourutine()
    {
        AreHandsStill = false;
        while (true)
        {
            yield return new WaitForSeconds(tickCheck);

            if (!handsManager.AreHandsTracked())
            {
                Debug.Log("[HandStillnessManager] Hand tracking failed");
                AreHandsStill = false;
                continue;
            }

            var actualRightPoses = handsManager.GetJoinPosesFromWrist(HandsManager.HandSide.Right);
            var actualLeftPoses = handsManager.GetJoinPosesFromWrist(HandsManager.HandSide.Left);

            if (_lastRightPoses == null || _lastLeftPoses == null)
            {
                Debug.Log("[HandStillnessManager] _lastRightPoses == null || _lastLeftPoses == null");
                _lastRightPoses = actualRightPoses;
                _lastLeftPoses = actualLeftPoses;
                continue;
            }

            // Debug.Log($"[HandStillnessManager]\n " +
            //           $"lx: {_lastRightPoses[0].position.x} ly: {_lastRightPoses[0].position.y} lz: {_lastRightPoses[0].position.z}\n" +
            //           $"ax: {actualRightPoses[0].position.x} xy:{actualRightPoses[0].position.y} xz: {actualRightPoses[0].position.z}");
            //
            // Debug.Log($"[HandStillnessManager]\n " +
            //           $"lx: {_lastLeftPoses[0].position.x} ly: {_lastLeftPoses[0].position.y} lz: {_lastLeftPoses[0].position.z}\n" +
            //           $"ax: {actualLeftPoses[0].position.x} xy:{actualLeftPoses[0].position.y} xz: {actualLeftPoses[0].position.z}");

            AreHandsStill = true;
            for (var i = 0; i < actualRightPoses.Length; i++)
                if (IsPoseMoved(actualRightPoses[i], _lastRightPoses[i], stillnessThreshold) ||
                    IsPoseMoved(actualLeftPoses[i], _lastLeftPoses[i], stillnessThreshold))
                {
                    AreHandsStill = false;
                    Debug.Log($"[HandStillnessManager] Pose {i} moved too much!");
                    break;
                }

            _lastRightPoses = actualRightPoses;
            _lastLeftPoses = actualLeftPoses;
        }
    }

    private bool IsPoseMoved(Pose actualPose, Pose lastPose, float threshold)
    {
        var diff = actualPose.position - lastPose.position;
        return Mathf.Abs(diff.x) > threshold || Mathf.Abs(diff.y) > threshold ||
               Mathf.Abs(diff.z) > threshold;
    }
}