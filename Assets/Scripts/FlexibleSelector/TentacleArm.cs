/*The MIT License(MIT)

Copyright(c) Patrice Robitaille

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using UnityEngine;

public class TentacleArm : MonoBehaviour {

    public MovableObject selectedObject = null;                 //This is the selected object where the tentacle will be attached
    public GameObject tentacleStartNode;                        //This is the node where the line renderer will start
    public GameObject tentacleEndNode;                          //This is the node where the line renderer will end (

    private LineRenderer tentacleArm;                           //This is the line renderer component
    private int armSegmentCount = 10;                           //This number of segment of the line renderer. More = smoother but less performance
    private bool _isLocked;                                     //Define if an the tentacle is locked to an object

    private float movableObjectSpeed = 0.15F;                   //The speed of the movement of the selected object while rotating your HMD
    private float scrollFactor = 0.1F;                          //The speed to resize the tentacle while using the scroll wheel

    // Use this for initialization
    void Start()
    {
        tentacleArm = this.gameObject.GetComponent<LineRenderer>();
        this.tentacleArm.SetVertexCount(armSegmentCount);
        this.IsLocked = false;
    }
    

    public bool IsLocked
    {
        get
        {
            return this._isLocked;
        }
        private set
        {
            this._isLocked = value;
            if (this._isLocked)
                this.tentacleArm.enabled = true;
            else
                this.tentacleArm.enabled = false;
        }
    }

    public void SelectObject(MovableObject selectedObject)
    {
        this.selectedObject = selectedObject;

        float distanceZ = Vector3.Distance(tentacleStartNode.transform.position, selectedObject.transform.position);
        Vector3 tentacleEndNodePosition = new Vector3(0, 0, distanceZ);
        this.tentacleEndNode.transform.localPosition = tentacleEndNodePosition;

        this.IsLocked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsLocked)
        {
            moveSelectedObject();

            moveLineRenderer();

            resizeTentacle();

            //If right click
            if (Input.GetMouseButtonDown(1))
            {
                selectedObject.Release();
                selectedObject = null;

                this.IsLocked = false;
            }
        }  
    }

    void moveSelectedObject()
    {
        //Move the object
        selectedObject.transform.position = Vector3.MoveTowards(selectedObject.transform.position, tentacleEndNode.transform.position, movableObjectSpeed);
        selectedObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

    void moveLineRenderer()
    {
        Vector3 to = selectedObject.transform.position;

        //Render the line renderer
        for (int index = 0; index < this.armSegmentCount; ++index)
        {
            float t1 = (float)index / (float)this.armSegmentCount;
            float t2 = (float)(0.5 * (double)t1 + 0.5);
            this.tentacleArm.SetPosition(index, Vector3.Lerp(this.tentacleStartNode.transform.position, Vector3.Lerp(tentacleEndNode.transform.position, to, t2), t1));
        }
    }

    void resizeTentacle()
    {
        //Change the length of LineRenderer with Scoll
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            this.tentacleEndNode.transform.localPosition = new Vector3(0, 0, tentacleEndNode.transform.localPosition.z + scrollFactor);
        }
        else if (d < 0f)
        {
            this.tentacleEndNode.transform.localPosition = new Vector3(0, 0, tentacleEndNode.transform.localPosition.z - scrollFactor);
        }
    }
}
