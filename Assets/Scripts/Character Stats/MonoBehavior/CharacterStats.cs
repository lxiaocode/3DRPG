using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO _characterDataSo;

    public int MaxHealth
    {
        get { return _characterDataSo == null ? 0 : _characterDataSo.maxHealth; }
        set => _characterDataSo.maxHealth = value;
    }
    
    public int CurrentHealth
    {
        get { return _characterDataSo == null ? 0 : _characterDataSo.currentHealth; }
        set => _characterDataSo.currentHealth = value;
    }
    
    public int BaseDefence
    {
        get { return _characterDataSo == null ? 0 : _characterDataSo.baseDefence; }
        set => _characterDataSo.baseDefence = value;
    }
    
    public int CurrentDefence
    {
        get { return _characterDataSo == null ? 0 : _characterDataSo.currentDefence; }
        set => _characterDataSo.currentDefence = value;
    }
}
