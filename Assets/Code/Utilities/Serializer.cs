using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class Serializer
{
    public static T Load<T>(string filename) where T : class
    {

        var dataLoaded = default(T);
            try
            {
                if (File.Exists(Application.persistentDataPath + "/" + filename))
                {
                    
                    Stream stream = File.Open(Application.persistentDataPath + "/" + filename, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new VersionDeserializationBinder();
                    dataLoaded = formatter.Deserialize(stream) as T;
                    stream.Close();

                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        
        return dataLoaded;
    }

    public static void Save<T>(string filename, T data) where T : class
    {
        var fullFileName = Application.persistentDataPath + "/" + filename;
        Stream stream = File.Open(Application.persistentDataPath + "/" + filename, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Binder = new VersionDeserializationBinder();
        formatter.Serialize(stream, data);
        stream.Close();

    }
}