using TMPro;
using UnityEngine;

public class MainMenuTextEditor : MonoBehaviour
{
    public TMPro.TextMeshProUGUI glowt;
    public TMPro.TextMeshProUGUI emmtw;
    public TMPro.TextMeshProUGUI eretol;
    public TMPro.TextMeshProUGUI molvin;
    public TMPro.TextMeshProUGUI tannimum;
    public TMPro.TextMeshProUGUI landberglife;
    public TMPro.TextMeshProUGUI mechanicallife;


    [SerializeField]
    private int _index = 0;
    public int test
    {
        set
        {
            _index = value;
            UpdateTexts();
        }
    }

    public void Increment()
    {
        _index = _index + 1;
        UpdateTexts();

    }

    private void UpdateTexts()
    {
        glowt.text = "glowt".Substring(0, GetClampToLength("glowt", 0));
        molvin.text = "molvin".Substring(0, GetClampToLength("molvin", 2));
        emmtw.text = "emmtw".Substring(0, GetClampToLength("emmtw", 4));
        eretol.text = "eretol".Substring(0, GetClampToLength("eretol", 6));
        tannimum.text = "Tannimum".Substring(0, GetClampToLength("tannimum", 8));
        landberglife.text = "landberglife".Substring(0, GetClampToLength("landberglife", 10));
        mechanicallife.text = "mechanicallife".Substring(0, GetClampToLength("mechanicallife", 12));

    }
    private int GetClampToLength(string text, int offset) { return Mathf.Clamp(text.Length + offset - _index, 0, text.Length); }
 


}
