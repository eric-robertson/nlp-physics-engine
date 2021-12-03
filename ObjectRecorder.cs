using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRecorder : MonoBehaviour
{

    public AbstractScene recordEventTo;
    public string objectLabel;
    public Logger logs;

    private double widthBuffer = 0.01;

    public Dictionary<string, bool> states = new Dictionary<string, bool>();

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        string label = recordEventTo.lookupLabel(collisionInfo.gameObject);
        //Debug.Log(string.Format("{0} hit {1}", recordEventTo.nameObject(objectLabel), recordEventTo.nameObject(label)));
        updateState( string.Format("collide|{0}", recordEventTo.itemTypes[label]), true);
        //logs.defineSimulationAttributes( Time.fixedTime, objectLabel, "collided", true, label);

    }
    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        string label = recordEventTo.lookupLabel(collisionInfo.gameObject);
        //Debug.Log(string.Format("{0} hit {1}", recordEventTo.nameObject(objectLabel), recordEventTo.nameObject(label)));   
        updateState( string.Format("collide|{0}", recordEventTo.itemTypes[label]), false);
        //logs.defineSimulationAttributes( Time.fixedTime, objectLabel, "collided", false, label);

    }

    void updateState ( string state, bool newState ){
        if ( states.ContainsKey(state) && states[state] == newState )
            return;

        logs.defineSimulationAttributes( Time.fixedTime, objectLabel, state.Split('|')[0], newState, state.Split('|')[1]);

        if ( !states.ContainsKey(state)){
            states.Add( state, newState );

            
        }
        else{
            states[state] = newState;

        }
    }

    void updateTempState ( string state, bool newState, Dictionary<string, bool> delta_states) {
        if ( !delta_states.ContainsKey(state))
            delta_states.Add( state, newState );
        else
            delta_states[state] = delta_states[state] && newState;
    }

    void Update()
    {
        Rigidbody2D body = GetComponent<Rigidbody2D>();

        if ( System.Math.Abs(body.angularVelocity) > 10 )
            updateState("rolling|", true);
        if ( System.Math.Abs(body.angularVelocity) < 1 )
            updateState("rolling|", false);

        if ( System.Math.Abs(body.velocity.magnitude) < 0.01 )
            updateState("moving|", false);
        if ( System.Math.Abs(body.velocity.magnitude) > 0.1 )
            updateState("moving|", true);

        Dictionary<string, bool> delta_states = new Dictionary<string, bool>();

        foreach ( string s in recordEventTo.items.Keys ) {
            if ( recordEventTo.items[s] == gameObject ) continue;

            Transform them = recordEventTo.items[s].transform;
            Transform me = gameObject.transform;
            string them_label = s;

            updateTempState(string.Format("left of|{0}", them_label), 
                them.position.x - them.localScale.x + widthBuffer> me.position.x + me.localScale.x, delta_states);
            updateTempState(string.Format("right of|{0}", them_label), 
                them.position.x + them.localScale.x - widthBuffer < me.position.x - me.localScale.x, delta_states);
            updateTempState(string.Format("above|{0}", them_label), 
                them.position.y + them.localScale.y - widthBuffer< me.position.y - me.localScale.y, delta_states);
            updateTempState(string.Format("below|{0}", them_label), 
                them.position.y - them.localScale.y + widthBuffer> me.position.y + me.localScale.y, delta_states);

        }

        foreach ( string s in delta_states.Keys ) {
            updateState( s , delta_states[s] );
        }

    }
}
