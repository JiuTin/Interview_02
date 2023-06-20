using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Node", menuName ="Event/Message/ShowDialogue")]
public class EN_ShowDialogue: EventNodeBase
{
    public DialogueData[] datas;            //可以有多个内容，会逐渐显示出来
    public int boxStyle = 0;
    private int _index;
    public override void Execute()
    {
        base.Execute();
        _index = 0;
        UIManager.OpenDialogueBox(ShowNextDialogue, boxStyle);
    }
    //显示剩下的内容，显示完后，状态改为Finished
    private void ShowNextDialogue(bool forceDiplayDirectly)
    {
        if (_index < datas.Length)
        {
            // 新实例化一个data，判断是否可以强制执行
            DialogueData data = new DialogueData()
            {
                Speaker = datas[_index].Speaker,
                Content = datas[_index].Content,
                CanQuickShow = datas[_index].CanQuickShow,
                AutoNext = datas[_index].AutoNext,
                NeedTyping = !forceDiplayDirectly && datas[_index].NeedTyping       //是否逐字打印
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
