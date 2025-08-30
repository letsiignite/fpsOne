using UnityEngine;
/// <summary>
/// SPecifies methods that deal with processing of damage caused by bullet hit or granade 
/// </summary>
/// 
namespace Game
{
    public interface IDamageHandler
    {
        public void ProcessDamage(float damageMultiplyer, float damage);
    }
}