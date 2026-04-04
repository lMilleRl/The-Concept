using UnityEngine;

public class Descrtiption : ScriptableObject
{
    [SerializeField] private string _ownDescription;
    
    public string OwnDescription => _ownDescription;
}
