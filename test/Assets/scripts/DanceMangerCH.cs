using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceMangerCH : MonoBehaviour
{
    Animator anim;
    List<string> animNames = new List<string>{"one", "two", "three"};
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void setFalse()
    {
        for(int i=0;i<3;i++)
        {
            anim.SetBool(animNames[i], false);
        }
        
    }

    public void randomAnimation()
    {
        int r = Random.Range(0, 3);
        anim.SetBool(animNames[r], true);
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("temp"))
        {
            setFalse();
            randomAnimation();
        }

    }
}
