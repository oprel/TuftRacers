using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour {
	
	public AudioClip[] ambientTracks;
	public AudioClip[] ambientSFX;
	public GameObject SFXprefab;
	public Vector2 ambientSFXrate;
	public float ambientRange;
	public float fadeTime;
	public AudioSource sourceMain, sourceSub;

	private float volumeMain, volumeSub;
	// Use this for initialization
	void Start () {
		StartCoroutine(playAmbientSFX());
		volumeMain = sourceMain.volume;
		volumeSub = sourceSub.volume;

	}
	private void Update() {
		sourceMain.volume = Mathf.Abs(Mathf.Sin(Time.time / fadeTime*2)) * volumeMain;
		sourceSub.volume = (volumeMain-sourceMain.volume - Mathf.Cos(Time.time / fadeTime)) * volumeSub;
		nextSong(sourceMain);
		nextSong(sourceSub);
	}

	private void nextSong(AudioSource source){
		if (source.volume<.01f || !source.isPlaying){
			source.clip = ambientTracks[Random.Range(0,ambientTracks.Length)];
			source.time = Random.value * (source.clip.length-2*fadeTime);
			source.Play();
		}
	}

	private IEnumerator playAmbientSFX(){
		while (true){
			yield return new WaitForSeconds(ambientSFXrate.x + Random.value*ambientSFXrate.y);
			ambientSFX SFX = Instantiate(SFXprefab,transform).GetComponent<ambientSFX>();
			Vector3 pos = transform.position + Random.insideUnitSphere * ambientRange;
			pos.y = transform.position.y;
			SFX.transform.position = pos;
			SFX.init(ambientSFX[Random.Range(0,ambientSFX.Length)]);
		}
	}
	private void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position, ambientRange);
	}
}
