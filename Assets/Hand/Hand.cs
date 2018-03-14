using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    int numFingers = 5;
    int numSegs = 3;
    Vector3[,] rots;
    Vector3[,] rotsTarget;
    Vector3[,] rotsLimitMin;
    Vector3[,] rotsLimitMax;
    int numTry = 200;
    Vector3[,,] rotsTry;
    GameObject[,] segs;
    GameObject[] palms;
    float smooth = .95f;
    [Range(1, 5)]
    public float delay;
//    float timeDelay;
    GameObject rod;
    GameObject[,] breadCrumbs;
//    int numBreadCrumbs;
    int frameCount;
    float speedCam = .25f;
    GameObject web;
    GameObject foreArm;
    int curTry;
    bool ynDoneLearning;

    // Use this for initialization
	void Start () {
        initRod();
        initBreadCrumbs();
        initRots();
        initRotsLimit();
        initPalm();
        initSegs();
        initForeArm();
        initRotsTry();
	}
	
	// Update is called once per frame
	void Update () {
        if (ynDoneLearning == true)
        {
            updateRotsTarget();
            updateRotsSmooth();
            updateSegs();
        }
        else
        {
            updateLearning();
        }
        updateCam();
        frameCount++;
	}

    void updateRotsTarget()
    {
        for (int f = 0; f < numFingers; f++)
        {
            int tNearest = findNearestTry(f);
//            if (tNearest > -1)
//            {
                loadRotsTry(tNearest, f);
//            }
        }
    }

    void initRotsTry() {
        rotsTry = new Vector3[numTry, numFingers, numSegs];
        for (int t = 0; t < numTry; t++) {
            for (int f = 0; f < numFingers; f++) {
                for (int s = 0; s < numSegs; s++) {
                    rotsTry[t, f, s] = getRotsRandom(f, s);
                }
            }
        }
    }

    void loadRotsTry(int t, int f) {
        for (int s = 0; s < numSegs; s++) {
            rots[f, s] = rotsTry[t, f, s];
        }        
    }

    void updateLearning() {
        if (curTry >= numTry) {
            rod.transform.position = new Vector3(-3.1f, -2.8f, 1.6f);
            rod.transform.localScale = new Vector3(3.8f, 3.8f, 3.8f);
            ynDoneLearning = true;
            return;
        } 
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                rots[f, s] = rotsTry[curTry, f, s];
            }
            updateSegs();
            breadCrumbs[curTry, f].transform.position = getTip(f);
        }
        curTry++;
    }

    int findNearestTry(int f) {
        float distNearest = -1;
        int tNearest = -1;
        float distNearestBackup = -1;
        int tNearestBackup = -1;
        Vector3 posRod = rod.transform.position;
        for (int t = 0; t < numTry; t++) {
            Vector3 posTry = breadCrumbs[t, f].transform.position;
            float dist = Vector3.Distance(posRod, posTry);
            if (dist > rod.transform.localScale.x / 2)
            {
                if (t == 0 || dist < distNearest)
                {
                    distNearest = dist;
                    tNearest = t;
                }
            } else {
                if (t == 0 || dist < distNearestBackup)
                {
                    distNearestBackup = dist;
                    tNearestBackup = t;
                }
            }
        }
        if (tNearest == -1) {
            tNearest = tNearestBackup;
        }
        return tNearest;
    }

    Vector3 getTip(int f) {
        GameObject segLast = segs[f, numSegs - 1];
        Vector3 posTip = segLast.transform.position + segLast.transform.forward * segLast.transform.localScale.z / 2;
        return posTip;
    }

    float getDistFingerTipToPoint(int f, Vector3 pos) {
        Vector3 posTip = getTip(f);
        return Vector3.Distance(posTip, pos);
    }

    void initForeArm() {
        foreArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        foreArm.name = "foreArm";
        foreArm.transform.position = new Vector3(-3.8f, -.7f, -6.7f);
        foreArm.transform.eulerAngles = new Vector3(-8.6f, -2.5f, 10.6f);
        foreArm.transform.localScale = new Vector3(3.6f, 1.4f, 9.4f);
    }

    void updateWeb() {
        GameObject goFrom = segs[0, 0];
        GameObject goTo = palms[1];
        Vector3 posGoTo = goTo.transform.position + goTo.transform.forward * goTo.transform.localScale.z / 3;
        Vector3 posGoFrom = goFrom.transform.position + goFrom.transform.up * -1 * goFrom.transform.localScale.y / 4;
        web.transform.position = posGoFrom;
        web.transform.LookAt(posGoTo);
        web.transform.position = (posGoFrom + posGoTo) / 2;
        web.transform.Rotate(0, 0, -1 * goFrom.transform.eulerAngles.x);
    }

    void updateCam() {
        Camera.main.transform.position = palms[0].transform.position;
        Camera.main.transform.position += Vector3.forward * 20 * Mathf.Cos(speedCam * frameCount * Mathf.Deg2Rad);
        Camera.main.transform.position += Vector3.right * 15 * Mathf.Sin(speedCam * frameCount * Mathf.Deg2Rad);
        Camera.main.transform.position += Vector3.up * 15;
        Camera.main.transform.LookAt(palms[0].transform.position);
    }

    void initBreadCrumbs() {
        breadCrumbs = new GameObject[numTry, numFingers];
        for (int t = 0; t < numTry; t++)
        {
            for (int f = 0; f < numFingers; f++)
            {
                breadCrumbs[t, f] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //float sx = rod.transform.localScale.x;
                //breadCrumbs[b].transform.position = new Vector3(-3, -3, 3) + Random.insideUnitSphere * 5;
                breadCrumbs[t, f].transform.position = Vector3.zero;
                breadCrumbs[t, f].transform.localScale = new Vector3(.125f, .125f, .125f);
                Color col = Color.white;
                if (f == 1) col = Color.red; 
                if (f == 2) col = Color.green; 
                if (f == 3) col = Color.blue; 
                if (f == 4) col = Color.yellow;
                breadCrumbs[t, f].GetComponent<Renderer>().material.color = col;
            }
        }
    }

    void initRod() {
        rod = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rod.name = "rod";
        rod.GetComponent<Renderer>().material.color = new Color(1, .5f, .5f);
    }

    void initPalm() {
        palms = new GameObject[numFingers];
        for (int f = 0; f < numFingers; f++)
        {
            palms[f] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            palms[f].name = "palm " + f;
            float x = -1.5f * f;
            float y = 0;
            float z = 0;
            palms[f].transform.localScale = new Vector3(1.5f, 1.5f, 4);
            if (f == 0) {
                palms[f].transform.position = new Vector3(x - 2, y, z - 2);
                palms[f].transform.localScale = new Vector3(1.5f, 1.5f, .5f);
                palms[f].transform.eulerAngles = new Vector3(15, 75, 0);
            } else {
                float yaw = -5 * (f - 2);
                float dn = (f - 2) * .25f;
                if (f == 2) dn -= .25f;
                palms[f].transform.position = new Vector3(x, y - dn, z - f * .1f);
                palms[f].transform.localScale = new Vector3(1.5f, 1.5f, 4 - f * .1f);
                palms[f].transform.eulerAngles = new Vector3(0, yaw, (f - 2) * 5);;
            }
        }
    }

    void initSegs() {
        web = GameObject.CreatePrimitive(PrimitiveType.Cube);
        web.name = "web";
        web.transform.localScale = new Vector3(1.1f, .5f, 3.9f);
        //
        segs = new GameObject[numFingers, numSegs];
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                if (s == numSegs - 1)
                {
                    segs[f, s] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                } else {
                    segs[f, s] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                segs[f, s].name = "finger:" + f + " seg:" + s;
                Vector3 sca = new Vector3(1, 1.5f, 2);
                if (f == 0) {
                    sca = new Vector3(1.5f, 1.5f, 1.5f);
                    if (s == 0) sca.z = 3;
                } else {
                    sca.z *= 1 - f * .1f;
                    if (f == 1) sca.z = 1.5f;
                }
                sca *= (1 - s * .1f);
                float c = s / (float)numSegs;
                segs[f, s].GetComponent<Renderer>().material.color = new Color(c, c, c);
                segs[f, s].transform.localScale = sca;
            }
        }
    }

    void initRots() {
        rots = new Vector3[numFingers, numSegs];
        rotsTarget = new Vector3[numFingers, numSegs];
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                rots[f, s] = new Vector3(10, 0, 0);
            }
        }
    }

    void initRotsLimit()
    {
        rotsLimitMin = new Vector3[numFingers, numSegs];
        rotsLimitMax = new Vector3[numFingers, numSegs];
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                if (f == 0)
                {
                    if (s == 0)
                    {
                        rotsLimitMin[f, s] = new Vector3(15, -45, 0);
                        rotsLimitMax[f, s] = new Vector3(60, 0, 0);
                    }
                    if (s == 1)
                    {
                        rotsLimitMin[f, s] = new Vector3(0, -45, 0);
                        rotsLimitMax[f, s] = new Vector3(0, 0, 0);
                    }
                    if (s == 2)
                    {
                        rotsLimitMin[f, s] = new Vector3(0, -45, 0);
                        rotsLimitMax[f, s] = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    if (s == 0)
                    {
                        if (f == 1) {
                            rotsLimitMin[f, s] = new Vector3(15, -5, 0);
                            rotsLimitMax[f, s] = new Vector3(85, 20, 0);
                        } else {
                            rotsLimitMin[f, s] = new Vector3(15, -5, 0);
                            rotsLimitMax[f, s] = new Vector3(85, 5, 0);
                        }
                    }
                    if (s == 1)
                    {
                        rotsLimitMin[f, s] = new Vector3(0, 0, 0);
                        rotsLimitMax[f, s] = new Vector3(60, 0, 0);
                    }
                    if (s == 2)
                    {
                        rotsLimitMin[f, s] = new Vector3(0, 0, 0);
                        rotsLimitMax[f, s] = new Vector3(45, 0, 0);
                    }
                }
            }
        }
    }

    Vector3 getRotsRandom(int f, int s) {
        float pitch = Random.Range(rotsLimitMin[f, s].x, rotsLimitMax[f, s].x);
        float yaw = Random.Range(rotsLimitMin[f, s].y, rotsLimitMax[f, s].y);
        float roll = Random.Range(rotsLimitMin[f, s].z, rotsLimitMax[f, s].z);
        return new Vector3(pitch, yaw, roll);
    }

    void updateRotsSmooth()
    {
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                rots[f, s] = smooth * rots[f, s] + (1 - smooth) * rotsTarget[f, s];
            }
        }
    }

    void updateSegs() {
        for (int f = 0; f < numFingers; f++) {
            for (int s = 0; s < numSegs; s++) {
                placeSeg(f, s);                
            }
        }
        updateWeb();
    }

    void placeSeg(int f, int s) {
        GameObject goPrev = palms[f];
        if (s > 0) {
            goPrev = segs[f, s - 1];         
        }
        GameObject go = segs[f, s];
        go.transform.position = goPrev.transform.position + goPrev.transform.forward * goPrev.transform.localScale.z / 2;
        go.transform.eulerAngles = goPrev.transform.eulerAngles;
        go.transform.Rotate(rots[f, s].x, rots[f, s].y, rots[f, s].z);
        go.transform.position += go.transform.forward * go.transform.localScale.z / 2;
    }
}
