using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

public class population_manager : MonoBehaviour
{
    [SerializeField] private GameObject organism_prefab;
    [SerializeField] private Transform start_location;
    private bool running = false;
    private List <dna> organisms;
    private float running_time;
    [SerializeField] private int num_org = 15;
    private float current_day;
    private int num_dead;
    private float start_time;
    private GameObject cam;
    private bool move_cam = true;
    void Start()
    {
        cam = Camera.main.gameObject;
        start_time = 0;
        num_dead = 0;
        current_day = 0;
        organisms = new List<dna>();
        for(int i = 0; i < num_org; i++)
        {
            organisms.Add(Instantiate(organism_prefab,start_location.position,Quaternion.identity,transform).GetComponent<dna>());
            organisms[i].initialize_DNA();
        }
        this.running = true;
        running_time = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        running_time = Time.time - start_time;
        if(running_time >= 200||num_dead >= num_org)
        {
            reset_day(); 
        }
        if(move_cam&& running_time >=0.5f)
        {
            Vector3 camtPos = new Vector3(getTop().transform.position.x,cam.transform.position.y,getTop().transform.position.z);
            cam.transform.position = Vector3.Lerp(cam.transform.position,camtPos,2*Time.deltaTime);
        }
    }
    public bool isRunning()
    {
        return running;
    }
    public void setRunning(bool b)
    {
        running = b;
    }

    public void reset_day()
    {
        move_cam = false;
        List<float> fit = new List<float>();
        float sum = 0;
        for(int i = 0; i < num_org;i++)
        {
            fit.Add(organisms[i].getFitness());
            sum += fit[i];
            
        }
        fit.Sort();
        organisms_sort();
        for(int i = 0; i < fit.Count; i++)
        {
            if (i == 0)
            {
                fit[i] = fit[i]/sum;
            }
            else
            {
                fit[i] = ( fit[i]/sum )+ fit[i-1];
            }
        }
        
        List<dna> tempO = new List<dna>();
        for(int i = 0 ; i < num_org; i++)
        {
            dna parent_a = new dna();
            dna parent_b = new dna();

            //int a_rand = Random.Range(0,num_org);
            //int b_rand = Random.Range(0,num_org);
            
            while(parent_a == null || parent_b == null)
            {
                float a_rand = Random.Range(0.0f,1.0f);
                float b_rand = Random.Range(0.0f,1.0f);

                
                for(int j = 0 ; j < fit.Count-1; j++)
                {               
                    if(a_rand < fit[j+1] && a_rand >= fit[j])
                    {
                        parent_a = organisms[j+1];
                        //Debug.Log(a_rand + " Organism Selected: "+ (j+1));
                    }

                    if(b_rand < fit[j+1] && b_rand >= fit[j])
                    {
                        parent_b = organisms[j+1];
                    }
                }
            }
            
              
            /* 
            while(a_randT >  fit[a_rand])
            {
                if(a_randT <  fit[a_rand] )
                {
                    parent_a = organisms[a_rand];
                }
                a_rand = Random.Range(0,num_org);
                a_randT = Random.Range(0.0f,1.0f);
            }
            while(b_randT >  fit[b_rand])
            {
                if(b_randT <  fit[b_rand] )
                {
                    parent_b = organisms[b_rand];
                }
                b_rand = Random.Range(0,num_org);
                b_randT = Random.Range(0.0f,1.0f);
            }
            */
            
            //parent_a = organisms[organisms.Count-1];
            //parent_b = organisms[organisms.Count-2];
          
            tempO.Add(parent_a.crossover_DNA(parent_b));         
            if(i == 0||i==1)
            {
                organisms[i].initialize_DNA();         
            }
            else
            {       
                organisms[i].setDNA(tempO[i]);
            }
            organisms[i].mutate_DNA();
            organisms[i].transform.position = start_location.position;
            organisms[i].transform.rotation = Quaternion.identity;
            organisms[i].setAlive(true);
            organisms[i].setFit(0);
        

        }
        cam.transform.position = new Vector3 (start_location.position.x,cam.transform.position.y,start_location.position.z);
        running_time = 0;
        current_day +=1;
        num_dead = 0;
        start_time = Time.time;
        move_cam = true;

    }
    void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle();
		Rect rect = new Rect(w/2, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 90;
		style.normal.textColor = new Color (0f, 0.0f, 0f, 1.0f);
		//string text = string.Format("{0:0.0} Running_Time ({0:0.0} Current_Day)", running_time, current_day);
        string text = string.Format("{0:0.0} Running_Time     ({1:0.} Current_Day)", running_time, current_day);
		GUI.Label(rect, text, style);

		
	}
    public dna getTop ()
    {   
        organisms_sort();
        return organisms[organisms.Count-1];
    }
    public void organisms_sort()
    {   
        organisms.Sort(
        delegate(dna p1, dna p2)
        {
            return p1.getFitness().CompareTo(p2.getFitness());
        }
        );
    }
    public void inc_dead()
    {
        num_dead ++;
    }
}
