using System.Collections;
using UnityEngine;

public class GameUiController : MonoBehaviour
{

    public Animator countDownAnimator;
    public TMPro.TextMeshProUGUI timer;

    

    private int state = 0;

    private float _timer = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            state += 1; 
            if(state == 1)
            {
                StartCoroutine(SetTextAfterTime(timer, 0, "3"));
                StartCoroutine(SetTriggerAfterTime(countDownAnimator, 0, "start_countdown"));

                StartCoroutine(SetTextAfterTime(timer, 1, "2"));
                StartCoroutine(SetTriggerAfterTime(countDownAnimator, 1, "start_countdown"));

                StartCoroutine(SetTextAfterTime(timer, 2, "1"));
                StartCoroutine(SetTriggerAfterTime(countDownAnimator, 2, "start_countdown"));

                StartCoroutine(SetTextAfterTime(timer, 3, "GO"));
                StartCoroutine(SetTriggerAfterTime(countDownAnimator, 3, "start_countdown"));
                StartCoroutine(SetTriggerAfterTime(countDownAnimator, 4, "move_corner"));
                StartCoroutine(SetStateAfter(4, 2));
                _timer = 0;




            }
            if (state == 3)
            {
                countDownAnimator.SetTrigger("move_middle");
            }
            if(state == 4) state = 0;

        }
        if(state == 2)
        {
            _timer += Time.deltaTime;
            timer.text = _timer.ToString(".0##");

        }

    }
    public IEnumerator SetStateAfter(float wait_time, int new_state)
    {
        yield return new WaitForSeconds(wait_time);
        state = new_state;

    }


    public IEnumerator SetTextAfterTime(TMPro.TextMeshProUGUI obj, float wait_time, string text)
    {
        yield return new WaitForSeconds(wait_time);
        obj.text = text;
        
    }

    public IEnumerator SetTriggerAfterTime(Animator anim, float wait_time, string trigger)
    {
        yield return new WaitForSeconds(wait_time);
        anim.SetTrigger(trigger);

    }

}
