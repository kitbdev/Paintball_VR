using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour {

    public float shootRepeatDur = 0.1f;
    public Material paintmat;
    public float wobbleSpeed = 1;
    public float wobbleScale = 1;
    public float wobbleDecay = 1;
    public float wobbleMax = 1;
    public float fillAmount = 0.5f;
    // public Rigidbody rb;
    PaintHandler paintHandler;
    Vector3 wobble = Vector3.zero;
    Vector3 lastpos = Vector3.zero;

    void Start() {
        paintHandler = GameObject.FindGameObjectWithTag("PaintHandler").GetComponent<PaintHandler>();
        lastpos = transform.position;
    }

    void Update() {
        if (Time.timeScale == 0) {
            return;
        }
        // reduce over time
        wobble.x = Mathf.Lerp(wobble.x, 0, Time.deltaTime * wobbleDecay);
        wobble.z = Mathf.Lerp(wobble.z, 0, Time.deltaTime * wobbleDecay);
        // sinewave 
        Vector3 twobble = wobble;
        twobble.x = wobble.x * Mathf.Sin(wobbleSpeed * Time.time);
        twobble.z = wobble.z * Mathf.Sin(wobbleSpeed * Time.time);
        // twobble = Vector3.ClampMagnitude(twobble, wobbleMax);
        // Debug.Log("v4 "+sinwobblex+", "+sinwobblez+"  "+wobble.x);
        // twobble *= Mathf.PI * 2;
        paintmat.SetVector("Wobble", new Vector4(twobble.x, twobble.z, 0, 0));
        // get velocity
        Vector3 vel = (transform.position - lastpos) / Mathf.Max(Time.deltaTime, 0.01f);
        wobble.x = Mathf.Clamp(wobble.x + vel.x * wobbleScale, -wobbleMax, wobbleMax);
        wobble.z = Mathf.Clamp(wobble.z + vel.z * wobbleScale, -wobbleMax, wobbleMax);
        lastpos = transform.position;

    }
    public void SetColor(Color paintcolor) {
        paintmat.SetColor("PaintTint", paintcolor);
    }
    public void SetFill(float paintFillAmount) {
        fillAmount = paintFillAmount;
        paintmat.SetFloat("FillAmount", fillAmount);
    }
}
