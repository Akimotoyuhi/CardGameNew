using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// コマンド実行関数に渡す際にコマンド群に情報を足すときに使う構造体
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
