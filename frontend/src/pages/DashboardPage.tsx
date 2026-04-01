import { useEffect, useMemo, useState } from "react";
import type { FormEvent } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { applyToJob, getApplicationsForJob } from "../services/applicationsService";
import { createJob, deleteJob, getJobs, updateJob } from "../services/jobsService";
import type { JobApplication, JobSummary } from "../types/models";

export function DashboardPage() {
  const { auth } = useAuth();
  const [jobs, setJobs] = useState<JobSummary[]>([]);
  const [applications, setApplications] = useState<JobApplication[]>([]);
  const [jobTitle, setJobTitle] = useState("");
  const [jobDescription, setJobDescription] = useState("");
  const [editingJobId, setEditingJobId] = useState("");
  const [editingTitle, setEditingTitle] = useState("");
  const [editingDescription, setEditingDescription] = useState("");
  const [selectedJobId, setSelectedJobId] = useState("");
  const [message, setMessage] = useState("");

  // Keeps company-only actions scoped to the signed-in company.
  const companyJobs = useMemo(() => jobs.filter((x) => x.companyId === auth?.userId), [jobs, auth?.userId]);

  useEffect(() => {
    getJobs(1, 50)
      .then((result) => setJobs(result.items))
      .catch((err: Error) => setMessage(err.message));
  }, []);

  async function onCreateJob(event: FormEvent) {
    event.preventDefault();

    try {
      await createJob(jobTitle, jobDescription);
      setJobTitle("");
      setJobDescription("");
      // Refreshes from server to avoid stale local dashboard state.
      const refreshed = await getJobs(1, 50);
      setJobs(refreshed.items);
      setMessage("Job post created.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  async function onApply(jobId: string) {
    try {
      await applyToJob(jobId);
      setMessage("Application submitted successfully.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  function onEditStart(job: JobSummary) {
    // Holds a temporary inline edit state without mutating the list item directly.
    setEditingJobId(job.id);
    setEditingTitle(job.title);
    setEditingDescription(job.description);
  }

  function onEditCancel() {
    setEditingJobId("");
    setEditingTitle("");
    setEditingDescription("");
  }

  async function onEditSave(jobId: string) {
    try {
      await updateJob(jobId, editingTitle, editingDescription);
      // Re-reads list after update so all users see backend-truth values.
      const refreshed = await getJobs(1, 50);
      setJobs(refreshed.items);
      onEditCancel();
      setMessage("Job updated.");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  async function onDelete(jobId: string) {
    try {
      await deleteJob(jobId);
      setJobs((prev) => prev.filter((x) => x.id !== jobId));
      setMessage("Job deleted.");
      // Clears applicant panel if the selected job was removed.
      if (selectedJobId === jobId) {
        setSelectedJobId("");
        setApplications([]);
      }
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  async function onLoadApplicants() {
    if (!selectedJobId) return;

    try {
      const result = await getApplicationsForJob(selectedJobId);
      setApplications(result);
      setMessage("");
    } catch (err) {
      setMessage((err as Error).message);
    }
  }

  if (!auth) {
    return null;
  }

  return (
    <section className="mx-auto max-w-6xl px-4 py-8">
      <h1 className="text-3xl font-bold text-violet-900">{auth.role} Dashboard</h1>
      <p className="mt-2 text-slate-600">Signed in as {auth.email}</p>

      {message && <p className="mt-4 rounded-md bg-violet-100 p-3 text-sm text-violet-800">{message}</p>}

      {auth.role === "Company" ? (
        <div className="mt-6 grid gap-6 lg:grid-cols-2">
          <form onSubmit={onCreateJob} className="rounded-2xl border border-violet-200 bg-white p-5 shadow-sm">
            <h2 className="text-xl font-semibold text-violet-900">Create Job</h2>
            <input
              placeholder="Job title"
              value={jobTitle}
              onChange={(e) => setJobTitle(e.target.value)}
              className="mt-4 w-full rounded-md border border-violet-200 px-3 py-2"
              required
            />
            <textarea
              placeholder="Description"
              value={jobDescription}
              onChange={(e) => setJobDescription(e.target.value)}
              className="mt-3 min-h-28 w-full rounded-md border border-violet-200 px-3 py-2"
              required
            />
            <button className="mt-3 rounded-md bg-violet-600 px-4 py-2 text-white">Post Job</button>
          </form>

          <div className="rounded-2xl border border-violet-200 bg-white p-5 shadow-sm">
            <h2 className="text-xl font-semibold text-violet-900">View Applicants</h2>
            <select
              value={selectedJobId}
              onChange={(e) => setSelectedJobId(e.target.value)}
              className="mt-4 w-full rounded-md border border-violet-200 px-3 py-2"
            >
              <option value="">Select your job</option>
              {companyJobs.map((job) => (
                <option key={job.id} value={job.id}>{job.title}</option>
              ))}
            </select>
            <button onClick={onLoadApplicants} className="mt-3 rounded-md bg-violet-600 px-4 py-2 text-white">
              Load Applicants
            </button>
            <ul className="mt-4 space-y-2 text-sm">
              {applications.map((a) => (
                <li key={a.id} className="rounded-md bg-violet-50 p-2">{a.userEmail}</li>
              ))}
            </ul>
          </div>

          <div className="rounded-2xl border border-violet-200 bg-white p-5 shadow-sm lg:col-span-2">
            <h2 className="text-xl font-semibold text-violet-900">Your Job Posts</h2>
            <ul className="mt-4 space-y-3">
              {companyJobs.map((job) => (
                <li key={job.id} className="rounded-md border border-violet-200 p-3">
                  {editingJobId === job.id ? (
                    <div>
                      <input
                        value={editingTitle}
                        onChange={(e) => setEditingTitle(e.target.value)}
                        className="w-full rounded-md border border-violet-200 px-3 py-2"
                      />
                      <textarea
                        value={editingDescription}
                        onChange={(e) => setEditingDescription(e.target.value)}
                        className="mt-2 min-h-24 w-full rounded-md border border-violet-200 px-3 py-2"
                      />
                      <div className="mt-3 flex gap-2">
                        <button onClick={() => onEditSave(job.id)} className="rounded-md bg-violet-600 px-3 py-1 text-white">
                          Save
                        </button>
                        <button onClick={onEditCancel} className="rounded-md bg-slate-200 px-3 py-1 text-slate-700">
                          Cancel
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div>
                      <h3 className="font-semibold text-violet-900">{job.title}</h3>
                      <p className="mt-1 text-sm text-slate-600">{job.description}</p>
                      <div className="mt-3 flex gap-2">
                        <button onClick={() => onEditStart(job)} className="rounded-md bg-violet-600 px-3 py-1 text-white">
                          Edit
                        </button>
                        <button onClick={() => onDelete(job.id)} className="rounded-md bg-rose-600 px-3 py-1 text-white">
                          Delete
                        </button>
                      </div>
                    </div>
                  )}
                </li>
              ))}
            </ul>
          </div>
        </div>
      ) : (
        <div className="mt-6 rounded-2xl border border-violet-200 bg-white p-6 shadow-sm">
          <h2 className="text-xl font-semibold text-violet-900">Open Positions</h2>
          <p className="mt-2 text-slate-600">Apply directly from your dashboard or review details first.</p>
          <ul className="mt-4 space-y-3">
            {jobs.map((job) => (
              <li key={job.id} className="rounded-md border border-violet-200 p-3">
                <h3 className="font-semibold text-violet-900">{job.title}</h3>
                <p className="mt-1 text-sm text-slate-600">{job.description}</p>
                <div className="mt-3 flex gap-2">
                  <button onClick={() => onApply(job.id)} className="rounded-md bg-violet-600 px-3 py-1 text-white">
                    Apply
                  </button>
                  <Link to={`/jobs/${job.id}`} className="rounded-md bg-slate-200 px-3 py-1 text-slate-700">
                    Details
                  </Link>
                </div>
              </li>
            ))}
          </ul>
        </div>
      )}
    </section>
  );
}
