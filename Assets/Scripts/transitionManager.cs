using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transitionManager : MonoBehaviour {

	public static transitionManager self;
	public Color[] fadeColors;
	public const float fadeSpeed = 6;

	private void Awake() {
		self = this;
	}

	public static void fadeIn(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingIn(obj, speed));
	}

	public static void fadeOut(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingOut(obj, speed));
	}
		public static void fadePulse(GameObject obj, float speed = fadeSpeed){
		self.StartCoroutine(self.fadingPulse(obj, speed));
	}


	private IEnumerator fadingOut(GameObject obj, float speed){
		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);
		for (int j = 0; j<speed; j++){
			Renderer.material.color = Color.Lerp(sourceMaterial.color,fadeColors[fadeColors.Length-1],j/speed);
			yield return null;
		}
		for (int i = fadeColors.Length-2; i>=0;i--){
			for (int j = 0; j<speed; j++){
				Renderer.material.color = Color.Lerp(fadeColors[i+1],fadeColors[i],j/speed);
				yield return null;
			}
		}
		Destroy(obj);
	}


	private IEnumerator fadingIn(GameObject obj, float speed){
		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);
		Renderer.material.color = fadeColors[0];
		for (int i = 1; i<fadeColors.Length;i++){
			for (int j = 0; j<speed; j++){
				Renderer.material.color = Color.Lerp(fadeColors[i-1],fadeColors[i],j/speed);
				yield return null;
			}
		}
		for (int j = 0; j<speed; j++){
			Renderer.material.color = Color.Lerp(fadeColors[fadeColors.Length-1],sourceMaterial.color,j/speed);
			yield return null;
		}
		Renderer.material = sourceMaterial;
	}

	private IEnumerator fadingPulse(GameObject obj, float speed){
		Renderer Renderer = obj.GetComponent<Renderer>();
		Material sourceMaterial = Renderer.material;
		Renderer.material = transparentDummyMaterial(sourceMaterial);
		for (int j = 0; j<speed; j++){
			Renderer.material.color = Color.Lerp(sourceMaterial.color,fadeColors[fadeColors.Length-1],j/speed);
			yield return null;
		}
		for (int i = fadeColors.Length-2; i>=0;i--){
			for (int j = 0; j<speed; j++){
				Renderer.material.color = Color.Lerp(fadeColors[i+1],fadeColors[i],j/speed);
				yield return null;
			}
		}
		for (int i = 1; i<fadeColors.Length;i++){
			for (int j = 0; j<speed; j++){
				Renderer.material.color = Color.Lerp(fadeColors[i-1],fadeColors[i],j/speed);
				yield return null;
			}
		}
		for (int j = 0; j<speed; j++){
			Renderer.material.color = Color.Lerp(fadeColors[fadeColors.Length-1],sourceMaterial.color,j/speed);
			yield return null;
		}
		Renderer.material = sourceMaterial;
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
