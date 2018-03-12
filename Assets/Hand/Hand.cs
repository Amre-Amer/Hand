using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
    int numFingers = 5;
    int numSegs = 3;
    Vector3[,] rots;
    Vector3[,] rotsTarget;
    [Range(0, 90)]
    public float range = 30;
    GameObject[,] segs;
    GameObject[] palms;
    float smooth = .95f;
    [Range(1, 5)]
    public float delay;
    float timeDelay;

    // Use this for initialization
	void Start () {
        initRots();
        initPalm();
        initSegs();
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.realtimeSinceStartup - timeDelay > delay)
        {
            updateRotsTarget();
            timeDelay = Time.realtimeSinceStartup;
        }
        updateRots();
        updateSegs();
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
                palms[f].transform.position = new Vector3(x, y - 1, z);
                palms[f].transform.localScale = new Vector3(1.5f, 1.5f, 4);
                palms[f].transform.eulerAngles = new Vector3(15, 53, -30);
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
        segs = new GameObject[numFingers, numSegs];
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                segs[f, s] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segs[f, s].name = "finger:" + f + " seg:" + s;
                Vector3 sca = new Vector3(1, 1.5f, 2);
                if (f == 0) {
                    sca = new Vector3(1.5f, 1.5f, 1);
                } else {
                    sca.z *= 1 - f * .1f;
                    if (f == 1) sca.z = 1.5f;
                }
                segs[f, s].transform.localScale = sca * (1 - s * .1f);
                float c = s / (float)numSegs;
                segs[f, s].GetComponent<Renderer>().material.color = new Color(c, c, c);
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

    void updateRotsTarget() {
        for (int f = 0; f < numFingers; f++)
        {
            for (int s = 0; s < numSegs; s++)
            {
                float pitch = Random.Range(-range, range);
                float yaw = Random.Range(-range, range);
                float roll = Random.Range(-range, range);
                rotsTarget[f, s] = new Vector3(pitch, yaw, roll);
            }
        }
    }

    void updateRots()
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
    }

    void placeSeg(int f, int s) {
        GameObject goPrev = palms[f];
        if (s > 0) {
            goPrev = segs[f, s - 1];         
        }
        GameObject go = segs[f, s];
        go.transform.position = goPrev.transform.position + goPrev.transform.forward * goPrev.transform.localScale.z / 2;
        go.transform.eulerAngles = goPrev.transform.eulerAngles + rots[f, s];
        go.transform.position += go.transform.forward * go.transform.localScale.z / 2;
    }
}
