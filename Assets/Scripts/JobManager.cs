using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JobManager : MonoBehaviour {
    public static JobManager Instance = null;

    [SerializeField] private int maxJobs = 2;
    public List<Job> jobs = new List<Job>();

    [Header("City Locations")]
    [SerializeField] private List<BuildingSO> buildings;

    [Header("Goods")]
    [SerializeField] private List<GoodsSO> goods;

    // ----- EVENTS -----
    [System.Serializable] public class JobManagerEvent: UnityEvent<List<Job>> { }
    [System.Serializable] public class JobClaimedEvent : UnityEvent<Job> { }
    public JobManagerEvent jobChangeEvent;
    public JobClaimedEvent jobClaimedEvent;

    private void Awake() {
        if (jobChangeEvent == null) { jobChangeEvent = new JobManagerEvent(); }
        if (jobClaimedEvent == null) { jobClaimedEvent = new JobClaimedEvent(); }
        if (Instance == null) { Instance = this; }
    }

    void Update() {
        if (jobs.Count < maxJobs) {  CreateJobs(); }
    }

    private Job RandomJob() {
        // create a job randomly

        // find a goods to deliver
        GoodsSO delivery = goods[Random.Range(0, goods.Count - 1)];

        // find an origin for those goods -- building must have a customer that produces the goods
        int origin = 0;
        bool valid = false;
        while (!valid) {
            origin = Random.Range(0, buildings.Count);
            valid = buildings[origin].customer != null && buildings[origin].customer.Produces(delivery);
        }

        // find a destination for the goods - must not be origin, must have a customer that accepts the goods
        int destination = 0;
        valid = false;
        while (!valid) {
            destination = Random.Range(0, buildings.Count);
            valid = (origin != destination) && buildings[destination].customer != null && buildings[destination].customer.Accepts(delivery);
        }


        Job job = Job.CreateJob("Deliver package", buildings[origin], buildings[destination], delivery, 
            Random.Range(1, 20), Random.Range(5, 10), Random.Range(10, 60));
        return job;
    }

    public void CreateJobs() {
        while (jobs.Count < maxJobs) {
            Job job = RandomJob();

            // add to list of jobs
            AddJob(job);
        }
    }

    public void AddJob(Job job) {
        if (jobs.Count == maxJobs) { return; }
        jobs.Add(job);
        jobChangeEvent?.Invoke(jobs);
    }

    public void JobClaimed(Job job) {
        jobs.Remove(job);
    }
}
