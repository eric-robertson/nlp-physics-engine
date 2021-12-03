using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractScene : MonoBehaviour
{

    public GameObject ball;
    public GameObject cube;
    public GameObject slope;
    public GameObject box;
    public GameObject invPyramid;
    public GameObject platform;
    public GameObject staircase;
    public GameObject cliff;

    public PhysicsMaterial2D bouncy;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D icy;

    public Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();
    public Dictionary<string, string> itemTypes = new Dictionary<string, string>();
    private List<string> createdItems = new List<string>();
    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();

    private List<string> description = new List<string>();

    private Logger logs = new Logger();
    private bool started = false;

    public void create ( string name, string item ) {
        logs.startTime = Time.fixedTime;

        if ( item.Equals("nothing")) return;

        if ( item.Equals("ball"))
            items.Add( name, Instantiate(ball));
        
        if ( item.Equals("cube"))
            items.Add( name, Instantiate(cube));

        if ( item.Equals("slope"))
            items.Add( name, Instantiate(slope));

        if ( item.Equals("platform"))
            items.Add( name, Instantiate(platform));

        if ( item.Equals("invPyramid"))
            items.Add( name, Instantiate(invPyramid));

        if ( item.Equals("box"))
            items.Add( name, Instantiate(box));

        if ( item.Equals("staircase"))
            items.Add( name, Instantiate(staircase));

        if ( item.Equals("cliff"))
            items.Add( name, Instantiate(cliff));

        items[name].name = name;
        items[name].transform.rotation = Quaternion.Euler(new Vector3(0,Random.Range(0,2) * 180,0));


        createdItems.Add(item);
        createdItems.Add(name);
        itemTypes.Add( name, item );
        logs.defineItem( name, item );

        if ( itemCounts.ContainsKey( item ))
            itemCounts[item] += 1;
        else
            itemCounts.Add(item, 1);
    }

    private void place ( string object1, string object2, string dir, Vector3 delta ){
        GameObject o1 = items[object1];
        GameObject o2 = items[object2];

        float random = Random.Range(0f, 6.28319f);
        o1.transform.position = o2.transform.position + delta + 0.8f * new Vector3(Mathf.Cos(random), Mathf.Sin(random), 0);
        o1.transform.rotation = Quaternion.Euler(new Vector3(0,0,Random.Range(-40, 40)));

        string o1_name = itemCounts[itemTypes[object1]] > 1 ? object1 : "the " + itemTypes[object1];
        string o2_name = itemCounts[itemTypes[object2]] > 1 ? object2 : "the " + itemTypes[object2];

        //description.Add(string.Format("{0} is placed {2} {1}.", o1_name, o2_name, dir));
        logs.definePosition(object1, dir, object2);

    }

    public void placeAbove ( string object1, string object2 ){
        place( object1, object2, "above", new Vector3(0,2,0));
    }
    public void placeIn ( string object1, string object2 ){
        place( object1, object2, "in", new Vector3(0,0,0));
    }
    public void placeBelow ( string object1, string object2 ){
        place( object1, object2, "below", new Vector3(0,-2,0));
    }
    public void placeLeft ( string object1, string object2 ){
        place( object1, object2, "left", new Vector3(-2,0,0));
    }
    public void placeRight ( string object1, string object2 ){
        place( object1, object2, "right", new Vector3(2,0,0));
    }


    public void setAttribute ( string object1, string attribute, string value ) {

        if ( ! items.ContainsKey(object1)) return;
        GameObject o1 = items[object1];
        Rigidbody2D body = o1.GetComponent<Rigidbody2D>();
        BoxCollider2D coll1 = GetComponent<BoxCollider2D>();
        CircleCollider2D coll2 = GetComponent<CircleCollider2D>();

        /*if ( value.Equals("") )
            description.Add(string.Format("{0} set to be {1}", object1, attribute));
        else
            description.Add(string.Format("{0} set have its {1} be {2}", object1, attribute, value));*/
        
        if ( !started )
            logs.defineInitialAttributes( object1, attribute, value );
        else
            logs.defineDelayedAttributes( object1, attribute, value );

        if ( attribute.Equals("drag")){
            if ( value.Equals("high"))
                body.drag = 20;
            if ( value.Equals("none"))
                body.drag = 0;
        }
        if ( attribute.Equals("bouncy")) {
            if ( coll1 != null )
                coll1.sharedMaterial = bouncy;
            if ( coll2 != null )
                coll2.sharedMaterial = bouncy;
            body.sharedMaterial = bouncy;
        }
        if ( attribute.Equals("icy")) {
            if ( coll1 != null )
                coll1.sharedMaterial = icy;
            if ( coll2 != null )
                coll2.sharedMaterial = icy;
            body.sharedMaterial = icy;
        }
        if ( attribute.Equals("friction")) {
            if ( coll1 != null )
                coll1.sharedMaterial = friction;
            if ( coll2 != null )
                coll2.sharedMaterial = friction;
            body.sharedMaterial = friction;
        }   
        if ( attribute.Equals("size")){
            if ( value.Equals("big"))
                o1.transform.localScale *= Random.Range(1.5f, 4f);
            if ( value.Equals("small"))
                o1.transform.localScale *= Random.Range(0.1f, 0.7f);
        }
        if ( attribute.Equals("mass")) {
            if ( value.Equals("high"))
                body.mass = Random.Range(4f, 30f);
            if ( value.Equals("low"))
                body.mass = Random.Range(0.05f, 0.6f);
        }  
        if ( attribute.Equals("gravity")) {
            if ( value.Equals("high"))
                body.gravityScale = Random.Range(2f, 6f);
            if ( value.Equals("low"))
                body.gravityScale = Random.Range(0.3f, 0.7f);
            if ( value.Equals("none"))
                body.gravityScale = 0;
            if ( value.Equals("reversed"))
                body.gravityScale = -1;
        }   

    }

    private void setVelocity ( string object1, string label, Vector3 direction, string speed) {
        if ( ! items.ContainsKey(object1)) return;
        GameObject o1 = items[object1];
        Rigidbody2D body = o1.GetComponent<Rigidbody2D>();

        float speed_f = 0;
        if ( speed.Equals("fast") )
            speed_f = Random.Range(4f, 8f);
        if ( speed.Equals("slow") )
            speed_f = Random.Range(1f, 2f);        

        body.velocity = direction * speed_f;
        
        if ( !started )
            logs.defineInitialAttributes( object1, "pushed " + label, speed );
        else
            logs.defineDelayedAttributes( object1, "pushed " + label, speed );

    }
    public void pushLeft ( string object1, string speed ){
        setVelocity( object1, "left", new Vector3(-1,0,0), speed );
    }
    public void pushRight ( string object1, string speed ){
        setVelocity( object1, "right" , new Vector3(1,0,0), speed );
    }
    public void pushUp ( string object1, string speed ){
        setVelocity( object1, "up" , new Vector3(0,1,0), speed );
    }
    public void pushDown ( string object1, string speed ){
        setVelocity( object1, "down" , new Vector3(0,1,0), speed );
    }

    public void disapear ( string object1 ){

        if ( ! items.ContainsKey(object1)) return;
        
        if ( !started )
            logs.defineInitialAttributes( object1, "removed", "" );
        else
            logs.defineDelayedAttributes( object1, "removed", "" );

        Destroy(items[object1]);
        items.Remove( object1 );
    }

    public void finalizeExists () {
        /*
        string scene = "In a 2D world, there exists";
        for ( int i = 0 ; i < createdItems.Count; i += 2 ){
            if ( i == createdItems.Count - 2 )
                scene += " and";
            if ( itemCounts[createdItems[i]] > 1 )
                scene += string.Format(" a {0} ({1})", createdItems[i], createdItems[i+1]);
            else
                scene += string.Format(" a {0}", createdItems[i]);
            if ( i != createdItems.Count - 2 )
                scene += ',';
        }
        Debug.Log(scene + '.');*/
    }

    public string nameObject ( string label ) {
        if ( itemCounts[itemTypes[label]] > 1)
            return label;
        else
            return "the " + itemTypes[label];
    }

    public void finalize () {
        foreach ( string s in description )
            Debug.Log(s);

        ObjectRecorder[] recorders = FindObjectsOfType<ObjectRecorder>();
        foreach( ObjectRecorder o in recorders ){
            o.recordEventTo = this;
            o.objectLabel = lookupLabel( o.gameObject );
            o.logs = logs;
        }

        started = true;

    }

    public string lookupLabel ( GameObject g) {
        foreach (string label in items.Keys ){
            if ( g.Equals(items[label]))
                return label;
        }
        if ( g.transform.parent )
            return lookupLabel(g.transform.parent.gameObject );
        return g.name;
    }

    public void clearScene ( int group ) {

        logs.dump(group);      

        foreach ( string s in items.Keys ){
            Destroy( items[s]);
        }
  
        items = new Dictionary<string, GameObject>();
        itemTypes = new Dictionary<string, string>();
        createdItems = new List<string>();
        itemCounts = new Dictionary<string, int>();
        description = new List<string>();
        logs = new Logger();
        started = false;

    }


}
