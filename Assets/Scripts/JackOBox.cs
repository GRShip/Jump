using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackOBox : MonoBehaviour
{
    float time = 0;
    public float cooltime = 2f;
    float pushtime = 0.2f;      //피격 시 PC가 래그돌 되는 시간

    enum state
    {
        Hiding,
        Boo,
        Stop,
        Back
    }

    state stat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stat = state.Hiding;
    }

    // Update is called once per frame
    void Update()
    {
        if (stat == state.Hiding)
        {
            time += Time.deltaTime;

            if (time >= cooltime)
            {
                stat = state.Boo;
            }
        }

        if (stat == state.Boo)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 10);
            if (transform.localPosition.y > transform.localScale.y * 1.5)
            {
                transform.Translate(new Vector3(0, 0, 0));
                stat = state.Stop;
                StartCoroutine(Stop());
            }
        }

        if (stat == state.Back)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 0.5f);
            if (transform.localPosition.y <= 0)
            {
                transform.Translate(new Vector3(0, 0, 0));
                stat = state.Hiding;
                time = 0;
            }
        }
    }

    IEnumerator Stop()
    {
        yield return new WaitForSeconds(1f);
        stat = state.Back;
    }
}
