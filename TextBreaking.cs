using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 该类解决中文符号在句首或句尾的问题，符号在句尾会自动换行
/// </summary>
public class TextBreaking : MonoBehaviour
{
    public Text textToReformat;
    [Multiline]
    public string debugString;

    [Header("Debug")] 
    private readonly List<string> avoidLineEndingList = new List<string>() {"“"};//避免在行尾的符号
    private readonly List<string> avoidLineHeadList = new List<string>() {"，", "！", "。"};//避免在行首的符号
    private readonly List<string> lineEndingList = new List<string>() {"\r\n", "\n", "\r"};//换行符列表
    [Multiline]
    private List<string> lineTextList = new List<string>();
    private string lineText;
    private int lineStartIndex;

    void Start()
    {
        Format();
    }

    [ContextMenu("Format")]
    void Format()
    {
        if (!string.IsNullOrEmpty(debugString))
        {
            textToReformat.text = debugString;
        }
        
        var generator = new TextGenerator();
        
        var settings = CopyFrom(textToReformat.GetGenerationSettings(textToReformat.rectTransform.rect.size));

        float boundWidth = textToReformat.rectTransform.sizeDelta.x;
        
        lineTextList.Clear();
        
        var value = textToReformat.text;
        //将\r\n替换为效果一样的\n，方便计算
        value = value.Replace("\r\n", "\n");
        
        var updatedText = "";
        int length = 0;
        lineText = "";
        lineStartIndex = 0;
        int lineLength = 0;
        while (length < value.Length)
        {
            length++;
            updatedText = value.Substring(0, length);
            //Debug.Log(updatedText + "==" + length); 
            
            lineLength++;
            if (lineStartIndex + lineLength > value.Length)
            {
                break;
            }
            lineText = value.Substring(lineStartIndex, lineLength); 

            float w = generator.GetPreferredWidth(lineText, settings) / settings.scaleFactor;
            
            var endValue = lineText.Substring(lineText.Length - 1, 1);
            //检测\r\n 这个上面将\r\n过滤掉了，可能不需要这样判断了
            var endValue2 = lineText.Length > 2 ? lineText.Substring(lineText.Length - 2, 2) : "";
            bool isLineEnding = lineEndingList.Contains(endValue2);
            if (!isLineEnding)
            {
                isLineEnding = lineEndingList.Contains(endValue);
            }

            if (isLineEnding)
            {
                length += 1;//判断是回车，长度需要增加1
                lineLength = 1;
                //lineText = value.Substring(lineStartIndex, 1);
                lineStartIndex = updatedText.Length;
                lineTextList.Add(lineText);
                continue;
            }
            if (w >= boundWidth)
            {
//                var endValue = lineText.Substring(lineText.Length - 1, 1);
                var endValue3=lineText.Length > 2 ? lineText.Substring(lineText.Length - 2, 1) : "";

                //判断句尾
                if (avoidLineEndingList.Contains(endValue3))
                {
                    int insertIndex = updatedText.Length - 2;
                    value = value.Insert(insertIndex, "\n");
                    lineText = value.Substring(lineStartIndex, insertIndex-lineStartIndex);
                    length -= 1;//插入后长度索引减掉
                    lineLength = 0;
                    lineStartIndex = insertIndex + 1;
                }
                //判断句首
                else if (avoidLineHeadList.Contains(endValue))
                {
                    int insertIndex = updatedText.Length - 2;
                    value = value.Insert(insertIndex, "\n");
                    lineText = value.Substring(lineStartIndex, insertIndex-lineStartIndex);
                    length -= 1;//插入后长度索引减掉
                    lineLength = 0;
                    lineStartIndex = insertIndex + 1;
                }
                else
                {
                    lineLength = 1;
                    lineText = value.Substring(lineStartIndex, lineText.Length - 1);
                    lineStartIndex = updatedText.Length-1;
                }
                lineTextList.Add(lineText);

                //Debug.Log("[AA]"+lineText + "==" + w + "===" + length);
            }
        }
 
        textToReformat.text = updatedText;
    }
    
    TextGenerationSettings CopyFrom(TextGenerationSettings o) {
        return new TextGenerationSettings {
            font = o.font,
            color = o.color,
            fontSize = o.fontSize,
            lineSpacing = o.lineSpacing,
            richText = o.richText,
            scaleFactor = o.scaleFactor,
            fontStyle = o.fontStyle,
            textAnchor = o.textAnchor,
            alignByGeometry = o.alignByGeometry,
            resizeTextForBestFit = o.resizeTextForBestFit,
            resizeTextMinSize = o.resizeTextMinSize,
            resizeTextMaxSize = o.resizeTextMaxSize,
            updateBounds = o.updateBounds,
            verticalOverflow = o.verticalOverflow,
            horizontalOverflow = o.horizontalOverflow,
            generationExtents = o.generationExtents,
            pivot = o.pivot,
            generateOutOfBounds = o.generateOutOfBounds
        };
    }
}
