using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// �R�}���h���s�֐��ɓn���ۂɃR�}���h�Q�ɏ��𑫂��Ƃ��Ɏg���\����
/// </summary>
public struct CommandsInfomation
{
    public List<Command> Commands { get; set; }
    public Sprite Sprite { get; set; }
    public string Tooltip { get; set; }
    public void Setup(List<Command> commands, Sprite sprite, string tooltip)
    {
        Commands = commands;
        Sprite = sprite;
        Tooltip = tooltip;
    }
}
