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

    public delegate void Action();
    public event Action ResetAll = delegate{ };

    [HideInInspector]
    public Rigidbody rigidbody;
    [HideInInspector]
    public float speedMultiplier = 1;
    private float knockoutTimer;
    private carAI carAI;
    private WheelCollider[] wheels;
    private AudioSource AudioSource;




	 void Awake(){
        rigidbody = GetComponent<Rigidbody>();
        if (!carManager.cars.Contains(this))
            carManager.cars.Add(this);
        wheels = GetComponentsInChildren<WheelCollider>();
        AudioSource = GetComponent<AudioSource>();
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
        if (!carAI || !carAI.enabled){
            float motor = maxMotorTorque * Input.GetAxis("Vertical " + playerID);
            float steering = maxSteeringAngle * Input.GetAxis("Horizontal " + playerID);
            applyWheels(motor,steering);
       
        }

    }

    void OnTriggerEnter(Collider other){
        if (other.tag == "destroy" && gameManager.ending == null){
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
				wheel.motorTorque = motor * speedMultiplier;

			// update visual wheels if any
            Quaternion q;
            Vector3 p;
            wheel.GetWorldPose (out p, out q);

            // assume that the only child of the wheelcollider is the wheel shape
            Transform shapeTransform = wheel.transform.GetChild (0);
            shapeTransform.position = p;
            shapeTransform.rotation = q;

		}
        engineSound(motor);
        
         //check if car is stuck
         bool stuck = false;
         foreach (WheelCollider w in wheels){
             if (!w.isGrounded) stuck=true;
         }
         if (motor == maxMotorTorque) stuck=true;
         if (carAI && carAI.enabled && motor >= maxMotorTorque * carAI.gas) stuck=true;
        if (stuck && Vector3.Magnitude(rigidbody.velocity)<.5f){
            knockoutTimer+= Time.deltaTime;
            if (knockoutTimer > carManager.stuckTimer) Reset();
        }else{
            knockoutTimer=0;
        }
    }

    public void Reset(){
        if (!lastCheckpoint) {
            gameObject.SetActive(false);
            carManager.carsInPlay--;
            return;
        }
        audioManager.playSFX(3);
        ResetAll();
        if (transitionManager.self != null) transitionManager.fadePulse(gameObject,3);
        knockoutTimer=0;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        if (!gameManager.self){
            transform.position= Vector3.right * order * carManager.carWidth;
            transform.rotation = Quaternion.identity;
            return;
        }
        
        
        Transform t = lastCheckpoint.transform;
        transform.position = t.position + carOffset(t) + 5* Vector3.up;
        transform.rotation = t.rotation;
    }

    public Vector3 carOffset(Transform t){
        int c = carManager.cars.Count-1;
        return  t.right * carManager.carWidth * (c/2 - order);

    }

    public void engineSound(float motor){
        float s = motor/(maxMotorTorque - rigidbody.velocity.magnitude*5);
        AudioSource.volume = Mathf.Lerp(AudioSource.volume,.3f +s,.1f);
        AudioSource.pitch = Mathf.Lerp(AudioSource.pitch, .4f +s - order/4,.1f);

    }
}