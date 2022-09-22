using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドロップを受け付けるインターフェース
/// </summary>
public interface IDrop
{
    /// <summary>
    /// ドロップ受付可否<br/>効果を書き換えたい場合はここで書き換える
    /// </summary>
    /// <param name="commands"></param>
    bool GetDrop(ref List<Command> commands);
    /// <summary>
    /// 受付対象のUseTypeを返す
    /// </summary>
    /// <returns></returns>
    UseType GetUseType { get; }
    /// <summary>
    /// ドロップ対象を強調表示させる
    /// </summary>
    void ShowDropEria(UseType useType);
}
