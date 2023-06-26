using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    public bool isCritical;

    public int MaxHealth
    {
        get { return characterData == null ? 0 : characterData.maxHealth; }
        set => characterData.maxHealth = value;
    }
    
    public int CurrentHealth
    {
        get { return characterData == null ? 0 : characterData.currentHealth; }
        set => characterData.currentHealth = value;
    }
    
    public int BaseDefence
    {
        get { return characterData == null ? 0 : characterData.baseDefence; }
        set => characterData.baseDefence = value;
    }
    
    public int CurrentDefence
    {
        get { return characterData == null ? 0 : characterData.currentDefence; }
        set => characterData.currentDefence = value;
    }

    #region Character Combat

    /// <summary>
    /// 计算伤害，并应用伤害
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defener"></param>
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);

        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        
        // TODO 更新UI
        // TODO 更新经验
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <returns></returns>
    private int CurrentDamage()
    {
        // 计算基本伤害
        float damage = UnityEngine.Random.Range(attackData.minDamge, attackData.maxDamage);
        
        // 计算暴击
        if (isCritical)
        {
            damage *= attackData.criticalMultiplier;
            Debug.Log("暴击 " + damage);
        }
        
        return (int)damage;
    }

    #endregion
}
