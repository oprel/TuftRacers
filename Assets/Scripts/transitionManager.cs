using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transitionManager : MonoBehaviour {

	public static transitionManager self;
	public Color[] fadeColors;
	public const float fadeSpeed = 6;
	public static List<GameObject> pulseList = new List<GameObject>();

	private void Awake() {
		self = this;
	}

	public static void fadeIn(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingIn(obj, speed));
	}

	public static void fadeOut(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingOut(obj, speed));
	}
	public static void fadeOut(GameObject obj, float speed = fadeSpeed, int i = 0){
		self.StartCoroutine(self.fadingOut(obj, speed));
	}

	public static void fadePulse(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingPulse(obj, speed));
	}


	private IEnumerator fadingOut(GameObject obj, float speed){
		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);
		yield return StartCoroutine(colorLerp(sourceMaterial.color,fadeColors[fadeColors.Length-1],speed,Renderer));
		for (int i = fadeColors.Length-2; i>=0;i--){
			yield return StartCoroutine(colorLerp(fadeColors[i+1],fadeColors[i],speed,Renderer));
		}
		Destroy(obj);
	}


	private IEnumerator fadingIn(GameObject obj, float speed){
		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);
		Renderer.material.color = fadeColors[0];
		for (int i = 1; i<fadeColors.Length;i++){
			yield return StartCoroutine(colorLerp(fadeColors[i-1],fadeColors[i],speed,Renderer));
		}
		yield return StartCoroutine(colorLerp(fadeColors[fadeColors.Length-1],sourceMaterial.color,speed,Renderer));
		if (!Renderer) yield break;
		Renderer.material = sourceMaterial;
	}

	private IEnumerator fadingPulse(GameObject obj, float speed){
		if (pulseList.Contains(obj)) yield break;
		pulseList.Add(obj);

		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);

		yield return StartCoroutine(colorLerp(sourceMaterial.color,fadeColors[fadeColors.Length-1],speed,Renderer));
		for (int i = fadeColors.Length-2; i>=0;i--){
			yield return StartCoroutine(colorLerp(fadeColors[i+1],fadeColors[i],speed,Renderer));
		}


		for (int i = 1; i<fadeColors.Length;i++){
			yield return StartCoroutine(colorLerp(fadeColors[i-1],fadeColors[i],speed,Renderer));
		}
		yield return StartCoroutine(colorLerp(fadeColors[fadeColors.Length-1],sourceMaterial.color,speed,Renderer));
		Renderer.material = sourceMaterial;
		pulseList.Remove(obj);
	}

	public IEnumerator colorLerp(Color a, Color b, float speed, Renderer Renderer){
		for (int i = 0; i<speed; i++){
			if (!Renderer) yield break;
			Renderer.material.color = Color.Lerp(a,b,i/speed);
			yield return null;
		}
	}

	public Material transparentDummyMaterial(Material sourceMat){
		Material mat = new Material(sourceMat.shader);
		mat.CopyPropertiesFromMaterial(sourceMat);
		mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
		mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		mat.SetInt("_ZWrite", 0);
		mat.DisableKeyword("_ALPHATEST_ON");
		mat.DisableKeyword("_ALPHABLEND_ON");
		mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
		mat.renderQueue = 3000;
		return mat;
	}
}
