using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
     
public class carController : MonoBehaviour {
	public int playerID = 1;
    public List<AxleInfo> axleInfos; 
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public GameObject lastCheckpoint;
    public int order;

    [HideInInspector]
    public Rigidbody rigidbody;
    private float knockoutTimer;
    private carAI carAI;


	 void Start(){
        order = playerID-1;
        rigidbody = GetComponent<Rigidbody>();
        carManager.cars.Add(this);
        foreach (AxleInfo axleInfo in axleInfos) {
            axleInfo.leftWheel.ConfigureVehicleSubsteps(8,20,20);
            axleInfo.rightWheel.ConfigureVehicleSubsteps(8,20,20);

		 }
        carAI = GetComponent<carAI>();
	 }
    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
     
    public void FixedUpdate()
    {
        Debug.DrawLine(transform.position,carManager.averagePos);
        if (!carAI || !carAI.enabled){
            float motor = maxMotorTorque * Input.GetAxis("Vertical " + playerID);
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal " + playerID);
            applyWheels(motor,steering);
       
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "destroy"){
            Reset();
        }
            
    }

    public void applyWheels(float motor, float steering){
         foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }else{
				axleInfo.leftWheel.steerAngle = -steering;
                axleInfo.rightWheel.steerAngle = -steering;
			}
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

         //check if car is stuck
        if (motor == maxMotorTorque && Vector3.Magnitude(rigidbody.velocity)<.1f){
            knockoutTimer+= Time.deltaTime;
            if (knockoutTimer > carManager.stuckTimer) Reset();
        }else{
            knockoutTimer=0;
        }
    }

    public void Reset(){
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        if (!lastCheckpoint) {
            gameObject.SetActive(false);
            carManager.carsInPlay--;
            return;
        }
        
        Transform t = lastCheckpoint.transform;
        int c = carManager.cars.Count-1;
        transform.position = t.position + carOffset(t) + 5* Vector3.up;
        transform.rotation = t.rotation;
    }

    public Vector3 carOffset(Transform t){
        int c = carManager.cars.Count-1;
        return  t.right * carManager.carWidth * (c/2 - order);

    }
}