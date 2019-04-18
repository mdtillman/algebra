using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public ParticleSystem midParticles, startParticles, endParticles;
    ParticleSystem.ShapeModule shape;
    LineRenderer line;
    float particlesLength;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        shape = midParticles.shape;
        Off();
    }

    // Update is called once per frame
    void Update()
    {
        particlesLength = Vector2.Distance(line.GetPosition(0), line.GetPosition(1));
        shape.radius = particlesLength / 2f;
        startParticles.transform.position = line.GetPosition(0);
        endParticles.transform.position = line.GetPosition(1);
        midParticles.transform.position = (line.GetPosition(0) + line.GetPosition(1)) / 2f;
        midParticles.transform.LookAt(line.GetPosition(0));
        if (line.GetPosition(0).x < line.GetPosition(1).x)
        {
            midParticles.transform.localEulerAngles = new Vector3(0f,
                                                            0f,
                                                            midParticles.transform.localEulerAngles.x);
        }
        else
        {
            midParticles.transform.localEulerAngles = new Vector3(0f,
                                                            0f,
                                                            -midParticles.transform.localEulerAngles.x);
        }
    }

    public void On()
    {
        startParticles.Play();
        endParticles.Play();
        midParticles.Play();
    }

    public void Off()
    {
        //Debug.Log("Off!");
        startParticles.Stop();
        endParticles.Stop();
        midParticles.Stop();
    }
}
