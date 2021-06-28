using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public Character_Data _characterData;

    public int _characterID;
    void Start()
    {
        _characterID = _characterData.characterPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(_characterID >= 3)
            {
                _characterID = 1;
            }
            else
            {
                _characterID++;
            }
        }
    }
}
