using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour {

 void Update()
    {
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
