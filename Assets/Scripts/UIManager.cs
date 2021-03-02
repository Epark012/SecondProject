using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI mph;
    public TextMeshProUGUI fps;
    public virtual void changeText(float speed)
    {
        float s = speed * 2.23694f; //속도를 마일 단위로 얻음. 시간당 속도.
        mph.text = Mathf.Clamp(Mathf.Round(s), 0f, 1000f) + "MPH";
    }
     void Update()
    {
       fps.text = (Mathf.Round(1f / Time.deltaTime)).ToString() + "FPS"; //초당 프레임수 
  }
}
