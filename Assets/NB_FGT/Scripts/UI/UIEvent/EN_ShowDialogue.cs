using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Node", menuName ="Event/Message/ShowDialogue")]
public class EN_ShowDialogue: EventNodeBase
{
    public DialogueData[] datas;            //�����ж�����ݣ�������ʾ����
    public int boxStyle = 0;
    private int _index;
    public override void Execute()
    {
        base.Execute();
        _index = 0;
        UIManager.OpenDialogueBox(ShowNextDialogue, boxStyle);
    }
    //��ʾʣ�µ����ݣ���ʾ���״̬��ΪFinished
    private void ShowNextDialogue(bool forceDiplayDirectly)
    {
        if (_index < datas.Length)
        {
            // ��ʵ����һ��data���ж��Ƿ����ǿ��ִ��
            DialogueData data = new DialogueData()
            {
                Speaker = datas[_index].Speaker,
                Content = datas[_index].Content,
                CanQuickShow = datas[_index].CanQuickShow,
                AutoNext = datas[_index].AutoNext,
                NeedTyping = !forceDiplayDirectly && datas[_index].NeedTyping       //�Ƿ����ִ�ӡ
            };
            UIManager.PrintDialogue(data);
            _index++;
        }
        else
        {
            state = NodeState.Finished;
            OnFinished(true);
        }
    }

}
