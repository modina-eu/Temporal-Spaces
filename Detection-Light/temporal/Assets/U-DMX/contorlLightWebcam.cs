using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using neoludicGames.uDmx;
using OscSimpl.Examples;
public class contorlLightWebcam : MonoBehaviour
{
    public DmxLightSourceTemporal l1;
    public PoseEstimator1 script;

    public float si;
    public float si2;
    public float si3;
    public float si4;
    public float si5;
    public GettingStartedReceiving osc;

    public float pe1 = 0f;
    private bool startpe = true;
    private float startTime;  
    private float pe2 = 0f;
    private bool startpe2 = true;
    private float startTime2;
    private float pe3 = 0f;
    private bool startpe3 = true;
    private float startTime3;
    private bool stoppe = true;
    private float stopTime;
    private bool stoppe2 = true;
    private float stopTime2;
    private bool stoppe3 = true;
    private float stopTime3;
    public float pha = 0f;
    private bool startpha = true;
    private float phaTime;
    private bool phastop = true;
    private float phastopTime;
    public Vector3 color;
    [Range (0,1)]
    public float finpha;
    void Start()
    {
    }
    float abs(float a) { return Mathf.Abs(a); }
    float sin(float a) { return Mathf.Sin(a); }
    float sm(float edge0, float edge1, float x){
        float t = Mathf.Clamp01((x - edge0) / (edge1 - edge0));
        return t* t * (3.0f - 2.0f * t);}
    float dist(float a, float b) { return Mathf.Pow(1-Mathf.Abs(a- sm(si4, si5, b)),5); }
    float distance(float a, float b) { return  Mathf.Abs(a -b); }
    float fract(float t) { return t - Mathf.Floor(t); }
    Vector3 fract3(Vector3 t)
    {
        return t - new Vector3(Mathf.Floor(t.x), Mathf.Floor(t.y), Mathf.Floor(t.z));
    }
    float rd(float x) { float fx = Mathf.Floor(x); return fract(Mathf.Sin(Vector2.Dot(new Vector2(fx, fx), new Vector2(54.56f, 54.56f))) * 7845.236f); }
    float no(float x) { return sm(0.3f,0.7f,Mathf.Lerp(rd(x), rd(x + 1), Mathf.SmoothStep(0, 1, fract(x)))); }
    float map(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
    }
    Vector3 clamp3( Vector3 x) { return new Vector3(Mathf.Clamp01(x.x),Mathf.Clamp01(x.y),Mathf.Clamp01(x.z)); }
    Vector3 hue(float x)
    {
        Vector3 vv = new Vector3(0, -1.0f / 3, 1.0f / 3);
        Vector3 x3 = new Vector3(x,x,x);
       
        Vector3 v1 =  new Vector3(1, 1, 1)-2 *fract3(new Vector3(x, x, x )+ new Vector3(0, -1.0f / 3, 1.0f / 3));
        return clamp3(3 * new Vector3(Mathf.Abs(v1.x), Mathf.Abs(v1.y), Mathf.Abs(v1.z)) - new Vector3(1, 1, 1));
    }
    public float step(float edge, float x)
    {
        return x >= edge ? 1.0f : 0.0f;
    }
    float lerp(float a, float b, float x)
    {
        return Mathf.Lerp(a, b, x);
    }
    Vector3 lerp3(Vector3 a, Vector3 b, Vector3 x)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, x.x), Mathf.Lerp(a.y, b.y, x.y), Mathf.Lerp(a.z, b.z, x.z));
    }
    Vector3 lerp3x(Vector3 a, Vector3 b, float x)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, x), Mathf.Lerp(a.y, b.y, x), Mathf.Lerp(a.z, b.z, x));
    }
    float pow(float a,float b) { return Mathf.Pow(a, b); }
    float mix(float a, float b ,float x) { return Mathf.Lerp(a, b, x); }
    float max(float a,float b) { return Mathf.Max(a, b); }

    void Update()
    {
        float pp = -10;
        float pp2 = 0;
        float pp3 = -10;
        float pp4 = -10;
        float pp5 = -10;
        float pp6 = -10;
        if (script.person1 > 1)
        {
            float ppt2 = map(script.pp, 0.18f, 0.38f, 0.0f, 1.0f);
             pp2 = map(script.pos1[7].x, lerp(0.0f, 0.23f, ppt2), lerp(1.0f, 0.7f, ppt2), 0.0f, 1.0f);
             pp = map(script.pos1[7].z, lerp(0.23f, 0.0f, pp2), lerp(0.7f, 1.0f, pp2), 0.0f, 1.0f);
            if (!startpe) 
            {
                startTime = Time.time;
                startpe = true; 
            }
            pe1 = Mathf.Clamp((Time.time - startTime)*2f,0f,2f);
            stoppe = false;
        }
        else
        {                                                                                                                   
            startpe = false;
            pp2 =0;
            pp = -10;
            if (!stoppe)
            {
                stopTime = Time.time;
                stoppe = true;
            }
            pe1 = 2-Mathf.Clamp((Time.time - stopTime) * 2f, 0f, 2f);

        }

        if (script.person2 > 1)
        {
        float ppt3 = map(script.ppb, 0.18f, 0.38f, 0.0f, 1.0f);
         pp4 = map(script.pos2[7].x, lerp(0.0f, 0.23f, ppt3), lerp(1.0f, 0.7f, ppt3), 0.0f, 1.0f);
         pp3 = map(script.pos2[7].z, lerp(0.23f, 0.0f, pp4), lerp(0.7f, 1.0f, pp4), 0.0f, 1.0f);

            if (!startpe2)
            {
                startTime2 = Time.time;
                startpe2 = true;
            }
            pe2 = (Time.time - startTime2) * 2f;
        }
        else
        {
            pp4 = -10;
            pp3 = -10;
            startpe2 = false;
            pe2 = -10;
        }
        if (script.person3 > 1)
        {
            float ppt5 = map(script.ppc, 0.18f, 0.38f, 0.0f, 1.0f);
            pp6 = map(script.pos3[7].x, lerp(0.0f, 0.23f, ppt5), lerp(1.0f, 0.7f, ppt5), 0.0f, 1.0f);
            pp5 = map(script.pos3[7].z, lerp(0.0f, 0.23f, pp6), lerp(1.0f, 0.7f, pp6), 0.0f, 1.0f);
            if (!startpe3)
            {
                startTime3 = Time.time;
                startpe3 = true;
            }
            pe3 = (Time.time - startTime3) * 2f;
        }
        else
        {
            pp6 = -10;
            pp5 = -10;
            startpe3 = false;
            pe3 = -10;
        }

        float sp = lerp(0.01f,0.3f,pow(max(osc.HandL, osc.HandR), si));
        float lp = lerp(70f, 30f, pow(max(osc.AnkleL, osc.AnkleR), si));
        float sp1 = lerp(0.01f, 0.3f, pow(max(osc.HandL2, osc.HandR2), si));
        float lp1 = lerp(70f, 30f, pow(max(osc.AnkleL2, osc.AnkleR2), si));
        float sp2 = lerp(0.01f, 0.3f, pow(max(osc.HandL3, osc.HandR3), si));
        float lp2 = lerp(70f, 30f, pow(max(osc.AnkleL3, osc.AnkleR3), si));

        if (osc.Phase2 == 1)
        {                             
            if (!startpha)
            {
                phaTime = Time.time;
                startpha = true;
            }
            pha = Mathf.Clamp((Time.time - phaTime) * 2f, 0f, 3f);
            phastop = false;
        }
        else
        {
            startpha = false;
            if (!phastop)
            {
                phastopTime = Time.time;
                phastop = true;
            }
            pha = 2 - Mathf.Clamp((Time.time - phastopTime) * 2f, 0f, 3f);

        }
        float pv4 = pow(osc.MaxSpeed , 2f)*osc.Phase2;
        for (int i = 0; i < 192; i++)
        {
            float a = i / 192.0f;
             float v1 = max(pow(Mathf.Clamp01(1 - distance(pp2, a)), lp) * sp, pow(Mathf.Clamp01(1 - distance(pp4, a)),lp1) * sp1);
             v1 = max(v1, pow(Mathf.Clamp01(1 - distance(pp6, a)), lp2) * sp2);
             float v2 = pow(max(Mathf.Clamp01(1 - distance(pe1, a)), max(Mathf.Clamp01(1 - distance(pe2, a)), Mathf.Clamp01(1 - distance(pe3, a)))), 100) * si4;
            float c = fract((i - 1) / 3.0f);
            float m1 = step(0.4f, c);
            float m2 = step(c, 0);
            float mb3 = Mathf.Max(m1 + m2);
            float m3 = 1 - mb3;
            float v3 = Mathf.Max(Mathf.Max(0.8f * m1, m2), 0.2f * m3) * .2f * pow(Mathf.Clamp01(1 - distance(pha,a)), 40f);            
            float v4 = pow(sin(sin(a * 20f )*3.14f+pv4*5)*0.5f+0.5f,1.5f) * max(m1*color.x, max(m2 * color.y, m3*color.z) ) * pv4*0.075f;
            l1.SetStrength(i, max(max(v3,max(v1,v2)*(1- osc.Phase2)),v4));
        }

        for (int i = 193; i < 384; i++)
        {
            float a = i / 192.0f - 1;
            float v1 = max(pow(Mathf.Clamp01(1 - distance( pp, a)), si2) * sp, pow(Mathf.Clamp01(1 - distance( pp3, a)), si2) * sp1) ;
            v1 = max(v1, pow(Mathf.Clamp01(1 - distance(pp5, a)), si2) * sp2);
            float me = lerp(a, 1 - a, step(0.5f, a));
            float   v2 = pow(max(Mathf.Clamp01(1 - distance(pe1 - 1, me)), max(Mathf.Clamp01(1 - distance(pe2 - 1, me)), Mathf.Clamp01(1 - distance(pe3 - 1, me)))), 100) * si4; 
            float c = fract((i - 1) / 3.0f);
            float m1 = step(0.4f, c);
            float m2 = step(c, 0);
            float mb3 = Mathf.Max(m1 + m2);
            float m3 = 1 - mb3;
            float v3 = Mathf.Max(Mathf.Max(0.8f * m1, m2), 0.2f * m3) * .2f * pow(Mathf.Clamp01(1 - distance(pha-1, a)), 40f);
            float v4 = pow(sin(sin(a * 20f) * 3.14f + pv4 * 5f) * 0.5f + 0.5f, 1.5f) * max(m1 * color.x, max(m2 * color.y, m3 * color.z)) * pv4 * 0.075f;
            l1.SetStrength(i, max(max(v3, max(v1, v2) * (1 - osc.Phase2)),v4));
        }

    }















    //dis = Mathf.Pow( Mathf.Abs(script.pos4.x - script.pos3.x),2)*si2;
    //col1 = Vector3.SmoothDamp(col1, new Vector3(dis , script.pos4.y, script.pos3.y ), ref velocity, smoothTime);
    //col2 = Vector3.SmoothDamp(col1, new Vector3(0, script.pos4.z, script.pos3.z), ref velocity2, smoothTime);
    //dis = script.pos7.x;
    //dis2 = script.pos7.y;
    //float time = Time.time;
    /*for (int  i = 0; i <  48; i++)
    {
        float a = (i ) / 48.0f;
        float b = Mathf.Floor(i / 3.0f)*3 / 48.0f;
        // Vector3 h = hue(a)*0.5f;
        float c = fract((i-1) / 3.0f);
        float m1 = step(0.4f, c);
        float m2 = step( c,0);
        float m3 = 1- Mathf.Max(m1 + m2);
        /* float tt = Time.time * 4;
         float c2 = step(Mathf.Lerp(0.4f, 0.2f, step(0.5f, fract(tt))), fract(distance(a + 0.5f / 16, 0.5f) * fract(Mathf.Floor(Time.time / 16.0f) / 4) * 4));
         float c3 = Mathf.Lerp(c2, 1- c2, step(0.5f, fract(tt * 0.5f)));
         Vector3 c4b = lerp3x( new Vector3(1, 0, 0),new Vector3(0, 0, 0), step(0.5f, fract(tt * 0.125f)));
         Vector3 c4 = lerp3x(c4b, lerp3x(new Vector3(0, 0, 1), new Vector3(0, 0, 0), step(0.5f, fract(tt * 0.25f))), c3);   */
    //float g1 = Mathf.Sin(distance(a, Mathf.Sin(Time.time) * 0.5f + 0.5f) * 10- Time.time * 10) * 0.5f + 0.5f;
    //float r1 = rd(a * 16+ 65+ Time.time * 2);
    //Vector3 c4 = new Vector3(rd(a * 16 + 125 + Time.time * 2), r1, r1);
    //float r = Mathf.Pow(fract(t*si),5*si2);
    //float r2 = step(0.5f, fract(b * si4+t*si3))*0.2f;
    /*float pp = pow(fract(time * 0.125f), 1.5f);
    float v1 = pow(fract(time * 3), (1- pp) * 4);
    float v2 = step(0.5f, fract(b * 4+ 40 * pp));
    float v3 = mix(v1, v2, step(0.5f, fract(time * 0.125f)));
    l1.SetStrength(i, pow(v3,2)*0.5f);
}             */
    //float p7 =  Mathf.SmoothDamp(previousHipPosition, script.pos7.x, ref smoothDampVelocity, 0.1f);
    //"nose", "leftShoulder", "rightShoulder", "leftElbow", "rightElbow", "leftWrist", "rightWrist",  "Hip", "leftKnee", "rightKnee", "leftAnkle", "rightAnkle"
    //tt = 100.0f-max(max(osc.HandL, osc.HandR), max(osc.ElL, osc.ElR))*2000.0f;
    /* tt = Vector2.Distance(new Vector2(script.pos5.x, script.pos5.y), lastPositione1) / Time.deltaTime;
     if (float.IsNaN(tt))
     {
         // Handle the NaN case, perhaps reset tt2 to a default value
         tt = 0.0f; // Or any other appropriate default value
     }
     tt2 += tt - si;
     tt2 = Mathf.Clamp01(tt2);
     //tt2 = Mathf.Clamp01(tt2);
     lastPositione1 = new Vector2(script.pos4.x, script.pos4.y);

       */
}
