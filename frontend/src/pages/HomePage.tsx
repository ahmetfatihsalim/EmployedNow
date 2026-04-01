import { useEffect, useState } from "react";
import { JobCard } from "../components/JobCard";
import { getJobs } from "../services/jobsService";
import type { JobSummary } from "../types/models";

export function HomePage() {
  const [jobs, setJobs] = useState<JobSummary[]>([]);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    getJobs(1, 20)
      .then((result) => setJobs(result.items))
      .catch((err: Error) => setError(err.message));
  }, []);

  return (
    <section className="mx-auto max-w-6xl px-4 py-8">
      <div className="mb-8 rounded-2xl bg-gradient-to-r from-violet-700 via-fuchsia-700 to-purple-700 p-6 text-white shadow-xl">
        <h1 className="text-3xl font-bold">Find your next role with confidence</h1>
        <p className="mt-2 text-violet-100">Public job board powered by your EmployedNow backend.</p>
      </div>

      {error && <p className="mb-4 rounded-lg bg-rose-100 p-3 text-sm text-rose-700">{error}</p>}

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {jobs.map((job) => (
          <JobCard key={job.id} job={job} />
        ))}
      </div>
    </section>
  );
}
