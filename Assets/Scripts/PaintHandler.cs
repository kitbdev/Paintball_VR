using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintHandler : MonoBehaviour {

    public Texture2D paintmap;
    public Texture2D splatTex;
    public Color paintColor1 = Color.magenta;
    public Color paintColor2 = Color.cyan;
    public Color paintColor3 = Color.yellow;
    public float paintScale = 0.1f;
    public bool randomRotation = true;
    public float randomScaleAmount = 0.2f;
    public int numPaintSplatRays = 15;
    public bool paintRunEffect = true;
    // position and timers
    List<Vector4> paintDrips = new List<Vector4>();
    // List<float> paintDripTimers = new List<float>();
    [ContextMenuItem("Clear", "ClearPaint")]
    public bool clearPaintOnStart = true;


    void Start() {
        if (clearPaintOnStart) {
            ClearPaint();
        }
    }

    void Update() {

        if (paintRunEffect) {

        }
    }

    public void ClearPaint() {
        Debug.Log("Clearing paintmap");
        var cols = paintmap.GetPixels();
        for (int i = 0; i < cols.Length; i++) {
            cols[i] = Color.clear;
        }
        paintmap.SetPixels(cols);
        paintmap.Apply(true);
    }
    public int GetPaintColor(Vector3 pos, Vector3 dir) {
        int color = 0;

        return color;
    }
    public void PaintSplat(Vector3 pos, Vector3 dir, int color) {
        // fire multiple rays to find surfaces to paint
        // randomly?
        RaycastHit hit;
        for (int rayi = 0; rayi < numPaintSplatRays; rayi++) {
            // random variation
            Vector3 ranDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Vector3 rayDir = dir;
            if (rayi != 0) {
                rayDir += ranDir;
                rayDir.Normalize();
            }
            // raycast to get uv coords
            // must have a mesh collider and be lightmap static
            List<int> hitRenderers = new List<int>();
            float rayLength = 0.5f;
            Debug.DrawRay(pos, rayDir * rayLength, Color.red, 2);
            if (Physics.Raycast(pos, rayDir, out hit, rayLength)) {
                Vector2 uvpoint = hit.lightmapCoord;
                Renderer affectedR = hit.collider.GetComponent<Renderer>();
                if (!affectedR) {
                    affectedR = hit.collider.GetComponentInParent<Renderer>();
                }
                if (!affectedR) {
                    continue;
                }
                if (hitRenderers.Contains(affectedR.GetInstanceID())) {
                    continue;
                }
                hitRenderers.Add(affectedR.GetInstanceID());
                Vector2 uvscale = new Vector2(affectedR.lightmapScaleOffset.x, affectedR.lightmapScaleOffset.y);
                Debug.Log($"hit {affectedR.name} uvpoint: {uvpoint} uvscale: {uvscale}");
                if (uvpoint == Vector2.zero) {
                    continue;
                }
                Paint(uvpoint, uvscale, color, true);
            }
            paintmap.Apply();
            // drips
            if (paintRunEffect) {

            }
        }
    }
    void Paint(Vector2 uvpos, Vector2 uvscale, int colorIndx, bool waitToApply = false) {

        // convert to pixel scale
        int pointX = (int)(uvpos.x * paintmap.width);
        int pointY = (int)(uvpos.y * paintmap.height);
        uvscale *= paintScale;
        int blockWidth = (int)(uvscale.x * paintmap.width);
        int blockHeight = (int)(uvscale.y * paintmap.height);
        blockWidth = Mathf.Clamp(blockWidth, 1, splatTex.width);
        blockHeight = Mathf.Clamp(blockHeight, 1, splatTex.height);

        int midX = blockWidth / 2;
        int midY = blockHeight / 2;
        int startX = Mathf.Clamp(pointX - midX, 0, paintmap.width);
        int startY = Mathf.Clamp(pointY - midY, 0, paintmap.height);

        float splatRot = Random.Range(0, 2 * Mathf.PI);
        float splatScale = Random.Range(1 - randomScaleAmount, 1 + randomScaleAmount);

        // get transformed(rotated and scaled) splat
        Color[][] tsplatcols = new Color[blockHeight][];
        for (int y = 0; y < blockHeight; y++) {
            tsplatcols[y] = new Color[blockWidth];
            for (int x = 0; x < blockWidth; x++) {
                float tx = x - midX;
                float ty = y - midY;
                // scale
                tx *= splatScale;
                ty *= splatScale;
                if (randomRotation) {
                    // rotate
                    float cosr = Mathf.Cos(splatRot);
                    float sinr = Mathf.Sin(splatRot);
                    float txa = tx * cosr + ty * sinr;
                    float tya = ty * cosr - tx * sinr;
                    tx = txa;
                    ty = tya;
                }
                int txi = Mathf.RoundToInt(tx + midX);
                int tyi = Mathf.RoundToInt(ty + midY);
                if (txi == Mathf.Clamp(txi, 0, splatTex.width) && tyi == Mathf.Clamp(tyi, 0, splatTex.height)) {
                    // use only alpha value, color later
                    // float a = splatTex.GetPixel(txi, tyi).a;
                    // tsplatcols[y][x] = new Color(a, a, a, a);
                    tsplatcols[y][x] = splatTex.GetPixel(txi, tyi);
                } else {
                    tsplatcols[y][x] = new Color(0, 0, 0, 0);
                }
                // tsplatcols[y][x] = new Color(1,1,1,1);
            }
        }
        // 1px blur
        // for (int x = 1; x < blockWidth - 1; x++) {
        //     for (int y = 1; y < blockHeight - 1; y++) {
        //         float navg = (tsplatcols[y][x].a + tsplatcols[y - 1][x].a + tsplatcols[y + 1][x].a +
        //             tsplatcols[y][x - 1].a + tsplatcols[y + 1][x].a) / 5;
        //         tsplatcols[y][x] = new Color(navg, navg, navg, navg);
        //     }
        // }
        // convert to single array
        Color[] splatmapColors = new Color[blockWidth * blockHeight];
        for (int x = 0; x < blockWidth; x++) {
            for (int y = 0; y < blockHeight; y++) {
                splatmapColors[y * blockWidth + x] = tsplatcols[y][x];
            }
        }
        Color colorToPaint = colorIndx == 0 ? paintColor1 : (colorIndx == 1 ? paintColor2 : (colorIndx == 2 ? paintColor3 : paintColor1));
        // add splat to original array
        Color[] originalColors = paintmap.GetPixels(startX, startY, blockWidth, blockHeight);
        Debug.Log($"painting at: {startX},{startY}  block: {blockWidth},{blockHeight} rot:{splatRot / 6.283f},sc:{splatScale}");
        for (int i = 0; i < splatmapColors.Length; i++) {
            // int x = i / scaleX; //Mathf.FloorToInt((i + 0f) / scaleX);
            // int y = i % scaleY;
            // Color col = paintmap.GetPixel(startX + x, startY + y);
            if (colorIndx < 0) {
                // erase
                splatmapColors[i] = (originalColors[i] - splatmapColors[i]);
            } else {
                // use new color or existing color if it has a higher alpha value
                float a = splatmapColors[i].a;
                if (originalColors[i].a > a) {
                    splatmapColors[i] = originalColors[i];
                } else {
                    splatmapColors[i] = colorToPaint;
                    splatmapColors[i].a = a;
                }
            }
        }
        // apply
        paintmap.SetPixels(startX, startY, blockWidth, blockHeight, splatmapColors);
        if (!waitToApply) {
            paintmap.Apply();
        }
    }
}
