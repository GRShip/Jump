using UnityEngine;

public class testt : MonoBehaviour
{
    GameObject player;
    GameObject head;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        player = GameObject.FindWithTag("Player");
        head = GameObject.Find("Head");

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = head.transform.position;
            transform.rotation = head.transform.rotation;
        }
    }
}
