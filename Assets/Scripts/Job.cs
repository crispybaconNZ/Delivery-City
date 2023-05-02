using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {
    public string job_name;         // short name for the job e.g., "Deliver package"
    public BuildingSO origin;       // where to pick up the package from
    public BuildingSO destination;  // where to deliver the package to
    public GoodsSO goods;           // picture of the package to use on screen
    public int reward;              // how much $$$ this job pays
    public int penalty;             // how much $$$ this job costs on failure
    public float time_limit;        // amount of time player has to complete this job (in seconds)

    public bool Expired { get { return time_limit <= 0; } }

    public static Job CreateJob(string job_name, BuildingSO origin, BuildingSO destination, GoodsSO goods, int reward, int penalty, int time_limit_secs) {
        Job job = new Job();
        job.job_name = job_name;
        job.origin = origin;
        job.destination = destination;
        job.goods = goods;
        job.reward = reward;
        job.penalty = penalty;
        job.time_limit = time_limit_secs;

        return job;
    }

    public override string ToString() {
        return $"{job_name}: {goods.goodsName}, {origin.name} => {destination.name}, ${reward} (-${penalty}), {time_limit} seconds";
    }

    public void ReduceTime(float delta) {
        time_limit -= delta;
    }


}
