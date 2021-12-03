using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Logger  {

    [System.Serializable]
    public class ObjectRecord {
        public string label;
        public string type;
    }

    [System.Serializable]    
    public class ObjectPosition {
        public string targetObject;
        public string relationTo;
        public string relatedObject;
    }

    [System.Serializable]    
    public class ObjectAttribute {
        public string targetObject;
        public string attribute;
        public string value;
    }    
    
    [System.Serializable]
    public class ResultStates {
        public float simulationTime;
        public string sourceObject;
        public string eventDetails;
        public string relatedObject;
        public bool newValue;
    }

    public List<ObjectRecord> items = new List<ObjectRecord>();
    public List<ObjectPosition> positions = new List<ObjectPosition>();
    public List<ObjectAttribute> attributes = new List<ObjectAttribute>();
    public List<ObjectAttribute> delayedEffects = new List<ObjectAttribute>();
    public List<ResultStates> results = new List<ResultStates>();

    public float startTime = 0;

    public Logger () {
    }

    public void defineItem ( string label, string type ) {
        ObjectRecord r = new ObjectRecord();
        r.label = label;
        r.type = type;
        items.Add(r);
    }

    public void definePosition ( string targetObject, string relationTo, string relatedObject ) {
        ObjectPosition p = new ObjectPosition();
        p.targetObject = targetObject;
        p.relationTo = relationTo;
        p.relatedObject = relatedObject;
        positions.Add(p);   
    }

    public void defineInitialAttributes ( string targetObject, string attribute, string value ) {
        ObjectAttribute o = new ObjectAttribute();
        o.targetObject = targetObject;
        o.attribute = attribute;
        o.value = value;
        attributes.Add(o);
    }
    public void defineDelayedAttributes ( string targetObject, string attribute, string value ) {
        ObjectAttribute o = new ObjectAttribute();
        o.targetObject = targetObject;
        o.attribute = attribute;
        o.value = value;
        delayedEffects.Add(o);
    }

    public void defineSimulationAttributes ( float time, string sourceObject, string eventDetails, bool newValue, string relatedObject ) {
        float simulationTime = time - startTime;
        ResultStates r = new ResultStates();
        r.simulationTime = simulationTime;
        r.sourceObject = sourceObject;
        r.eventDetails = eventDetails;
        r.newValue = newValue;
        r.relatedObject = relatedObject;
        results.Add(r);
    }

    public void dump (int group) {

        float id = Random.Range(0, 10000000);
        var text = JsonUtility.ToJson(this);
        StreamWriter writer = new StreamWriter(string.Format("./Saves/{1}_{0}.json", id, group), true);
        writer.WriteLine(text);
        writer.Close();

    }




}
