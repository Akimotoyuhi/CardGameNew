using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class PreviewButtonAttribute : PropertyAttribute
{
    public CardDataBase DataBase { get; set; }
}
