using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ItemList : MonoBehaviour
{
    [SerializeField] string path;

    private string finallPath;
    public ArrayList items = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        string[] finallPath = Directory.GetFiles(Application.dataPath + path);
        
        foreach (string itemName in finallPath)
        {
            string tmp = itemName.Replace('\\', '/');
            //Debug.Log(tmp);
            int index = tmp.LastIndexOf("/");
            string localPath = "Assets" + path;

            if (index > 0)
                localPath += tmp.Substring(index);
            Object item = AssetDatabase.LoadAssetAtPath(localPath, typeof(EquippableItem));
            if (item != null)
            {
                items.Add(item);
                Debug.Log(item.name.ToString());
            }
            //Debug.Log("?");
        }
    }
    
}