using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �h���b�v���󂯕t����C���^�[�t�F�[�X
/// </summary>
public interface IDrop
{
    /// <summary>
    /// �h���b�v��t��<br/>���ʂ��������������ꍇ�͂����ŏ���������
    /// </summary>
    /// <param name="commands"></param>
    bool GetDrop(ref List<Command> commands);
    /// <summary>
    /// ��t�Ώۂ�UseType��Ԃ�
    /// </summary>
    /// <returns></returns>
    UseType GetUseType();
}
