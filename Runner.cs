using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    
    int simulations = 99999;
    List<string> objectTypes = new List<string>{"ball","cube"};
    List<string> baseTypes = new List<string>{"box", "platform","slope", "invPyramid", "staircase", "cliff" };

    List<float> randomSeeds = new List<float>();
    int randomId = 0;

    void Start()
    {
        RunItteration(0);
    }

    int randRange( int a, int b){
        return (int)System.Math.Floor(randomSeeds[++randomId] * (b-a)) + a;
    }

    void ApplyAttribute ( List<string> labels, AbstractScene scene, int objectNumber ) {
        int a = randRange(0, 18);
        string target = labels[randRange(0, objectNumber)];
        if ( a == 0) scene.setAttribute(target, "drag", "high" );
        if ( a == 1) scene.setAttribute(target, "drag", "none" );
        if ( a == 2) scene.setAttribute(target, "bouncy", "" );
        if ( a == 3) scene.setAttribute(target, "icy", "" );
        if ( a == 4) scene.setAttribute(target, "friction", "" );
        if ( a == 5) scene.setAttribute(target, "size", "small" );
        if ( a == 6) scene.setAttribute(target, "size", "big" );
        if ( a == 7) scene.setAttribute(target, "mass", "low" );
        if ( a == 8) scene.setAttribute(target, "mass", "high" );
        if ( a == 9) scene.setAttribute(target, "gravity", "low" );
        if ( a == 10) scene.setAttribute(target, "gravity", "high" );
        if ( a == 11) scene.setAttribute(target, "gravity", "none" );
        if ( a == 12) scene.setAttribute(target, "gravity", "reversed" );
        if ( a == 13) scene.pushLeft(target, "fast" );
        if ( a == 14) scene.pushRight(target, "fast" );
        if ( a == 15) scene.pushUp(target, "fast" );
        if ( a == 16) scene.pushDown(target, "fast" );
        if ( a == 17) scene.disapear(target );
    }

    void RunItteration(int trial)
    {

        Time.timeScale = 100;

        simulations -= 1;
        if ( simulations == 0) return;

        if ( trial == 0 ) {
            randomSeeds = new List<float>();
            for ( var i = 0 ; i < 100; i ++ )
                randomSeeds.Add(Random.Range(0f,1f));
            trial = 10;
        }

        AbstractScene scene = GetComponent<AbstractScene>();
        randomId = 0;

        // Choose objects in scene
        int objectNumber = randRange(1, 6);
        List<string> labels = new List<string>();

        for ( var i = 0 ; i < objectNumber; i ++ ){
            string objectType = objectTypes[randRange(0,objectTypes.Count)];
            string new_label = string.Format("{0}{1}", objectType, i);
            labels.Add(new_label);
            scene.create(new_label, objectType);
        }

        // Choose base
        int baseType = randRange(0,baseTypes.Count);
        Debug.Log(baseType + " " + baseTypes.Count + " " + labels.Count);
        scene.create(baseTypes[baseType], baseTypes[baseType]);

        // Place first item
        if ( baseType == 0 )
            scene.placeIn(labels[0], baseTypes[baseType]);
        else
            scene.placeAbove(labels[0], baseTypes[baseType]);

        // Place rest
        for ( var i = 1; i < objectNumber; i ++ ){
            int position =  randRange(0,4);
            int previous = randRange(0,(System.Math.Min(i,objectNumber)));
            if ( position == 0 )
                scene.placeAbove(labels[i], labels[previous]);
            if ( position == 1 )
                scene.placeBelow(labels[i], labels[previous]);
            if ( position == 2 )
                scene.placeLeft(labels[i], labels[previous]);
            if ( position == 3 )
                scene.placeRight(labels[i], labels[previous]);
        }

        // Attributes
        int attributeCount = randRange(0, 7);
        for ( var i = 1 ; i < attributeCount; i ++ ) {
            ApplyAttribute( labels, scene, objectNumber );
        }

        // Startup

        scene.finalize();
        StartCoroutine(DelayedActions());
        StartCoroutine(RunSim());

        // Delayed Actions

        IEnumerator DelayedActions() 
        {
            yield return new WaitForSeconds(5);

            // Attributes
            int attributeCount = randRange(0, 6);
            for ( var i = 1 ; i < attributeCount; i ++ ) {
                ApplyAttribute( labels, scene, objectNumber );
            }

        }

        IEnumerator RunSim() 
        {
            yield return new WaitForSeconds(20);

            Debug.Log("end");
            scene.clearScene(randRange(0, 10000000));
            RunItteration(trial-1);
        }

    }

}
