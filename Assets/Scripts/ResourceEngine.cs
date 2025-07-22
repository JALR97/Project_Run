using System;
using UnityEngine;

public class ResourceEngine : MonoBehaviour
{
//-----------------//Data structures//-----------------//
//enums


//structs


//-----------------//Components//-----------------//
//Internal Components


//Prefabs


//External References
    [SerializeField] private PlayerController playerController;
    
//-----------------//Variables//-----------------//
//Process variables - private
    private float _volition;
    private float _muscleStrain;
    private float _stamina;
    private float _temperature;
    private int _stability;
    

//Balance variables - serialized 


//Public properties - private set "Name { get; private set; }"


//-----------------//Functions//-----------------//
//Built-in
    private void Start() {
        _volition = 100f;
        _muscleStrain = 0f;
        _stamina = 100f;
        _temperature = 37f;
        _stability = 5;
    }

//Inner process - private


//External interaction - public

}
