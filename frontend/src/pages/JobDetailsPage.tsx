import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { applyToJob } from "../services/applicationsService";
import { getJobById } from "../services/jobsService";
import type { JobDetail } from "../types/models";

export function JobDetailsPage() {
  const { id } = useParams();
  const { auth } = useAuth();
  const [job, setJob] = useState<JobDetail | null>(null);
  const [message, setMessage] = useState("");

  useEffect(() => {
    if (!id) return;
    getJobById(id)
      .then(setJob)
      .catch((err: Error) => setMessage(err.message));
  }, [id]);

  async function onApply() {
    if (!id) return;

    try {
      await applyToJob(id);
      setMessage("Application submitted successfully.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  if (!job) {
    return <section className="mx-auto max-w-4xl px-4 py-10 text-slate-600">Loading job details...</section>;
  }

  return (
    <section className="mx-auto max-w-4xl px-4 py-8">
      <article className="rounded-2xl border border-violet-200 bg-white p-6 shadow-md">
        <h1 className="text-3xl font-bold text-violet-900">{job.title}</h1>
        <p className="mt-3 text-slate-700">{job.description}</p>

        <div className="mt-5 flex flex-wrap gap-3 text-sm text-slate-500">
          <span>Company: {job.companyEmail}</span>
          <span>Posted: {new Date(job.createdAt).toLocaleDateString()}</span>
        </div>

        {auth?.role === "User" && (
          <button onClick={onApply} className="mt-6 rounded-lg bg-violet-600 px-4 py-2 font-semibold text-white hover:bg-violet-700">
            Apply Now
          </button>
        )}

        {message && <p className="mt-4 rounded-lg bg-violet-100 p-3 text-sm text-violet-800">{message}</p>}
      </article>
    </section>
  );
}
