using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RandomSpawning : MonoBehaviour
{
    [HideInInspector] public Vector3 localPosition;
    [HideInInspector] public Quaternion localRotation;

    [SerializeField] private Env env;


    public void MoveToSafeRandomPosition(float xMinValueSpawning, float xMaxValueSpawning, float zMinValueSpawning,
        float zMaxValueSpawning)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100;    //Prevent an infinite loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        while (!safePositionFound && attemptsRemaining > 0)
        {
            potentialPosition = new Vector3(UnityEngine.Random.Range(xMinValueSpawning, xMaxValueSpawning), 0f, UnityEngine.Random.Range(zMinValueSpawning, zMaxValueSpawning));

            float yaw = UnityEngine.Random.Range(-180f, 180f);
            potentialRotation = Quaternion.Euler(0f, yaw, 0f);

            Collider[] collider = Physics.OverlapSphere(potentialPosition, 0.05f);

            safePositionFound = collider.Length == 0;
        }

        localPosition = potentialPosition;
        localRotation = potentialRotation;

    }
}
