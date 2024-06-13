using System;
using Unity.Cinemachine;
using UnityEngine;

public class SetupRocketOnLoad : MonoBehaviour
{
    private GameObject _rocket;

    void Start()
    {
        _rocket = GameObject.Find("VehicleRoot");

        SetRocketOnGround();
        SetCameraToTargetRocket();
    }


    void Update()
    {

    }

    private void SetRocketOnGround()
    {
        var startingHeight = 100f;
        _rocket.transform.position += Vector3.up * startingHeight; //Get it all the way above around first.

        Vector3 lowestPoint = GetLowestPoint(_rocket);

        if (Physics.Raycast(lowestPoint + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit))
        {
            float distanceToMoveDown = Vector3.Distance(lowestPoint, hit.point);
            _rocket.transform.position -= Vector3.up * distanceToMoveDown;
        }

    }

    //recursively goes through each child to find the lowest colider point.
    private Vector3 GetLowestPoint(GameObject gameObject)
    {
        var renderer = gameObject.GetComponent<Renderer>();
        var lowestPoint = new Vector3(0, float.MaxValue, 0);
        if (renderer != null)
        {
            lowestPoint = renderer.bounds.min;
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var childLowestPoint = GetLowestPoint(gameObject.transform.GetChild(i).gameObject);
            if (childLowestPoint.y < lowestPoint.y)
            {
                lowestPoint = childLowestPoint;
            }
        }

        return lowestPoint;
    }

    private void SetCameraToTargetRocket()
    {
        var camera = this.gameObject.GetComponent<CinemachineCamera>();
        var cameraTarget = new CameraTarget();
        cameraTarget.TrackingTarget = _rocket.transform;
        camera.Target = cameraTarget;

        var composer = this.gameObject.GetComponent<CinemachineOrbitalFollow>();
        composer.TargetOffset = new Vector3(60, 0);
    }
}
