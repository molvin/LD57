using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilitiesUiCotroller : MonoBehaviour
{
    public TextMeshProUGUI ghost;
    public GameObject abilityLayoutGroup;

    public TextMeshProUGUI abilityPrefab;

    public float moveTime = 1;
    public float expandTime = 1;
    public AnimationCurve moveCurve;

    private List<TextMeshProUGUI> instances = new();

    public void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddAbility(Vector3.zero, "BABBIDOO");
        }
        */
    }

    public void AddAbility(Vector3 worldPos, string name)
    {
        StartCoroutine(lerpToCorner(worldPos, name));
    }
    public void ClearAbilities()
    {
        foreach(var text in instances)
        {
            Destroy(text.gameObject);
        }
        instances.Clear();
    }

    public IEnumerator lerpToCorner(Vector3 worldPos, string name)
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
        TextMeshProUGUI instance = GameObject.Instantiate(abilityPrefab, abilityLayoutGroup.transform);
        instance.text = "";
        while (elapsedTime < expandTime)
        {
            ghost.transform.position = Vector3.Lerp(startPos, instance.transform.position, moveCurve.Evaluate(elapsedTime / expandTime));
            ghost.fontSize = Mathf.Lerp(32, 22, (elapsedTime / expandTime));

            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        ghost.gameObject.SetActive(false);
        instance.text = ghost.text;
        ghost.fontSize = 22;

        instances.Add(instance);

        yield return 0;
    }


}
