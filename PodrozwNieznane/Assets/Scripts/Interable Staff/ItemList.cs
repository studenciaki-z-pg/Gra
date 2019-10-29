using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ItemList
{
    private string path = "Items";

    private string finallPath;
    public static EquippableItem[] ListOfItems;

    // Tworzenie listy przedmiotów wybranej ścieżce
    public ItemList()
    {
        ArrayList arrayList = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "\\" + path);
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("\\");
            string localPath = "Assets\\" + path;

            if (index > 0)
                localPath += fileName.Substring(index);
            Object item = AssetDatabase.LoadAssetAtPath(localPath, typeof(EquippableItem));
            if (item != null)
                arrayList.Add(item);
        }

        ListOfItems = new EquippableItem[arrayList.Count];
        for (int i = 0; i < arrayList.Count; i++)
        {
            ListOfItems[i] = (EquippableItem)arrayList[i];
            //Debug.Log(ListOfItems[i].name); //TO DELETE
        }
    }

    public ItemList(string anotherPath)
    {
        ArrayList arrayList = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "\\" + anotherPath);
        foreach (string fileName in fileEntries)
        {
            int index = fileName.LastIndexOf("\\");
            string localPath = "Assets\\" + anotherPath;

            if (index > 0)
                localPath += fileName.Substring(index);
            Object item = AssetDatabase.LoadAssetAtPath(localPath, typeof(EquippableItem));
            if (item != null)
                arrayList.Add(item);
        }

        ListOfItems = new EquippableItem[arrayList.Count];
        for (int i = 0; i < arrayList.Count; i++)
        {
            ListOfItems[i] = (EquippableItem)arrayList[i];
            //Debug.Log(ListOfItems[i].name); //TO DELETE
        }
    }

        public EquippableItem[] getItemList()
    {
        return ListOfItems;
    }
    
    public void setPath(string newPath)
    {
        path = newPath;
    }

}