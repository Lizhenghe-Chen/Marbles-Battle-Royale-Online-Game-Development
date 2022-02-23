using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is the Mune Class for each meune page
public class Menu : MonoBehaviour
{
    public string menuName;

    public bool open;

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    void Start()
    {
    }

    void Update()
    {
    }
}
