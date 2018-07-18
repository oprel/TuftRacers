using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class quickTimer{
	public static bool timer(ref float timer, float timerMax, float increment = 0){
		if (increment==0) increment = Time.deltaTime;
		timer += increment;
		if (timer>timerMax){
			timer-=timerMax;
			return true;
		}
		return false;
	}
}
