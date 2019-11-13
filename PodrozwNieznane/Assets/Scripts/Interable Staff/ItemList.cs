using System.Collections;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ItemList : MonoBehaviour
{
    private string path = "Items";

    [SerializeField] EquippableItem[] listOfItems;

    private string finallPath;
    public static EquippableItem[] ListOfItems;

    private void Start()
    {
        ListOfItems = listOfItems;
    }

    // Tworzenie listy przedmiotów wybranej ścieżce
    //    public ItemList()
    //    {
    //#if UNITY_EDITOR
    //        ArrayList arrayList = new ArrayList();
    //        string[] fileEntries = Directory.GetFiles(Application.dataPath + "\\" + path);
    //        foreach (string fileName in fileEntries)
    //        {
    //            int index = fileName.LastIndexOf("\\");
    //            string localPath = "Assets\\" + path;

    //            if (index > 0)
    //                localPath += fileName.Substring(index);
    //            Object item = AssetDatabase.LoadAssetAtPath(localPath, typeof(EquippableItem));
    //            if (item != null)
    //                arrayList.Add(item);
    //        }

    //        ListOfItems = new EquippableItem[arrayList.Count];
    //        for (int i = 0; i < arrayList.Count; i++)
    //        {
    //            ListOfItems[i] = (EquippableItem)arrayList[i];
    //        }
    //#endif
    //    }

    //    public ItemList(string anotherPath)
    //    {
    //#if UNITY_EDITOR
    //        ArrayList arrayList = new ArrayList();
    //        string[] fileEntries = Directory.GetFiles(Application.dataPath + "\\" + anotherPath);
    //        foreach (string fileName in fileEntries)
    //        {
    //            int index = fileName.LastIndexOf("\\");
    //            string localPath = "Assets\\" + anotherPath;

    //            if (index > 0)
    //                localPath += fileName.Substring(index);
    //            Object item = AssetDatabase.LoadAssetAtPath(localPath, typeof(EquippableItem));
    //            if (item != null)
    //                arrayList.Add(item);
    //        }

    //        ListOfItems = new EquippableItem[arrayList.Count];
    //        for (int i = 0; i < arrayList.Count; i++)
    //        {
    //            ListOfItems[i] = (EquippableItem)arrayList[i];
    //        }
    //#endif
    //    }

    public EquippableItem[] getItemList()
    {
        return ListOfItems;
    }
    
    public void setPath(string newPath)
    {
        path = newPath;
    }

}