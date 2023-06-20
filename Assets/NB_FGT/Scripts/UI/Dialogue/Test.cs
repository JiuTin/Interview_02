using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string Speaker;
    [Multiline]
    public string Content;
    public bool AutoNext;
    public bool NeedTyping;
    public bool CanQuickShow;
}
public class Test : MonoBehaviour
{
    public SequenceEventExecutor _sequenceExecutor;
    public GameObject dialogueBox;
    private void Start()
    {

        _sequenceExecutor.Init(OnFinishedEvent);

    }
    void Update()
    {
        //检测是否有NPC可以交互,按F键
        if (GameInputManager.MainInstance.Grab && !dialogueBox.activeSelf)
        {
            _sequenceExecutor.Execute();
        }
    }
    void OnFinishedEvent(bool success)
    {
       
    }
}
