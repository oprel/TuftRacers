using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leapController : MonoBehaviour {

	public float leapCap = 2000;
	public float frequency = 5;
	[Range(0,1)]public float forwardRatio = .2f;
	public float gravityMod = 1;
	public float leapCharge;
	public float leapChargeRate;
	public GameObject killbox;

	public Image display;


	private float leapTimer;
	private Rigidbody Rigidbody;
	private carController car;
	private float oldDrag;
	private bool flying = false;
	private float previousCharge;
	private float previousM;
	private float baseGravityMod;


	// Use this for initialization
	void Awake () {
		Rigidbody = GetComponent<Rigidbody>();
		car = GetComponent<carController>();
		baseGravityMod = gravityMod;
		oldDrag = Rigidbody.drag;
		//car.charging += chargingLeap;
	}
	
	// Update is called once per frame
	void Update () {
		killbox.SetActive(flying);
		if (Input.GetButton("Fire" + car.playerID)) {
		if (car.isGrounded()) chargingLeap();
		}
		if (Input.GetButtonDown("Fire" + car.playerID) && flying) gravityMod*=8;
		if (flying && car.isGrounded()) endLeap();
		display.fillAmount=leapCharge/leapCap;
		
		
		if (leapCharge>0){
			if (leapCharge == previousCharge){
				Leap();
			}
			previousCharge = leapCharge;
		}
	}

	private void FixedUpdate() {
		if (flying){
			Rigidbody.AddForce(Physics.gravity * Rigidbody.mass * gravityMod);
		}
	}


	private void OnCollisionEnter(Collision other) {
		//if (flying) endLeap();
	}

	private void endLeap(){
		Rigidbody.drag = oldDrag;
		Rigidbody.constraints = RigidbodyConstraints.None;
		gravityMod=baseGravityMod;
		flying = false;

	}


	public void chargingLeap(){
		if (leapCharge<leapCap)
			leapCharge += Time.deltaTime * leapCap/leapChargeRate;
		forwardRatio = (leapCharge/leapCap)/4;
		
	}
	public void Leap(){
		
		StartCoroutine(LeapRoutine());
		
	}	
	private IEnumerator LeapRoutine(){
		oldDrag = Rigidbody.drag;
		Vector3 pulse = forwardRatio * transform.forward * leapCharge * Rigidbody.mass + (1-forwardRatio) * Vector3.up * leapCharge * Rigidbody.mass;
		Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		Rigidbody.drag = 0;
		Rigidbody.AddForce(pulse);
		audioManager.playSFX(4);
		leapTimer = frequency;
		yield return new WaitForSeconds(.1f);
		flying = true;
		leapCharge=0;
	}

}
