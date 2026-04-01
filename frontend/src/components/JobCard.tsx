import { Link } from "react-router-dom";
import type { JobSummary } from "../types/models";

export function JobCard({ job }: { job: JobSummary }) {
  return (
    <article className="group rounded-xl border border-violet-200/60 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-lg">
      <h3 className="text-lg font-semibold text-violet-900">{job.title}</h3>
      <p className="mt-2 text-sm text-slate-600">{job.description}</p>
      <div className="mt-4 flex items-center justify-between">
        <span className="text-xs text-slate-500">{new Date(job.createdAt).toLocaleDateString()}</span>
        <Link
          to={`/jobs/${job.id}`}
          className="rounded-md bg-violet-600 px-3 py-1.5 text-xs font-semibold text-white hover:bg-violet-700"
        >
          View Details
        </Link>
      </div>
    </article>
  );
}
