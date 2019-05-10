using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using TMPro;

public class gregg : MonoBehaviour
{
    private enum GreggState { OFF, APPEARING, DISAPPEARING };   //enum for gregg action states
    private enum BubbleState { OFF, APPEARING, DISAPPEARING };  //enum for bubble action states
    
    #region Constants
    private const float _TIME_TO_FADE = 2.0f;      //modifier for fade in/out time for bubble. Higher is faster
    private const float _RADIUS = 1.5f;            //radius btwn player and gregg
    private const float _START_POSITION = 120f;    //angle from the player's view where gregg will initially appear
    private const float _GREGG_Y = -0.58f;         //Y position where he looks like he's standing and not floating
    #endregion Constants
    
    #region Public Variables
    public bool             inGregg;        //flag letting other objects know if gregg is gregging
    public TextMeshPro      proText;        //text inside speech bubble attached to gregg
    public GameObject       bubble;         //speech bubble attached to gregg
    #endregion Public Variables

    #region Private Variables
    private GreggState      _greggState;    //Gregg's current state
    private BubbleState     _bubbleState;   //Text bubble's current state
    private bool            _appeared;      //used to help reset Gregg when the scene resets
    private SpriteRenderer  _bubbleRndr;    //Renderer for the text bubble
    private Color           _bubbleAlpha;   //text bubble's color, but with alpha == 0
    private Color           _bubbleColor;   //original color for the text bubble
    private TextMeshProUGUI    _textRndr;   //Renderer for the text
    private Color           _textAlpha;     //text's color, but with alpha == 0
    private Color           _textColor;     //original color for the text
    #endregion Private Variables

    void Start()
    {
        //Initialize variables
        _appeared = false;
        _bubbleRndr = bubble.GetComponent<SpriteRenderer>();
        _bubbleAlpha = _bubbleRndr.material.color;
        _bubbleColor = _bubbleAlpha;
        _bubbleAlpha.a = 0;
        _bubbleRndr.material.color = _bubbleAlpha;
        
        _textAlpha = proText.color;
        _textColor = _textAlpha;
        _textAlpha.a = 0;
        proText.color = _textAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q;
        float angleFromViewpoint;
        switch (_greggState)
        {
            case GreggState.APPEARING:
                // Gregg will appear behind the player, then walk in an arc to them
                Vector3 left = Vector3.Cross(Camera.main.transform.position - transform.position, -Vector3.up);
                transform.RotateAround(Camera.main.transform.position, Vector3.up, -60 * Time.deltaTime);
                transform.LookAt(left);
                q = new Quaternion(0, transform.rotation[1], 0, transform.rotation[3]);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, 360);
                angleFromViewpoint = Vector3.Angle(RemoveY(Camera.main.transform.forward), RemoveY(transform.position));
                if (Mathf.Floor(angleFromViewpoint) == 0)
                {
                    transform.LookAt(Camera.main.transform);
                    q = new Quaternion(0, transform.rotation[1], 0, transform.rotation[3]);
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, 360);
                    _greggState = GreggState.OFF;
                }
                break;
            case GreggState.DISAPPEARING:
                // Gregg will appear move in an arc around the player until he's out of sight, then disappear
                Vector3 right = Vector3.Cross(Camera.main.transform.position - transform.position, Vector3.up);
                transform.RotateAround(Camera.main.transform.position, Vector3.up, 60 * Time.deltaTime);
                transform.LookAt(right);
                q = new Quaternion(0, transform.rotation[1], 0, transform.rotation[3]);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, 360);
                angleFromViewpoint = Vector3.Angle(RemoveY(Camera.main.transform.forward), RemoveY(transform.position));
                if (Mathf.Floor(angleFromViewpoint) >= 150)
                {
                    _greggState = GreggState.OFF;
                    gameObject.transform.position += new Vector3(0f, -4f, 0f);
                }
                break;
            default:
                ;
                break;
        }

        switch (_bubbleState)
        {
            case BubbleState.APPEARING:
                //bubble fades in
                _bubbleRndr.material.color = Color.Lerp(_bubbleRndr.material.color, _bubbleColor, _TIME_TO_FADE * Time.deltaTime);
                proText.color = Color.Lerp(proText.color, _textColor, _TIME_TO_FADE * Time.deltaTime);
                break;
            case BubbleState.DISAPPEARING:
                //bubble fades out
                _bubbleRndr.material.color = Color.Lerp(_bubbleRndr.material.color, _bubbleAlpha, _TIME_TO_FADE * Time.deltaTime);
                proText.color = Color.Lerp(proText.color, _textAlpha, _TIME_TO_FADE * Time.deltaTime);
                break;
            default:
                ;
                break;
        }
    }

    //Used to reset gregg when the scene is reset
    public void resetGregg()
    {
        _appeared = false;
        _greggState = GreggState.OFF;
        transform.position = new Vector3(0f,-500f,0f);
    }

    //Start routine to make Gregg appear
    public void greggAppear()
    {
        StartCoroutine(greggAppearHelper());
    }

    //Start routine to make Gregg disappear
    public void greggDisappear()
    {
        StartCoroutine(greggDisappearHelper());
    }

    //Coroutine to make Gregg walk into view
    public IEnumerator greggAppearHelper()
    {
        if (!_appeared)
        {
            yield return new WaitForSeconds(3);
            transform.position = new Vector3(Mathf.Sin(_START_POSITION * Mathf.Deg2Rad) * _RADIUS, _GREGG_Y, Mathf.Cos(_START_POSITION * Mathf.Deg2Rad) * _RADIUS);
            inGregg = true;
            _appeared = true;
            _greggState = GreggState.APPEARING;
        }
    }

    //Coroutime to make Gregg walk off-camera
    public IEnumerator greggDisappearHelper()
    {
        _bubbleState = BubbleState.APPEARING;
        yield return new WaitForSeconds(4);
        _bubbleState = BubbleState.DISAPPEARING;
        yield return new WaitForSeconds(3);
        _greggState = GreggState.DISAPPEARING;
    }

    //Helper method to replace the y value of a vector
    private static Vector3 ReplaceY(Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    //Special case of ReplaceY
    private static Vector3 RemoveY(Vector3 v)
    {
        return ReplaceY(v, 0);
    }
}
