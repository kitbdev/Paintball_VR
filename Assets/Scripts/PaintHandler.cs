using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintHandler : MonoBehaviour {

    public Texture2D paintmap;
    public Texture2D splatTex;
    public Color paintColor1 = Color.magenta;
    public Color paintColor2 = Color.cyan;
    public Color paintColor3 = Color.yellow;
    public LayerMask paintableLayer;
    public float paintScale = 0.1f;
    public bool randomRotation = true;
    public float randomScaleAmount = 0.2f;
    public int numPaintSplatRays = 15;
    public bool paintRunEffect = true;
    public bool blurPaint = false;
    // position and timers
    List<Vector4> paintDrips = new List<Vector4>();
    // List<float> paintDripTimers = new List<float>();
    [ContextMenuItem("Clear", "ClearPaint")]
    public bool clearPaintOnStart = true;
    [ContextMenuItem("Fill Paint", "FillWithPaint")]
    public bool clearPaintOnEnd = true;


    void Start() {
        if (clearPaintOnStart) {
            ClearPaint();
        }
    }
    private void OnDestroy() {
        if (clearPaintOnEnd) {
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
    public void FillWithPaint() {
        Debug.Log("Painting entire paintmap");
        var cols = paintmap.GetPixels();
        for (int i = 0; i < cols.Length; i++) {
            cols[i] = Color.white;
        }
        paintmap.SetPixels(cols);
        paintmap.Apply(true);
    }
    public int GetPaintColor(Vector3 pos, Vector3 dir) {
        int color = -2;
        // if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 50)) {
        if (Physics.Raycast(pos, dir, out var hit, 100, paintableLayer.value)) {
            Vector2 uvpoint = hit.lightmapCoord;
            if (uvpoint == Vector2.zero) {
                Debug.Log("No lightmap value for " + hit.collider.name);
                return -2;
            }
            // todo get area and average
            int pointX = (int)(uvpoint.x * paintmap.width);
            int pointY = (int)(uvpoint.y * paintmap.height);
            // Color[] pmapoColors = paintmap.GetPixels(startX, startY, blockWidth, blockHeight);
            Color col = paintmap.GetPixel(pointX, pointY);

            // get dominant color
            col.a = 1;
            int dcolor = -3;
            if (col.grayscale == 0) {
                dcolor = -1;
            } else if (col == paintColor1) {
                dcolor = 0;
            } else if (col == paintColor2) {
                dcolor = 1;
            } else if (col == paintColor3) {
                dcolor = 2;
            }
            // otherwise get the closest
            if (dcolor == -1) {
                Debug.Log("No Paint there");
            } else {
                Debug.Log($"Paint is {dcolor} {(dcolor == 0 ? "magenta" : (dcolor == 1 ? "cyan" : (dcolor == 2 ? "yellow" : "other")))}");
            }
            color = dcolor;
        }
        return color;
    }
    public Color GetColor(int colorIndx) {
        Color color = Color.black;
        switch (colorIndx) {
            case -1:
                color = Color.white;
                break;
            case 0:
                color = paintColor1;
                break;
            case 1:
                color = paintColor2;
                break;
            case 2:
                color = paintColor3;
                break;
            default:
                break;
        }
        return color;
    }
    public void PaintSplat(Vector3 pos, Vector3 dir, int color) {
        // fire multiple rays to find surfaces to paint
        // randomly?
        RaycastHit hit;
        List<int> hitRenderers = new List<int>();
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
            float rayLength = 0.5f;
            Debug.DrawRay(pos, rayDir * rayLength, Color.red, 2);
            if (Physics.Raycast(pos, rayDir, out hit, rayLength, paintableLayer.value)) {
                Vector2 uvpoint = hit.lightmapCoord;
                if (uvpoint == Vector2.zero) {
                    continue;
                }
                Renderer affectedR = hit.collider.GetComponent<Renderer>();
                if (!affectedR) {
                    affectedR = hit.collider.GetComponentInParent<Renderer>();
                }
                if (!affectedR) {
                    continue;
                }
                int gid = hit.collider.gameObject.GetInstanceID();
                // Debug.Log("check id "+gid+" in:"+hitRenderers.Contains(gid)+" a:"+hitRenderers+"");
                if (hitRenderers.Contains(gid)) {
                    continue;
                }
                hitRenderers.Add(gid);
                Debug.DrawRay(hit.point, hit.normal * 2, Color.blue, 2);
                Vector2 goScale = Vector2.one;
                Vector3 lscale = hit.collider.transform.localScale;
                // string dbgscale = "na";
                if (lscale.x == lscale.y && lscale.x == lscale.z) {
                    goScale *= lscale.x;
                    // dbgscale = "even";
                } else {
                    float fdot = Mathf.Abs(Vector3.Dot(hit.normal, hit.collider.transform.forward));
                    float rdot = Mathf.Abs(Vector3.Dot(hit.normal, hit.collider.transform.right));
                    float updot = Mathf.Abs(Vector3.Dot(hit.normal, hit.collider.transform.up));
                    if (fdot >= rdot && fdot >= updot) {
                        // hit forward side
                        goScale.x = lscale.x;
                        goScale.y = lscale.y;
                        // dbgscale = "forw";
                    } else if (rdot >= fdot && rdot >= updot) {
                        // hit right side
                        goScale.x = lscale.z;
                        goScale.y = lscale.y;
                        // dbgscale = "right";
                    } else {
                        // hit top side
                        goScale.x = lscale.x;
                        goScale.y = lscale.z;
                        // dbgscale = "top";
                    }
                    // Debug.Log($"fd:{fdot} rd:{rdot} ud:{updot}");
                }
                Vector2 uvscale = new Vector2(affectedR.lightmapScaleOffset.x, affectedR.lightmapScaleOffset.y);
                // uvscale.x = goScale.x;
                // uvscale.y = goScale.y;
                // Debug.Log($"hit {affectedR.name} uvpoint: {uvpoint} uvscale: {uvscale} goscale: {goScale} side {dbgscale}");
                float gscaleRatio = goScale.x / goScale.y;
                // uvscale.x = Mathf.Pow(uvscale.x, 0.5f);
                // uvscale.y = Mathf.Pow(uvscale.y, 0.5f);
                // uvscale.x *= goScale.x;
                // uvscale.y *= goScale.y;
                if (gscaleRatio < 1) {
                    uvscale.y *= 1 / gscaleRatio;
                } else {
                    // uvscale.x *= gscaleRatio;
                    uvscale.y *= gscaleRatio;
                }
                uvscale.x = Mathf.Clamp(uvscale.x, 0.01f, 1f);
                uvscale.y = Mathf.Clamp(uvscale.y, 0.01f, 1f);
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
        // uvscale.x = Mathf.Pow(uvscale.x, 0.5f);
        // uvscale.y = Mathf.Pow(uvscale.y, 0.5f);
        uvscale /= 16;
        uvscale /= 2;
        // uvscale += Vector2.one * 32;
        uvscale *= paintScale;
        int blockWidth = (int)(uvscale.x * paintmap.width + 8);
        int blockHeight = (int)(uvscale.y * paintmap.height + 8);
        blockWidth = Mathf.Clamp(blockWidth, 4, paintmap.width / 2);
        blockHeight = Mathf.Clamp(blockHeight, 4, paintmap.height / 2);
        // blockWidth = Mathf.Clamp(blockWidth, 1, splatTex.width);
        // blockHeight = Mathf.Clamp(blockHeight, 1, splatTex.height);
        // blockWidth = 64;
        // blockHeight = 64;

        int midX = blockWidth / 2;
        int midY = blockHeight / 2;
        int startX = Mathf.Clamp(pointX - midX, 0, paintmap.width - blockWidth);
        int startY = Mathf.Clamp(pointY - midY, 0, paintmap.height - blockHeight);

        float splatRandRot = Random.Range(0, 2 * Mathf.PI);
        float splatRandScale = Random.Range(1 - randomScaleAmount, 1 + randomScaleAmount);

        // get transformed (rotated and scaled) splat
        Color[][] tsplatcols = new Color[blockHeight][];
        for (int y = 0; y < blockHeight; y++) {
            tsplatcols[y] = new Color[blockWidth];
            for (int x = 0; x < blockWidth; x++) {
                float tx = x - midX;
                float ty = y - midY;
                // scale
                tx *= splatTex.width / blockWidth;
                ty *= splatTex.height / blockHeight;
                tx *= splatRandScale;
                ty *= splatRandScale;
                if (randomRotation) {
                    // rotate
                    float cosr = Mathf.Cos(splatRandRot);
                    float sinr = Mathf.Sin(splatRandRot);
                    float txa = tx * cosr + ty * sinr;
                    float tya = ty * cosr - tx * sinr;
                    tx = txa;
                    ty = tya;
                }
                int txi = Mathf.RoundToInt(tx + splatTex.width / 2);
                int tyi = Mathf.RoundToInt(ty + splatTex.height / 2);
                if (txi == Mathf.Clamp(txi, 0, splatTex.width) && tyi == Mathf.Clamp(tyi, 0, splatTex.height)) {
                    // use only alpha value, color is later
                    float a = splatTex.GetPixel(txi, tyi).a;
                    tsplatcols[y][x] = new Color(a, a, a, a);
                    // tsplatcols[y][x] = splatTex.GetPixel(txi, tyi);
                } else {
                    tsplatcols[y][x] = new Color(0, 0, 0, 0);
                }
                // tsplatcols[y][x] = new Color(1,1,1,1);
            }
        }
        // 1px blur
        if (blurPaint) {
            Color[][] btsplatcols = new Color[blockHeight][];
            for (int y = 1; y < blockHeight - 1; y++) {
                btsplatcols[y] = new Color[blockWidth];
                for (int x = 1; x < blockWidth - 1; x++) {
                    float navg = (tsplatcols[y][x].a + tsplatcols[y - 1][x].a + tsplatcols[y + 1][x].a +
                        tsplatcols[y][x - 1].a + tsplatcols[y + 1][x].a) / 5f;
                    btsplatcols[y][x] = new Color(navg, navg, navg, navg);
                }
            }
            tsplatcols = btsplatcols;
        }
        // convert to single array
        Color[] splatmapColors = new Color[blockWidth * blockHeight];
        for (int x = 0; x < blockWidth; x++) {
            for (int y = 0; y < blockHeight; y++) {
                splatmapColors[y * blockWidth + x] = tsplatcols[y][x];
            }
        }
        Color colorToPaint = GetColor(colorIndx);
        // add splat to original array
        Color[] originalColors = paintmap.GetPixels(startX, startY, blockWidth, blockHeight);
        // Debug.Log($"painting at: {startX},{startY}  block: {blockWidth},{blockHeight} rot:{splatRandRot / 6.283f},sc:{splatRandScale}");
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
                if (a == 0) {
                    splatmapColors[i] = originalColors[i];
                } else if (originalColors[i].a > a) {
                    splatmapColors[i] = originalColors[i];
                    if (originalColors[i].r != colorToPaint.r && originalColors[i].g != colorToPaint.g && originalColors[i].b != colorToPaint.b) {
                        splatmapColors[i].a -= a;
                    }
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
