using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour {

	public static audioManager self;
	
	public AudioSource bgmSource;
	public AudioSource sfxSource;

	public AudioClip[] BGM;
	public AudioClip[] SFX;

	void Awake(){
		self = this;
	}
	void Update () {
		if (!bgmSource.isPlaying){
			bgmSource.clip = BGM[Random.Range(0,BGM.Length)];
			bgmSource.Play();
		}
	}

	public static void playSFX(int i){
		//if (self.sfxSource.isPlaying) return;
		if (i<0 || i >= self.SFX.Length) return;
		self.sfxSource.clip = self.SFX[i];
		self.sfxSource.Play();
	}

	public static void nextBGM(){
		self.bgmSource.Stop();
	}


}
