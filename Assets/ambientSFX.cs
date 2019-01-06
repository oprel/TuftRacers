using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambientSFX : MonoBehaviour {

	private AudioSource source;
	
	public void init(AudioClip audio){
		source = GetComponent<AudioSource>();
		source.clip = audio;
		source.Play();
		StartCoroutine(autoDestroy());
	}
	
	private IEnumerator autoDestroy(){
		while (source.isPlaying){
			yield return null;
		}
		Destroy(gameObject);
	}

}
