using UnityEngine;
using System.Collections;
using System.Collections.Generic;

     
public class carController : MonoBehaviour {
	public int playerID = 1;
   
    [Header("Engine")]
    public float maxMotorTorque;
    public float maxSteeringAngle;
    
    
    [Header("Game")]
    public int wins;
    public int order;
    public GameObject lastCheckpoint;

    [HideInInspector]
    public Rigidbody rigidbody;
    private float knockoutTimer;
    private carAI carAI;
    private WheelCollider[] wheels;


	 void Awake(){
        rigidbody = GetComponent<Rigidbody>();
        if (!carManager.cars.Contains(this))
            carManager.cars.Add(this);
        wheels = GetComponentsInChildren<WheelCollider>();

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
        //Debug.DrawLine(transform.position,carManager.averagePos);
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
       		foreach (WheelCollider wheel in wheels)
		{
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = steering;

			if (wheel.transform.localPosition.z < 0)
				wheel.motorTorque = motor;

			// update visual wheels if any
            Quaternion q;
            Vector3 p;
            wheel.GetWorldPose (out p, out q);

            // assume that the only child of the wheelcollider is the wheel shape
            Transform shapeTransform = wheel.transform.GetChild (0);
            shapeTransform.position = p;
            shapeTransform.rotation = q;

		}
        
         //check if car is stuck
        if ((!wheels[0].isGrounded ||motor == maxMotorTorque) && Vector3.Magnitude(rigidbody.velocity)<.1f){
            knockoutTimer+= Time.deltaTime;
            if (knockoutTimer > carManager.stuckTimer) Reset();
        }else{
            knockoutTimer=0;
        }
    }

    public void Reset(){
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        if (!gameManager.self){
            transform.position= Vector3.zero;
            transform.rotation = Quaternion.identity;
            return;
        }
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