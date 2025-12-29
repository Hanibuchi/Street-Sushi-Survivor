using System.Collections.Generic;
using UnityEngine;

public class CarFollowSensor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Car _car;

    private HashSet<Car> _carsAhead = new HashSet<Car>();

    private void OnTriggerEnter(Collider other)
    {
        if (_car == null) return;
        // Debug.Log("CarFollowSensor: OnTriggerEnter detected");

        // 前方の車を検知
        Car otherCar = other.GetComponentInChildren<Car>();

        if (otherCar != null && otherCar != _car)
        {
            _carsAhead.Add(otherCar);
            UpdateSpeed();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Car otherCar = other.GetComponentInChildren<Car>();

        if (otherCar != null && _carsAhead.Contains(otherCar))
        {
            _carsAhead.Remove(otherCar);
            UpdateSpeed();
        }
    }

    private void UpdateSpeed()
    {
        if (_car == null) return;
        // Debug.Log($"CarFollowSensor: Updating speed. Cars ahead count: {_carsAhead.Count}");

        if (_carsAhead.Count > 0)
        {
            // 前方に車がいる場合、最も遅い車の速度に合わせる
            float slowestSpeed = float.MaxValue;
            foreach (var car in _carsAhead)
            {
                if (car.CurrentSpeed < slowestSpeed)
                {
                    slowestSpeed = car.CurrentSpeed;
                }
            }
            _car.SetSpeed(slowestSpeed);
        }
        else
        {
            // 前方に車がいなくなったら元の速度に戻す
            _car.ResetSpeed();
        }
    }
}
