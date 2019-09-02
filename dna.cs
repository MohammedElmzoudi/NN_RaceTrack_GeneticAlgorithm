using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
public class dna : MonoBehaviour
{
    private int n_input = 5;
    private int n_hidden1 = 25;
    private int n_hidden2 = 25;
    private int n_output = 1;
    
    private  Matrix<float> weights_1;
    private Matrix<float> weights_2;
    private Matrix<float> weights_3;
    
    private Matrix<float> input_values;
    private Matrix<float> bias1;
    private Matrix<float> bias2;
    private Matrix<float> bias3;
    private population_manager popM;
    private List<Vector3> ray_dir;
    private float fitness = 0;
    private bool alive = true;
    private float distance = 0;

    void Start()
    {
        
        fitness = 0;
        popM = GameObject.FindGameObjectWithTag("Population Manager").GetComponent<population_manager>();
        ray_dir = new List<Vector3>();
        ray_dir.Add(new Vector3(0,0,1));
        ray_dir.Add(new Vector3(1,0,1));
        ray_dir.Add(new Vector3(-1,0,1));
        ray_dir.Add(new Vector3(1,0,0));
        ray_dir.Add(new Vector3(-1,0,0));
        
        initialize_DNA();

    }
    
    void FixedUpdate() 
    {
        if(popM.isRunning() && alive)
        {
            float raysum = 0;
            for(int i = 0; i < ray_dir.Count;i++)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position,transform.TransformDirection(ray_dir[i]), out hit))
                {
                    if(hit.collider.tag == "Obstacle")
                    {
                        input_values[i,0] = hit.distance;
                        raysum += hit.distance;                   
                    }
                }
                //Test Raycasting directions
                Debug.DrawRay(transform.position,transform.TransformDirection(ray_dir[i])*30,Color.green);
            }
            fitness += Mathf.Pow(2,(raysum/n_input)/ 100);
            //fitness += (raysum/n_input)/100 ;
            //fitness += 1 /  Mathf.Pow(2,(  Vector3.Distance(transform.position,target_pos))/10);
            //fitness = Mathf.Clamp(fitness,0,9999999);
        }
    }

    public void initialize_DNA()
    {
        this.weights_1 = randomize_matrix(n_hidden1,n_input);
        this.weights_2 = randomize_matrix(n_hidden2,n_hidden1);
        this.weights_3 = randomize_matrix(n_output,n_hidden2);

        this.bias1 = randomize_matrix(n_hidden1,1);
        this.bias2 = randomize_matrix(n_hidden2,1);
        this.bias3 = randomize_matrix(n_output,1);

        this.input_values = Matrix<float>.Build.Dense(n_input,1);
    }
    public Matrix<float> randomize_matrix(int r_num, int c_num)
    {
        Matrix<float> t = Matrix<float>.Build.Dense(r_num,c_num);
        for(int i = 0; i< r_num;i++)
        {
            for(int j = 0; j < c_num;j++)
            {
                t[i,j] = Random.Range(-4f,4f);
            }
        }
        return t;
    }
    public void mutate_DNA()
    {
        if(Random.Range(0.0f,1.0f)<0.05f)
        {
            this.weights_1[Random.Range(0,n_hidden1-1),Random.Range(0,n_input-1)] += Random.Range(-1f,1f);
            this.weights_2[Random.Range(0,n_hidden2-1),Random.Range(0,n_hidden1-1)] += Random.Range(-1f,1f);
            this.weights_3[Random.Range(0,n_output-1),Random.Range(0,n_hidden2-1)] += Random.Range(-1f,1f);
            this.bias1[Random.Range(0,n_hidden1-1),0] += Random.Range(-1f,1f);
            this.bias2[Random.Range(0,n_hidden2-1),0] += Random.Range(-1f,1f);
            this.bias3[Random.Range(0,n_output-1),0] += Random.Range(-1f,1f);
        }
    }
    public dna crossover_DNA(dna parent_b)
    {
        Matrix<float> w1 = copy_matrix(parent_b.getWeights1());
        Matrix<float> w2 = copy_matrix(parent_b.getWeights2());
        Matrix<float> w3 = copy_matrix(parent_b.getWeights3());
        Matrix<float> b1 = copy_matrix(parent_b.getb1());
        Matrix<float> b2 = copy_matrix(parent_b.getb2());
        Matrix<float> b3 = copy_matrix(parent_b.getb3());

        Matrix <float> new_w1 = copy_matrix(weights_1);
        for(int i = 0; i < (this.weights_1.RowCount*this.weights_1.ColumnCount) / 2;i++)
        {

            int randI = Random.Range(0,this.weights_1.RowCount);
            int randJ = Random.Range(0,this.weights_1.ColumnCount);
            new_w1[randI,randJ] = w1[randI,randJ]; 

        }

        Matrix <float> new_w2 = copy_matrix(weights_2);
        for(int i = 0; i < (this.weights_2.RowCount*this.weights_2.ColumnCount) / 2;i++)
        {

            int randI = Random.Range(0,this.weights_2.RowCount);
            int randJ = Random.Range(0,this.weights_2.ColumnCount);
            new_w2[randI,randJ] = w2[randI,randJ]; 

        }

        Matrix <float> new_w3 = copy_matrix(weights_3);
        for(int i = 0; i < (this.weights_3.RowCount*this.weights_3.ColumnCount) / 2;i++)
        {

            int randI = Random.Range(0,this.weights_3.RowCount);
            int randJ = Random.Range(0,this.weights_3.ColumnCount);
            new_w3[randI,randJ] = w3[randI,randJ]; 

        }
        
        Matrix <float> new_b1 = copy_matrix(bias1);
        for(int i = 0; i < (this.bias1.RowCount*this.bias1.ColumnCount) / 2 ;i++)
        {

            int randI = Random.Range(0,this.bias1.RowCount);
            new_b1[randI,0] = b1[randI,0];      
            
        }
        
        Matrix <float> new_b2 = copy_matrix(bias2);
        for(int i = 0; i < (this.bias2.RowCount*this.bias2.ColumnCount) / 2 ;i++)
        {

            int randI = Random.Range(0,this.bias2.RowCount);
            new_b2[randI,0] = b2[randI,0];  

        }

        Matrix <float> new_b3 = copy_matrix(bias3);
        for(int i = 0; i < (this.bias3.RowCount*this.bias3.ColumnCount) / 2 ;i++)
        {

            int randI = Random.Range(0,this.bias3.RowCount);
            new_b3[randI,0] = b3[randI,0];  
              
        }

        dna new_dna = new dna();
        new_dna.setDNA(new_w1,new_w2,new_w3,new_b1,new_b2, new_b3);  
        return new_dna;
    }
    public void setDNA( Matrix <float> w1, Matrix <float> w2, Matrix <float> w3 , Matrix <float> b1, Matrix <float> b2, Matrix <float> b3 )
    {
        this.weights_1 = copy_matrix(w1);
        this.weights_2 = copy_matrix(w2);
        this.weights_3 = copy_matrix(w3);
        this.bias1 = copy_matrix(b1);
        this.bias2 = copy_matrix(b2);
        this.bias3 = copy_matrix(b3);
    }
    public void setDNA(dna nD)
    {
        this.weights_1 = copy_matrix(nD.getWeights1());
        this.weights_2 = copy_matrix(nD.getWeights2());
        this.weights_3 = copy_matrix(nD.getWeights3());
        this.bias1 = copy_matrix(nD.getb1());
        this.bias2 = copy_matrix(nD.getb2());
        this.bias3 = copy_matrix(nD.getb3());
    }
    public void setFit(float n)
    {
        this.fitness = n;
    }
    // Create a new referance to a matrix
    public Matrix<float> copy_matrix(Matrix<float> t)
    {
        Matrix<float> temp = Matrix<float>.Build.Dense(t.RowCount,t.ColumnCount);
        for(int i = 0; i < t.RowCount; i++)
        {
            for(int j = 0; j < t.ColumnCount; j++)
            {
                temp[i,j] = t[i,j];
            }
        }
        return temp;
    }
    public Matrix<float> tanh(Matrix<float> m)
    {
        Matrix<float> tm = m;
        for(int i = 0; i < m.RowCount; i++)
        {
            for(int j = 0; j < m.ColumnCount; j ++)
            {
                float x = m[i,j];
                //tm[i,j] = (Mathf.Exp(x) - Mathf.Exp(-x))/ (Mathf.Exp(-x)+Mathf.Exp(x));
                tm[i,j] = tm[i,j] / (1 + Mathf.Abs(tm[i,j]));
                //tm[i,j] = (float)Trig.Tanh(m[i,j]);
            }
        }
        return tm;
    }
    public Matrix<float> sigmoid(Matrix<float> m)
    {
        Matrix<float> tm = m;
        for(int i = 0; i < m.RowCount; i++)
        {
            for(int j = 0; j < m.ColumnCount; j ++)
            {
                float x = m[i,j];
                tm[i,j] =  Mathf.Max(0, tm[i,j]);
                //tm[i,j] = 1 / (1+Mathf.Exp(-m[i,j]));
            }
        }
        return tm;
    }
    public float predict()
    {
        Matrix<float> h1 = weights_1.Multiply(input_values) + bias1;
        h1 = sigmoid(h1);

        Matrix<float> h2 = weights_2.Multiply(h1) + bias2;
        h2 = sigmoid(h2);

        Matrix<float> output = weights_3.Multiply(h2) + bias3;
        output = tanh(output);
        
        return output[0,0];
    }



    public Matrix <float> getWeights1()
    {
        return copy_matrix(weights_1);
    }
    public Matrix <float> getWeights2()
    {
        return copy_matrix(weights_2);
    }
    public Matrix <float> getWeights3()
    {
        return copy_matrix(weights_3);
    }
    public Matrix <float> getb1()
    {
        return copy_matrix(bias1);
    }
    public Matrix <float> getb2()
    {
        return copy_matrix(bias2);
    }
     public Matrix <float> getb3()
    {
        return copy_matrix(bias3);
    }
    public Matrix <float> getInputs()
    {
        return copy_matrix(input_values);
    }
    
    public float getFitness()
    {
        return fitness;
    }
    public void setAlive(bool b)
    {
        if(b == false)
        {
            
            if(alive == true)
            {
                GetComponent<MeshRenderer>().material.SetColor("_Color",Color.grey);
                popM.inc_dead();
                this.input_values = Matrix<float>.Build.Dense(n_input,1,0);
                //fitness = 1 /  Mathf.Pow(2, ( Vector3.Distance(transform.position,target_pos))* Time.fixedDeltaTime );
            }
            GetComponent<Collider>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color",Color.red);
            GetComponent<Collider>().enabled = true;   
        }
        alive = b;
    }
    public bool isAlive()
    {
        
        return alive;
    }


}
