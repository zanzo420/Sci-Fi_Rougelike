//Copyright © Darwin Willers 2017

using System;
using System.Collections;
using UnityEngine;

public class OutdoorCamera : MonoBehaviour
{
    public static OutdoorCamera Instance;

    public float scrollSpeed = 5f;
    [Range(0.01f, 0.2f)]
    public float scrollArea = .1f;

    public int minZoom = 10;
    public int maxZoom = 5;
    [Range(1f, 5f)]
    public float ZoomSpeed = 3f;

    private Camera _cam;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Debug.LogError("Second outdoor Camera apeard: " + name);
            Destroy(this);
            return;
        }
        
        _cam = Camera.main;        
    }

    /// <summary>
    /// Negative amount means zooming in, positiv means zooming out
    /// </summary>
    /// <param name="amount"></param>
    public void Zoom(float amount)
    {
        //TODO - Fix max and min zoom
        var newHeight = transform.position.y + amount;
        Mathf.Clamp(newHeight, maxZoom, minZoom);
        StopAllCoroutines();
        StartCoroutine(SmoothZoom(newHeight));
    }

    private void Update()
    {
        var screensize = new Vector2(Screen.width, Screen.height);
        var areaX = screensize.x * scrollArea;
        var areaY = screensize.y * scrollArea;

        var dir = new Vector3();
        var mousePos = Input.mousePosition;

        if (mousePos.x < areaX) dir.x -= 1;
        else if (mousePos.x > screensize.x - areaX) dir.x += 1;
        
        if (mousePos.y < areaY) dir.z -= 1;
        else if (mousePos.y > screensize.y - areaY) dir.z += 1;
        
        if(dir != Vector3.zero)
            Scroll(dir);


    }

    private void Scroll(Vector3 dir)
    {
        _cam.transform.Translate( dir * scrollSpeed * Time.deltaTime, Space.World);
    }

    private IEnumerator SmoothZoom(float destination)
    {
        do
        {
            var currentZoom = transform.position.y;
            var dir = currentZoom <= destination ? 1f : -1f;
            var disToMove = dir * ZoomSpeed * Time.deltaTime;
            if (Mathf.Abs(disToMove) > Mathf.Abs(currentZoom - destination))
                disToMove = destination - currentZoom;

            transform.Translate(new Vector3(0, disToMove, 0), Space.World);

            yield return null;

        } while (Math.Abs(transform.position.y - destination) > 0.01f);
    }
}