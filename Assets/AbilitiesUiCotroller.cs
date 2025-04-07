using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilitiesUiCotroller : MonoBehaviour
{
    public TextMeshProUGUI ghost;
    public GameObject abilityLayoutGroup;


    public TextMeshProUGUI abilityPrefab;

    public float moveTime = 1;
    public float expandTime = 1;
    public AnimationCurve moveCurve;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddAbility(Vector3.zero, "BABBIDOO");
        }
    }

    public void AddAbility(Vector3 worldPos, string name)
    {
    
        StartCoroutine(lerpToCorner(worldPos));
    }

    public IEnumerator lerpToCorner(Vector3 worldPos)
    {
        ghost.text = name;
        Vector3 startPos = Camera.main.WorldToScreenPoint(worldPos);
        ghost.transform.position = startPos;
        ghost.gameObject.SetActive(true);

        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            ghost.fontSize = Mathf.Lerp(22, 32, (elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        elapsedTime = 0;
        while (elapsedTime < expandTime)
        {
            ghost.transform.position = Vector3.Lerp(startPos, abilityLayoutGroup.transform.position, moveCurve.Evaluate(elapsedTime / expandTime));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        ghost.gameObject.SetActive(false);
        TextMeshProUGUI instance = GameObject.Instantiate(abilityPrefab, abilityLayoutGroup.transform);
        instance.text = ghost.text;
        ghost.fontSize = 22;

        yield return 0;
    }


}
